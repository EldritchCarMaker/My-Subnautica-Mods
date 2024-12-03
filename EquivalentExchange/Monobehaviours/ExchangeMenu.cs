using EquivalentExchange.Patches;
using Nautilus.Handlers;
using Nautilus.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
#if !SN1
using TMPro;
#endif
using UnityEngine;
using UnityEngine.UI;
using UWE;
using static EquivalentExchange.QMod;
#if SN
using Sprite = Atlas.Sprite;
#endif

namespace EquivalentExchange.Monobehaviours
{
    public class ExchangeMenu : uGUI_InputGroup, uGUI_IIconGridManager, uGUI_IToolbarManager, uGUI_IButtonReceiver, INotificationListener, EventList<TechType>.IListListener
    {
		public const int PRICEOFUNMARKEDITEM = 50;
		//if you're a modder trying to change this value for your item, please use the ExternalModCompat class

		private static readonly Dictionary<TechType, int> cachedItemCosts = new Dictionary<TechType, int>();

		public bool state { get; private set; }

		public int TabOpen
		{
			get
			{
				return (int)selected;
			}
		}
		public int CurrentConvertAmount { get; private set; } = 1;
		public int TabCount
		{
			get
			{
				return Enum.GetValues(typeof(ExchangeMenuTab)).Length;
			}
		}

		public ScrollRect scrollRect;
		public GameObject scrollBar;

		public delegate void OnCloseDelegate();
		public OnCloseDelegate OnClose;
		public enum IconClickEffectsType
		{
			None,
			ClickAllowed,
			ClickNotAllowed
		}
		public delegate bool OnPointerClickDelegate(TechType clickedType, out IconClickEffectsType iconClick);
		public List<OnPointerClickDelegate> onPointerClick = new List<OnPointerClickDelegate>();
		public static bool IsOpen()
		{
			return ExchangeMenu.singleton != null && ExchangeMenu.singleton.state;
		}

		public override void Awake()
		{
			if (_singleton != null)
			{
				Debug.LogError("Multiple ExchangeMenu instances found in scene!", this);
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			_singleton = this;
			base.Awake();
			name = "ExchangeMenu";

			canvasScaler = GetComponent<uGUI_CanvasScaler>();
#if SN1
			title = transform.Find("Content").Find("Title").GetComponent<Text>();
#else
            title = transform.Find("Content").Find("Title").GetComponent<TextMeshProUGUI>();
#endif
			toolbar = transform.Find("Content").Find("Toolbar").GetComponent<uGUI_Toolbar>();
			iconGrid = transform.Find("Content").Find("ScrollView").Find("Viewport").Find("ScrollCanvas").GetComponent<uGUI_IconGrid>();
			content = transform.Find("Content").gameObject;
			scrollRect = GetComponentInChildren<ScrollRect>(true);
			scrollBar = transform.Find("Content/Scrollbar").gameObject;

			foreach (Transform transform in toolbar.transform)
				if (transform.name == "ToolbarIcon")
					Destroy(transform.gameObject);

			foreach (Transform transform in iconGrid.transform)
				if (transform.name == "GridIcon")
					Destroy(transform.gameObject);

			QMod.SaveData.learntTechTypes.AddListener(this);

			ClearNotificationCounts();
			iconGrid.iconSize = new Vector2(iconSize, iconSize);
			iconGrid.minSpaceX = minSpaceX;
			iconGrid.minSpaceY = minSpaceY;
			iconGrid.Initialize(this);
			int count = TabCount;
			Sprite[] array = new Sprite[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = GetSpriteForTabType((ExchangeMenuTab)i);

				/*
				TechGroup value = ExchangeMenu.groups[i];
				string str = groups[i].ToString();
				array[i] = SpriteManager.Get(SpriteManager.Group.Tab, "group" + str);
				*/
			}
			title.text = "EXCHANGE";
			object[] array2 = array;
			toolbar.Initialize(this, array2, null, 15);
			toolbar.Select((int)selected);
			UpdateItems();
            NotificationManager.main.Subscribe(this, notificationGroup, string.Empty);
#if !SN1
			GetComponent<uGUI_GraphicRaycaster>().enabled = true;//for some reason seems to be disabled by default on BZ and SN2.0?
#endif

			CreateEldritchLogo();
        }

		private void CreateEldritchLogo()
		{
			var content = transform.Find("Content");
			var obj = new GameObject("LogoObj");
			obj.transform.parent = content.transform;
			obj.transform.position = content.transform.position;
			obj.transform.localScale = Vector3.one;

			var assetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
            var image = ImageUtils.LoadSpriteFromFile(Path.Combine(assetsFolder, "eldritch_logo.png"));

			var icon = obj.EnsureComponent<uGUI_ItemIcon>();
			icon.SetForegroundSprite(image);
			var color = new Color(0, 0.7f, 1, 0.05f);
			icon.SetForegroundColors(color, color, color);

			obj.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
		}

		private Sprite GetSpriteForTabType(ExchangeMenuTab tab)
        {
			switch(tab)
            {
				case ExchangeMenuTab.RawMaterials:
					return SpriteManager.Get(TechType.Titanium);

				case ExchangeMenuTab.BiologicalMaterials:
					return SpriteManager.Get(TechType.AcidMushroom);

				case ExchangeMenuTab.CraftedItems:
					return SpriteManager.Get(TechType.WiringKit);

				case ExchangeMenuTab.Equipment:
					return SpriteManager.Get(TechType.Fins);

				case ExchangeMenuTab.Misc:
					return SpriteManager.Get(TechType.NutrientBlock);

				case ExchangeMenuTab.ModdedItems:
					return SpriteManager.Get(TechType.PrecursorKey_Red);
			}
			return SpriteManager.Get(TechType.Seamoth);//random one, idk
		}

		// Token: 0x06003453 RID: 13395 RVA: 0x00120195 File Offset: 0x0011E395
		private void Start()
		{
			if (!ModPresent("FCSAlterraHub"))
			{
				return;
			}

			QMod.TryUnlockTechType(QMod.FCSConvertType);
			QMod.TryUnlockTechType(QMod.FCSConvertBackType);

#if SN1
			SetFCSIcons();
        }
		public void SetFCSIcons()
		{
            var FCSScreen = ExternalModCompat.GetFCSPDA();

            if (!FCSScreen)
            {
                CoroutineHost.StartCoroutine(WaitForFCSPDA());
                return;
            }

            var iconTransform = FCSScreen.transform.Find("Information/ToggleHud/AccountBalanceIcon");
            if (!iconTransform) return;

            QMod.FCSCreditIconSprite = iconTransform.GetComponent<Image>().sprite;

            SMLHelper.V2.Handler.SpriteHandler.RegisterSprite(QMod.FCSConvertType, QMod.FCSCreditIconSprite);
            SMLHelper.V2.Handler.SpriteHandler.RegisterSprite(QMod.FCSConvertBackType, QMod.FCSCreditIconSprite);
        }
		public IEnumerator WaitForFCSPDA()
		{
			yield return new WaitUntil(() => ExternalModCompat.GetFCSPDA());
			SetFCSIcons();
		}
#else
        }
#endif

        public bool GetIsLeftMouseBoundToRightHand()
		{
			return "MouseButtonLeft" == GameInput.GetBinding(GameInput.Device.Keyboard, GameInput.Button.RightHand, GameInput.BindingSet.Primary) || "MouseButtonLeft" == GameInput.GetBinding(GameInput.Device.Keyboard, GameInput.Button.RightHand, GameInput.BindingSet.Secondary);
		}

		public override void Update()
		{
			base.Update();
			if (state && openInFrame != Time.frameCount)
			{
				bool flag = GameInput.GetButtonDown(GameInput.Button.RightHand);
				if (GetIsLeftMouseBoundToRightHand())
				{
					flag = false;
				}
				if (GameInput.GetButtonDown(GameInput.Button.UICancel) || flag || !base.focused)
				{
					Close();
				}
				UpdateConvertAmount();
			}
			if(Input.GetKeyDown(QMod.config.menuKey) && Input.GetKeyDown(QMod.config.menuKey2))
            {
				Open();
            }
		}

		// Token: 0x06003456 RID: 13398 RVA: 0x00120232 File Offset: 0x0011E432
		private void LateUpdate()
		{
			if (state)
			{
				UpdateToolbarNotificationNumbers();
			}
		}

		// Token: 0x06003457 RID: 13399 RVA: 0x00120244 File Offset: 0x0011E444
		private void OnDestroy()
		{
			ExchangeMenu._singleton = null;
		}

		// Token: 0x06003458 RID: 13400 RVA: 0x001202D3 File Offset: 0x0011E4D3
		public override void OnSelect(bool lockMovement)
		{
			base.OnSelect(lockMovement);
			GamepadInputModule.current.SetCurrentGrid(iconGrid);
		}

		// Token: 0x06003459 RID: 13401 RVA: 0x001202EC File Offset: 0x0011E4EC
		public override void OnDeselect()
		{
			base.OnDeselect();
			Close();
		}

		// Token: 0x0600345A RID: 13402 RVA: 0x001202FC File Offset: 0x0011E4FC
		public static void Show()
		{
			ExchangeMenu instance = ExchangeMenu.GetInstance();
			if (instance != null)
			{
				instance.Open();
			}
		}

		// Token: 0x0600345B RID: 13403 RVA: 0x00120320 File Offset: 0x0011E520
		public static void Hide()
		{
			ExchangeMenu instance = ExchangeMenu.GetInstance();
			if (instance != null)
			{
				instance.Close();
			}
		}

		// Token: 0x0600345C RID: 13404 RVA: 0x00120342 File Offset: 0x0011E542
		public static void EnsureCreated()
		{
			ExchangeMenu.GetInstance();
		}

		// Token: 0x0600345D RID: 13405 RVA: 0x0012034A File Offset: 0x0011E54A
		public void Open()
		{
			if (state)
			{
				return;
			}
			UpdateToolbarNotificationNumbers();
			UpdateItems();
			MainCameraControl.main.SaveLockedVRViewModelAngle();
			SetState(true);
			openInFrame = Time.frameCount;
		}

		// Token: 0x0600345E RID: 13406 RVA: 0x00120377 File Offset: 0x0011E577
		public void Close()
		{
			if (!state)
			{
				return;
			}
			SetState(false);
		}
#if SN1
		public void GetToolbarTooltip(int index, out string tooltipText, List<TooltipIcon> tooltipIcons)
		{
			tooltipText = null;
			if (index < 0 || index >= TabCount)
			{
				return;
			}
			tooltipText = TooltipFactory.Label(groupNames[(ExchangeMenuTab)index]);
		}
#else
		public void GetToolbarTooltip(int index, TooltipData data)
		{
			if (index < 0 || index >= TabCount)
			{
				return;
			}
			TooltipFactory.Label(groupNames[(ExchangeMenuTab)index], data.prefix);
		}
#endif

		// Token: 0x06003463 RID: 13411 RVA: 0x001203B7 File Offset: 0x0011E5B7
		public void OnToolbarClick(int index, int button)
		{
			if (button == 0)
			{
				SetCurrentTab(index);
			}
		}

#if SN1
		public void GetTooltip(string id, out string tooltipText, List<TooltipIcon> tooltipIcons)
		{
			TechType techType;
			if (items.TryGetValue(id, out techType))
			{
				//TooltipFactory.BuildTech(techType, locked, out tooltipText, tooltipIcons);
				var sb = new StringBuilder();

                WriteDetails(techType, sb);
				tooltipText = sb.ToString();
				return;
			}
			tooltipText = null;
		}
#else
		public void GetTooltip(string id, TooltipData data)
		{
			TechType techType;
			if (items.TryGetValue(id, out techType))
			{
				//TooltipFactory.BuildTech(techType, locked, out tooltipText, tooltipIcons);
				WriteDetails(techType, data.prefix);
			}
		}
#endif

		public void WriteDetails(TechType techType, StringBuilder stringBuilder)
        {
			TooltipFactory.Initialize();
			string key = techType.AsString(false);

			TooltipFactory.WriteTitle(stringBuilder, Language.main.Get(key) + (CurrentConvertAmount > 1 ? $" ({CurrentConvertAmount})" : ""));
			TooltipFactory.WriteDescription(stringBuilder, Language.main.Get(TooltipFactory.techTypeTooltipStrings.Get(techType)));

#if SN1
			if (techType != QMod.FCSConvertBackType)
				WriteCost(techType, stringBuilder);
			else
				WriteFCSCost(techType, stringBuilder);
#else
            WriteCost(techType, stringBuilder);
#endif
		}
		public void WriteCost(TechType techType, StringBuilder stringBuilder)
        {
			stringBuilder.Append(Environment.NewLine);

			float amount = GetCost(techType) * CurrentConvertAmount;
			
			ECMFont.Format(amount, stringBuilder);
		}
#if SN1
		public void WriteFCSCost(TechType techType, StringBuilder stringBuilder)
		{
            stringBuilder.Append(Environment.NewLine);

			float current = (float)ExternalModCompat.GetFCSCredit();
            float amount = QMod.ECMConvertPerClick * QMod.ECMToFCSCreditRate * CurrentConvertAmount;
            bool flag = current >= amount ||
#if SN
				!GameModeUtils.RequiresIngredients();
#else
                !GameModeManager.GetOption<bool>(GameOption.CraftingRequiresResources);
#endif

            if (flag)
            {
                stringBuilder.Append("<color=#94DE00FF>");
            }
            else
            {
                stringBuilder.Append("<color=#DF4026FF>");
            }

            stringBuilder.Append("Credit:");

            if (amount > 1)
            {
                stringBuilder.Append(" x");
                stringBuilder.Append(amount);
            }
            if (/*current > 0 && current < amount*/
                true)
            {
                stringBuilder.Append(" (");
                stringBuilder.Append(current);
                stringBuilder.Append(")");
            }
            stringBuilder.Append("</color>");
        }
#endif

        public static float GetCost(TechType techType, int depth = 0, bool useCreative = true, bool useEfficiency = true)
        {
			if (useCreative &&
#if SN
								!GameModeUtils.RequiresIngredients()
#else
								!GameModeManager.GetOption<bool>(GameOption.CraftingRequiresResources)
#endif
								)
                return 0;

			if (TryGetConfigCost(techType, out var cost)) return cost;//use config dictionaries first, in case the player changed one of the configs

			if (techType == QMod.FCSConvertType)
				return QMod.ECMConvertPerClick;

			if (cachedItemCosts.TryGetValue(techType, out cost)) return cost;

			if (depth > 10) return PRICEOFUNMARKEDITEM;

			float totalCost = 0;

#if SN
			var techData = CraftData.Get(techType, true);
#else
			var techData = TechData.GetIngredients(techType);
#endif

			if (techData == null)
            {
				return PRICEOFUNMARKEDITEM;
            }

			int count =
#if SN
				techData.ingredientCount;
#else
				techData.Count;
#endif

			for (var i = 0; i < count; i++)
			{
				var ingredient =
#if SN
					techData.GetIngredient(i);
#else
					techData[i];
#endif


				if (ingredient != null)
				{
					for (var j = 0; j < ingredient.amount; j++)
						totalCost += GetCost(ingredient.techType, depth+1, useCreative, useEfficiency); 

				}
			}

			var amountCrafted = techData.craftAmount;
			totalCost /= amountCrafted;//For things like pipes
			//Pipes cost 2 titanium, but craft 5 of them
			//So without this they're a net increase, pay 10 ECM for two titanium, craft into 5 pipes, each worth 10, now you have 40 extra ECM
			//This way it's worth 1/5th of the amount. So each pipe is only worth 2
			//Balances out to even this way

			cachedItemCosts.Add(techType, (int)totalCost);

            return useEfficiency ? totalCost * QMod.config.inefficiencyMultiplier : totalCost;
        }

		public static bool TryGetConfigCost(TechType techType, out int cost)
		{
            if (QMod.config.ModifiedItemCosts.TryGetValue(techType, out cost))
                return true;

            if (QMod.config.BaseMaterialCosts.TryGetValue(techType, out cost))
                return true;

            if (QMod.config.OrganicMaterialsCosts.TryGetValue(techType, out cost))
                return true;

			return false;
        }

		// Token: 0x06003465 RID: 13413 RVA: 0x00002319 File Offset: 0x00000519
		public void OnPointerEnter(string id)
		{
		}

		// Token: 0x06003466 RID: 13414 RVA: 0x00002319 File Offset: 0x00000519
		public void OnPointerExit(string id)
		{
		}

		// Token: 0x06003467 RID: 13415 RVA: 0x001203F8 File Offset: 0x0011E5F8
		public void OnPointerClick(string id, int button)
		{
			if (button != 0 && button != 2)
			{
				return;
			}
			TechType techType;
			if (!items.TryGetValue(id, out techType))
			{
				return;
			}


			foreach (var listener in onPointerClick)//for auto item converter, opens menu but doesn't actually give the item or take any ecm
			{
				if (listener(techType, out var onClick))
					continue;

				if (onClick != IconClickEffectsType.None)
				{
                    if (iconGrid.icons.TryGetValue(id, out uGUI_IconGrid.IconData asd))
                    {
                        float duration = 1f + UnityEngine.Random.Range(-0.2f, 0.2f);
                        asd.icon.PunchScale(5f, 0.5f, duration);
                    }

					if(onClick == IconClickEffectsType.ClickAllowed) 
						FMODUWE.PlayOneShot(uGUI.main.craftingMenu.soundAccept, MainCamera.camera.transform.position, 1f);
					else
                        FMODUWE.PlayOneShot(uGUI.main.craftingMenu.soundDeny, MainCamera.camera.transform.position, 1f);
                }

				return;
			}



			bool allowed = false;

            if (techType == QMod.FCSConvertBackType)
            {
                allowed = ExchangeCreditForECM();
            }
			else if (button == 0)
			{
				allowed = TryConvertItem(techType);
			}
			else if(EasyCraftPatches.CanAutoConvert())//maybe set to EasyCraftPatches.PlayerHasChip() instead, not sure.
            {
				allowed = TryConvertItemBack(techType);
			}


            if(allowed)
                FMODUWE.PlayOneShot(uGUI.main.craftingMenu.soundAccept, MainCamera.camera.transform.position, 1f);
			else
                FMODUWE.PlayOneShot(uGUI.main.craftingMenu.soundDeny, MainCamera.camera.transform.position, 1f);


            if (iconGrid.icons.TryGetValue(id, out uGUI_IconGrid.IconData iconData))
            {
				float duration = 1f + UnityEngine.Random.Range(-0.2f, 0.2f);
				iconData.icon.PunchScale(5f, 0.5f, duration);
            }
		}

		public bool TryConvertItemBack(TechType techType)
		{
			if (Inventory.main.GetPickupCount(techType) < CurrentConvertAmount) return false;

            float cost = GetCost(techType, 0, false, false);
			for(var I = 0; I < CurrentConvertAmount; I++)
			{
				if (Inventory.main.DestroyItem(techType)) QMod.SaveData.ECMAvailable += cost;
			}

            return true;
		}

		public bool TryConvertItem(TechType techType)
		{
            float cost = GetCost(techType) * CurrentConvertAmount;

            if (QMod.SaveData.ECMAvailable >= cost)
            {
                if (techType != QMod.FCSConvertType)
                {
                    for (int i = 0; i < CurrentConvertAmount; i++)
                    {
						CraftData.AddToInventory(techType);
                    }
                }
                else
                {
#if SN1
					ExternalModCompat.AddFCSCredit((decimal)cost * QMod.ECMToFCSCreditRate);
                    ErrorMessage.AddMessage($"Added {(decimal)cost * QMod.ECMToFCSCreditRate} alterra credit");
#endif
                }


                QMod.SaveData.ECMAvailable -= cost;
				return true;
            }
            else
            {
				return false;
            }
        }

		public bool ExchangeCreditForECM()
		{
#if !SN1
			return false;
#else
			int cost = QMod.ECMConvertPerClick * QMod.ECMToFCSCreditRate * CurrentConvertAmount;


			if(ExternalModCompat.GetFCSCredit() < cost)
				return false;


            ExternalModCompat.RemoveFCSCredit(cost);
            QMod.AddAmount(QMod.ECMConvertPerClick * CurrentConvertAmount);
            ErrorMessage.AddMessage($"Removed {cost} alterra credit");
            return true;
#endif
        }

		public void UpdateConvertAmount()
		{
			if(GameInput.GetButtonHeld(GameInput.Button.Sprint))
			{
				CurrentConvertAmount = 5;
				return;
			}
			else if(GameInput.GetButtonUp(GameInput.Button.Sprint))
			{
				CurrentConvertAmount = 1;
				return;
			}

			if (Input.GetKeyDown(QMod.config.convertIncrease))
				CurrentConvertAmount++;
			else if (Input.GetKeyDown(QMod.config.convertDecrease))
				CurrentConvertAmount--;//anyone else think this looks weird? variable++ looks normal, but not variable--

			if (iconGrid.GetCount() <= 60)
			{

				var direction = (int)Input.mouseScrollDelta.y;

				if (direction != -1 || CurrentConvertAmount != 1) //want to avoid it going negative
					CurrentConvertAmount += direction;
			}

			CurrentConvertAmount = Mathf.Clamp(CurrentConvertAmount, 1, 48);
        }

		public void OnSortRequested()
		{
			//DON'T FUCKING PUT ANYTHING HERE. LAST TIME I DID, IT BROKE THE ENTIRE MENU. 
			//QUARANTINE THIS SHIT
			//PRETEND IT HAS KHARAA
			//LEAVE
			//STOP READING




			//no lul
		}

		private void UpdateToolbarNotificationNumbers()
		{
			foreach (ExchangeMenuTab tab in Enum.GetValues(typeof(ExchangeMenuTab)))
			{
				toolbar.SetNotificationsAmount((int)tab, tabNotificationCounts[tab].Count);
			}
		}

		// Token: 0x0600346B RID: 13419 RVA: 0x001204EC File Offset: 0x0011E6EC
		private void ClearNotificationCounts()
		{
			foreach (ExchangeMenuTab tab in Enum.GetValues(typeof(ExchangeMenuTab)))
			{
				tabNotificationCounts[tab].Clear();
			}
		}


		// Token: 0x0600346F RID: 13423 RVA: 0x001205CC File Offset: 0x0011E7CC
		public static ExchangeMenu GetInstance()
		{
			if (_singleton == null)
			{
				if(!uGUI_BuilderMenu.singleton)
				{
					CoroutineHost.StartCoroutine(WaitForBuilderMenu());
					return null;
				}
				var origMenu = uGUI_BuilderMenu.GetInstance();

                var exchangeMenu = UWE.Utils.InstantiateDeactivated(origMenu.gameObject, origMenu.transform.position, origMenu.transform.rotation);
                GameObject.DestroyImmediate(exchangeMenu.GetComponent<uGUI_BuilderMenu>());
                exchangeMenu.AddComponent<ExchangeMenu>();
                exchangeMenu.SetActive(true);
            }
			return _singleton;
		}

		public static IEnumerator WaitForBuilderMenu()
		{
			yield return new WaitUntil(() => uGUI_BuilderMenu.singleton);
			GetInstance();
		}

		// Token: 0x06003470 RID: 13424 RVA: 0x00120630 File Offset: 0x0011E830
		private void SetState(bool newState)
		{
			if (state == newState)
			{
				return;
			}
			state = newState;
			if (state)
			{
				canvasScaler.SetAnchor();
				content.SetActive(true);
				if (!base.focused)
				{
					base.Select(false);
					return;
				}
			}
			else
			{
				if (base.focused)
				{
					base.Deselect(null);
				}
				content.SetActive(false);
				if (OnClose != null)
					OnClose();
			}
		}

		// Token: 0x06003471 RID: 13425 RVA: 0x0012069C File Offset: 0x0011E89C
		private void SetCurrentTab(int index)
		{
			if (index < 0 || index >= TabCount)
			{
				return;
			}
			if (index == (int)selected)
			{
				return;
			}
			toolbar.Select(index);
			selected = (ExchangeMenuTab)index;
			UpdateItems();
			iconGrid.UpdateNow();
			GamepadInputModule.current.SetCurrentGrid(iconGrid);
		}
		

		// Token: 0x06003472 RID: 13426 RVA: 0x001206FC File Offset: 0x0011E8FC
		private void UpdateItems()
		{
			title.text = "EXCHANGE";

			iconGrid.Clear();
			items.Clear();

			int iteration = 0;
			foreach (TechType type in QMod.SaveData.learntTechTypes)
			{
				if (GetTabTypeForTech(type) != selected)
					continue;

				items.Add(iteration.ToString(), type);
				iconGrid.AddItem(iteration.ToString(), SpriteManager.Get(type), SpriteManager.GetBackground(type), false, iteration);
				iconGrid.RegisterNotificationTarget(iteration.ToString(), notificationGroup, type.EncodeKey());
				if (tabNotificationCounts[selected].Contains(type))
					NotificationManager.main.Add(notificationGroup, type.EncodeKey());
				iteration++;
			}
            SetScrollBarActive(true);
            Invoke(nameof(UpdateScrollBar), 0.05f);//makes sure that the scrollrect moves up before getting disabled
													  //or it may cut off some items and not be able to be moved
        }

		public void UpdateScrollBar()
		{
            if (iconGrid.GetCount() > 60)
				SetScrollBarActive(true);
            else
				SetScrollBarActive(false);
        }
		public void SetScrollBarActive(bool active)
		{
            scrollBar.SetActive(active);
            scrollRect.enabled = active;
        }
		private ExchangeMenuTab GetTabTypeForTech(TechType type)
        {
			if (QMod.config.MovedItems.TryGetValue(type, out var movedTab))
				return movedTab;

			if (QMod.config.BaseMaterialCosts.ContainsKey(type))
				return ExchangeMenuTab.RawMaterials;

			if (QMod.config.OrganicMaterialsCosts.ContainsKey(type))
				return ExchangeMenuTab.BiologicalMaterials;

			if (EnumHandler.TryGetValue(type.ToString(), out TechType custom))
				return ExchangeMenuTab.ModdedItems;


#if SN
			if (CraftData.GetEquipmentType(type) != EquipmentType.None)
				return ExchangeMenuTab.Equipment;

			if (CraftData.Get(type, true) != null)
				return ExchangeMenuTab.CraftedItems;

#else
            if (TechData.GetEquipmentType(type) != EquipmentType.None)
                return ExchangeMenuTab.Equipment;

			var ingredients = TechData.GetIngredients(type);
            if (ingredients != null && ingredients.Count > 0)
                return ExchangeMenuTab.CraftedItems;
#endif

            return ExchangeMenuTab.Misc;
        }

		// Token: 0x06003473 RID: 13427 RVA: 0x001207B8 File Offset: 0x0011E9B8
		public bool OnButtonDown(GameInput.Button button)
		{
			if (button == GameInput.Button.UICancel)
			{
				Close();
				return true;
			}
			if (button == GameInput.Button.UINextTab)
			{
				int currentTab = (TabOpen + 1) % TabCount;
				SetCurrentTab(currentTab);
				return true;
			}
			if (button != GameInput.Button.UIPrevTab)
			{
				return false;
			}
			int currentTab2 = (TabOpen - 1 + TabCount) % TabCount;
			SetCurrentTab(currentTab2);
			return true;
		}

		public void OnAdd(NotificationManager.Group group, string key)
		{

		}

		public void OnRemove(NotificationManager.Group group, string key)
		{
			if(group != NotificationManager.Group.Undefined)
				return;

			TechType type = key.DecodeKey();

            foreach (ExchangeMenuTab tab in Enum.GetValues(typeof(ExchangeMenuTab)))
			{
				if (tabNotificationCounts[tab].Contains(type))
				{
					tabNotificationCounts[tab].Remove(type);
				}
			}
			UpdateToolbarNotificationNumbers();
		}

		void EventList<TechType>.IListListener.OnAdd(EventList<TechType> sender, TechType item)
		{
            tabNotificationCounts[GetTabTypeForTech(item)].Add(item);
        }

		void EventList<TechType>.IListListener.OnRemove(EventList<TechType> sender, TechType item)
		{
			var list = tabNotificationCounts[GetTabTypeForTech(item)];
			if(list.Contains(item))
				list.Remove(item);
        }

        void EventList<TechType>.IListListener.OnClear(EventList<TechType> sender, List<TechType> items)
		{
			foreach(var type in items)
			{
                var list = tabNotificationCounts[GetTabTypeForTech(type)];
                if (list.Contains(type))
                    list.Remove(type);
            }
		}

        // Token: 0x04002F3B RID: 12091
        private const string prefabPath = "ExchangeMenu";


		// Token: 0x04002F3D RID: 12093
		private const NotificationManager.Group notificationGroup = NotificationManager.Group.Undefined;

		// Token: 0x04002F3E RID: 12094
		public static ExchangeMenu singleton => GetInstance();

		private static ExchangeMenu _singleton;

		// Token: 0x04002F42 RID: 12098
		[AssertNotNull]
		public uGUI_CanvasScaler canvasScaler;

		// Token: 0x04002F43 RID: 12099
		[AssertNotNull]
#if SN1
		public Text title;
#else
		public TextMeshProUGUI title;
#endif

		// Token: 0x04002F44 RID: 12100
		[AssertNotNull]
		public uGUI_Toolbar toolbar;

		// Token: 0x04002F45 RID: 12101
		[AssertNotNull]
		public uGUI_IconGrid iconGrid;

		// Token: 0x04002F46 RID: 12102
		[AssertNotNull]
		public GameObject content;

		// Token: 0x04002F47 RID: 12103
		[Range(1f, 256f)]
		public float iconSize = 64f;

		// Token: 0x04002F48 RID: 12104
		[Range(0f, 64f)]
		public float minSpaceX = 20f;

		// Token: 0x04002F49 RID: 12105
		[Range(0f, 64f)]
		public float minSpaceY = 20f;

		// Token: 0x04002F4B RID: 12107
		private Dictionary<string, TechType> items = new Dictionary<string, TechType>();

		// Token: 0x04002F4C RID: 12108
		private int openInFrame = -1;

		// Token: 0x04002F4D RID: 12109
		private new ExchangeMenuTab selected;

		public enum ExchangeMenuTab
        {
			RawMaterials,
			BiologicalMaterials,
			CraftedItems,
			Equipment,
			Misc,
			ModdedItems
		}
		private Dictionary<ExchangeMenuTab, string> groupNames { get; } = new Dictionary<ExchangeMenuTab, string>()
		{
			{ ExchangeMenuTab.RawMaterials, "Raw Materials" },
			{ ExchangeMenuTab.BiologicalMaterials, "Biological Materials" },
			{ ExchangeMenuTab.CraftedItems, "Crafted Items" },
			{ ExchangeMenuTab.Equipment, "Equipment" },
			{ ExchangeMenuTab.Misc, "Misc" },
			{ ExchangeMenuTab.ModdedItems, "Modded Items" },
		};


		private Dictionary<ExchangeMenuTab, List<TechType>> tabNotificationCounts = new Dictionary<ExchangeMenuTab, List<TechType>>()
		{
			{ ExchangeMenuTab.RawMaterials, new List<TechType>() },
            { ExchangeMenuTab.BiologicalMaterials, new List<TechType>() },
            { ExchangeMenuTab.CraftedItems, new List<TechType>() },
            { ExchangeMenuTab.Equipment, new List<TechType>() },
			{ ExchangeMenuTab.Misc, new List<TechType>() },
            { ExchangeMenuTab.ModdedItems, new List<TechType>() }
        };
	}

}