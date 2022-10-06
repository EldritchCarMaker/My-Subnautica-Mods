using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EquivalentExchange.Monobehaviours
{
    public class AutomaticItemConverter : HandTarget, IHandTarget
    {
        private StorageContainer container;
        public TechType itemType;
        private GameObject prefab;
        private Pickupable pickupable;
        private float EMCCost;

        private bool isClearing;

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

            id = GetComponent<UniqueIdentifier>().id;

            foreach(var oldItem in GetComponentsInChildren<Pickupable>(true))
            {
                Destroy(oldItem.gameObject);
            }
        }
        public void Start()
        {
            LoadSaveData();
            QMod.SaveData.OnStartedSaving += OnStartedSaving;
        }
        public void LoadSaveData()
        {
            if(QMod.SaveData.AutoItemConverters.TryGetValue(id, out var value))
                SetItemType(value.ToString(), false);
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

                container.onUse = new StorageContainer.UseEvent();

                root.SetActive(true);
                container.enabled = true;
            }
            container.CreateContainer();

            container.onUse.AddListener(ToggleDoorState);
            container.container.onRemoveItem += OnRemove;
            container.container.isAllowedToRemove += AllowedToRemove;
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
            var size = CraftData.GetItemSize(itemType);
            if (itemType != TechType.None && container.container.HasRoomFor(size.x, size.y))
            {
                //if (!prefab) prefab = CraftData.GetPrefabForTechType(itemType);
                if(!prefab)
                    return;

                container.container.AddItem(Instantiate(prefab).GetComponent<Pickupable>()).item.gameObject.SetActive(false);
            }
        }
        public void OnRemove(InventoryItem item)
        {
            if(!isClearing)//OnRemove event is called on clear too, don't like that
                QMod.SaveData.EMCAvailable -= EMCCost;
        }
        public bool AllowedToRemove(Pickupable item, bool verbose)
        {
            if(QMod.SaveData.EMCAvailable >= EMCCost)
                return true;

            return false;
        }
        public void OnHandClick(GUIHand hand)
        {
            if(!constructable.constructed) return;

            if(GameInput.GetButtonHeld(GameInput.Button.Sprint))
            {
                SetItemType();
            }
            else
            {
                //container.OnHandClick(hand); seriously why does this not work?????? it works like half the time at first, then decides to just never work again
                PDA pda = Player.main.GetPDA();
                Inventory.main.SetUsedStorage(container.container, false);
                pda.Open(PDATab.Inventory);
            }
        }
        public void SetItemType()
        {
            uGUI.main.userInput.RequestString("Item to convert", "Submit", itemType.ToString(), 20, new uGUI_UserInput.UserInputCallback(OnStringInput));
        }
        private void OnStringInput(string name) => SetItemType(name);
        private void SetItemType(string name, bool showMessage = true)
        {
            TechType type = QMod.GetTechType(name);

            itemType = type;

            isClearing = true;
            container.container.Clear();
            isClearing = false;

            if (type == TechType.None)
            {
                if(showMessage) ErrorMessage.AddMessage($"Could not find TechType {name}, stopping auto conversion");
                return;
            }

            if(!QMod.SaveData.learntTechTypes.Contains(type))
            {
                if (showMessage) ErrorMessage.AddMessage("Can only select items you've unlocked");
                return;
            }

            if (prefab) Destroy(prefab);

            prefab = Instantiate(CraftData.GetPrefabForTechType(type));
            if(!prefab || !prefab.TryGetComponent(out pickupable))
            {
                if (showMessage) ErrorMessage.AddMessage("How the fuck did you unlock an item that couldn't be picked up? This item isn't valid for this, and stop cheating");
                return;
            }
            prefab.transform.SetParent(transform, false);
            prefab.transform.position = transform.position;
            prefab.SetActive(false);

            if (showMessage) ErrorMessage.AddMessage("Set item");
            EMCCost = ExchangeMenu.GetCost(type);
            var itemSize = CraftData.GetItemSize(type);
            container.Resize(itemSize.x, itemSize.y);
        }
        public void OnHandHover(GUIHand hand)
        {
            if (!constructable.constructed) return;

            HandReticle.main.SetInteractText("Open Item Converter", $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Set Item Type", uGUI.FormatButton(GameInput.Button.Sprint))}");
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

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
