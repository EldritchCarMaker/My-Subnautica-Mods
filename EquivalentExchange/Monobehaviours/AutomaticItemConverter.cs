using SMLHelper.V2.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace EquivalentExchange.Monobehaviours
{
    public class AutomaticItemConverter : HandTarget, IHandTarget
    {
        private StorageContainer container;
        public TechType itemType;
        private GameObject prefab;
        private Pickupable pickupable;
        private float ECMCost;

        private bool isClearing;
        private bool isListeningForTechType;

        private string id;
        public override void Awake()
        {
            base.Awake();

            MedicalCabinet cabinet = GetComponent<MedicalCabinet>();

            //SetUpDoor(cabinet);

            Destroy(cabinet);

            SetUpContainer();

            Destroy(GetComponent<LiveMixin>());

            constructable = GetComponent<Constructable>();

            CoroutineHost.StartCoroutine(WaitForExchangeMenu());

            foreach(var oldItem in GetComponentsInChildren<Pickupable>(true))
            {
                Destroy(oldItem.gameObject);
            }
        }
        public IEnumerator WaitForExchangeMenu()
        {
            yield return new WaitUntil(() => ExchangeMenu.singleton != null);
            ExchangeMenu.singleton.OnClose += OnExchangeMenuClose;
            ExchangeMenu.singleton.onPointerClick.Add(OnExchangeMenuClick);
        }
        public void Start()
        {
            id = GetComponent<UniqueIdentifier>().id;

            LoadSaveData();
            QMod.SaveData.OnStartedSaving += OnStartedSaving;
        }
        public void LoadSaveData()
        {
            if(QMod.SaveData.AutoItemConverters.TryGetValue(id, out var value))
                SetItemType(value, false);
        }
        private void OnStartedSaving(object sender, EventArgs e)
        {
            if (QMod.SaveData.AutoItemConverters.ContainsKey(id))
                QMod.SaveData.AutoItemConverters[id] = itemType;
            else
                QMod.SaveData.AutoItemConverters.Add(id, itemType);
        }
        public void SetUpDoor(MedicalCabinet cabinet)
        {
            doorRenderer = cabinet.doorRenderer;
            doorMat = doorRenderer.material;
            //doorMat.SetFloat(ShaderPropertyID._GlowStrength, 0f);
            //doorMat.SetFloat(ShaderPropertyID._GlowStrengthNight, 0f);

            doorOpenTransform = cabinet.doorOpenTransform;
            doorOpenQuat = doorOpenTransform.localRotation;
            door = cabinet.door;
            doorCloseQuat = door.transform.localRotation;

            openSFX = cabinet.openSFX;
            closeSFX = cabinet.closeSFX;
        }
        public void SetUpContainer()
        {
            if (container) return;

            container = GetComponentInChildren<StorageContainer>();

            if(!container)
            {
                var root = new GameObject("StorageRoot");
                root.transform.parent = transform;
                root.SetActive(false);

                var coi = root.EnsureComponent<ChildObjectIdentifier>();

                container = root.EnsureComponent<StorageContainer>();

                container.enabled = false;

                container.prefabRoot = gameObject;
                container.storageRoot = coi;

                coi.classId = "AutoItemConverter";

                container.width = 1;
                container.height = 1;
                container.storageLabel = "Auto Item Converter";
#if SN
                container.onUse = new StorageContainer.UseEvent();
#endif

                root.SetActive(true);
                container.enabled = true;
            }
            container.CreateContainer();
#if SN
            container.onUse.AddListener(ToggleDoorState);
#endif
            container.container.onRemoveItem += OnRemove;
            container.container.isAllowedToRemove += AllowedToRemove;
            container.container.isAllowedToAdd += (pickupable, verbose) => false;
        }
        private void ToggleDoorState()
        {
            //SetDoorState(!doorOpen);
        }
        private void SetDoorState(bool open)
        {
            changeDoorState = true;
            doorOpen = open;
            (doorOpen ? openSFX : closeSFX).Play();
            CancelInvoke(nameof(DoorInactive));
            Invoke(nameof(DoorInactive), 4f);
        }
        private void DoorInactive()
        {
            changeDoorState = false;
        }
        public void Update()
        {
            if (false && changeDoorState)
            {
                Quaternion targetQuat = doorOpen ? doorOpenQuat : doorCloseQuat;
                door.transform.localRotation = Quaternion.Slerp(door.transform.localRotation, targetQuat, Time.deltaTime * 5f);
            }

            var size =
#if SN
                CraftData.GetItemSize(itemType);
#else
                TechData.GetItemSize(itemType);
#endif

            if (itemType != TechType.None && container.container.HasRoomFor(size.x, size.y))
            {
                //if (!prefab) prefab = CraftData.GetPrefabForTechType(itemType);
                if(!prefab)
                    return;
                if (!Instantiate(prefab).TryGetComponent(out Pickupable pickupable)) { ClearItem(); return; }

                var item = new InventoryItem(pickupable);
                container.container.UnsafeAdd(item);
                pickupable.gameObject.SetActive(false);
            }
        }
        public void OnRemove(InventoryItem item)
        {
            if(!isClearing)//OnRemove event is called on clear too, don't like that
                QMod.SaveData.ECMAvailable -= ECMCost;
        }
        public bool AllowedToRemove(Pickupable item, bool verbose)
        {
            if(QMod.SaveData.ECMAvailable >= ECMCost)
                return true;

            return false;
        }
        public void OnHandClick(GUIHand hand)
        {
            if(!constructable.constructed) return;

            if(GameInput.GetButtonHeld(GameInput.Button.Sprint))
            {
                //SetItemType();
                isListeningForTechType = true;
                ExchangeMenu.GetInstance().Open();
            }
            else
            {
                //container.OnHandClick(hand); seriously why does this not work?????? it works like half the time at first, then decides to just never work again
                PDA pda = Player.main.GetPDA();
                Inventory.main.SetUsedStorage(container.container, false);
                pda.Open(PDATab.Inventory);
            }
        }
        public bool OnExchangeMenuClick(TechType techType, out ExchangeMenu.IconClickEffectsType iconType)
        {
            iconType = ExchangeMenu.IconClickEffectsType.None;
            if (!isListeningForTechType) return true;

            //uGUI.main.userInput.RequestString("Item to convert", "Submit", itemType.ToString(), 20, new uGUI_UserInput.UserInputCallback(OnStringInput));
            SetItemType(techType);

            isListeningForTechType = false;

            iconType = ExchangeMenu.IconClickEffectsType.ClickAllowed;

            ExchangeMenu.GetInstance().Close();

            return false;
        }

        private void SetItemType(string name, bool showMessage = true)
        {
            TechType type = QMod.GetTechType(name);

            SetItemType(type, showMessage);
        }
        private void SetItemType(TechType type, bool showMessage = true)
        {
            if(type == QMod.FCSConvertType || type == QMod.FCSConvertBackType)
            {
                if (showMessage)
                    ErrorMessage.AddMessage("Can't set the item converter to FCS conversions");
                return;
            }

            ClearItem();

            if(type == itemType)
            {
                if (showMessage) ErrorMessage.AddMessage("Cleared item selection");
                itemType = TechType.None;
                return;
            }

            itemType = type;

            if (type == TechType.None)
            {
                if (showMessage) ErrorMessage.AddMessage("Could not find TechType, stopping auto conversion");
                return;
            }

            if (!QMod.SaveData.learntTechTypes.Contains(type))
            {
                if (showMessage) ErrorMessage.AddMessage("Can only select items you've unlocked");
                return;
            }
#if SN1
            SetItemPrefab(type, showMessage);
#else 
            CoroutineHost.StartCoroutine(SetItemPrefab(type, showMessage));
#endif
        }
#if SN1
        public void SetItemPrefab(TechType type, bool showMessage)
        {
            prefab = Instantiate(CraftData.GetPrefabForTechType(type));
            if (!prefab || !prefab.TryGetComponent(out pickupable))
            {
                if (showMessage) ErrorMessage.AddMessage("How the fuck did you unlock an item that couldn't be picked up? This item isn't valid for this, and stop cheating");
                if (prefab) Destroy(prefab);
                return;
            }
            prefab.transform.SetParent(transform, false);
            prefab.transform.position = transform.position;
            prefab.SetActive(false);

            if (showMessage) ErrorMessage.AddMessage("Set item");
            ECMCost = ExchangeMenu.GetCost(type);
            var itemSize = CraftData.GetItemSize(type);
            container.Resize(itemSize.x, itemSize.y);
        }
#else
        public IEnumerator SetItemPrefab(TechType type, bool showMessage)
        {
            var result = new TaskResult<GameObject>();
            yield return CraftData.InstantiateFromPrefabAsync(type, result);
            prefab = result.Get();

            if (!prefab || !prefab.TryGetComponent(out pickupable))
            {
                if (showMessage) ErrorMessage.AddMessage("How the fuck did you unlock an item that couldn't be picked up? This item isn't valid for this, and stop cheating");
                if (prefab) Destroy(prefab);
                yield break;
            }
            prefab.transform.SetParent(transform, false);
            prefab.transform.position = transform.position;
            prefab.SetActive(false);

            if (showMessage) ErrorMessage.AddMessage("Set item");
            ECMCost = ExchangeMenu.GetCost(type);
#if SN2
            var itemSize = CraftData.GetItemSize(type);
#else
            var itemSize = TechData.GetItemSize(type);
#endif
            container.Resize(itemSize.x, itemSize.y);
        }
#endif
        public void ClearItem()
        {
            if (prefab) Destroy(prefab);
            container.Resize(1, 1);
            isClearing = true;
            container.container.Clear();
            isClearing = false;
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!constructable.constructed) return;
#if SN1
            HandReticle.main.SetInteractText("Open Item Converter", $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Set Item Type", uGUI.FormatButton(GameInput.Button.Sprint))}");
#else
            HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, "Open Item Converter");
            HandReticle.main.SetTextRaw(HandReticle.TextType.HandSubscript, $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Set Item Type", uGUI.FormatButton(GameInput.Button.Sprint))}");
#endif
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
        }
        public void OnExchangeMenuClose() => isListeningForTechType = false;

        public FMOD_CustomEmitter openSFX;

        public FMOD_CustomEmitter closeSFX;

        public Constructable constructable;

        public FMOD_CustomEmitter playSound;

        public GameObject door;

        public Transform doorOpenTransform;

        public Renderer doorRenderer;

        private Material doorMat;

        private bool doorOpen;

        private bool changeDoorState;

        private Quaternion doorOpenQuat;

        private Quaternion doorCloseQuat;
    }
}
