using SMLHelper.V2.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace EquivalentExchange.Monobehaviours
{
    public class ExchangeMenu : uGUI_InputGroup, uGUI_IIconGridManager, uGUI_IToolbarManager, uGUI_IButtonReceiver, INotificationListener, EventList<TechType>.IListListener
    {
		public const int PRICEOFUNMARKEDITEM = 50;
		//if you're a modder trying to change this value for your item, please use the ExternalModCompat class


		public bool state { get; private set; }

		public int TabOpen
		{
			get
			{
				return (int)selected;
			}
		}

		public int TabCount
		{
			get
			{
				return Enum.GetValues(typeof(ExchangeMenuTab)).Length;
			}
		}
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
			if (ExchangeMenu.singleton != null)
			{
				Debug.LogError("Multiple ExchangeMenu instances found in scene!", this);
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			ExchangeMenu.singleton = this;
			base.Awake();
			name = "ExchangeMenu";

			canvasScaler = GetComponent<uGUI_CanvasScaler>();
			title = transform.Find("Content").Find("Title").GetComponent<UnityEngine.UI.Text>();
			toolbar = transform.Find("Content").Find("Toolbar").GetComponent<uGUI_Toolbar>();
			iconGrid = transform.Find("Content").Find("ScrollView").Find("Viewport").Find("ScrollCanvas").GetComponent<uGUI_IconGrid>();
			content = transform.Find("Content").gameObject;

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
			Atlas.Sprite[] array = new Atlas.Sprite[count];
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
        }

		private Atlas.Sprite GetSpriteForTabType(ExchangeMenuTab tab)
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
			if(!QModManager.API.QModServices.Main.ModPresent("FCSAlterraHub"))
			{
				return;
			}

            QMod.TryUnlockTechType(QMod.FCSConvertType);
            QMod.TryUnlockTechType(QMod.FCSConvertBackType);

			var FCSScreen = ExternalModCompat.SetFCSConvertIcons();
			if(FCSScreen == null || !(FCSScreen is GameObject gameObject))
			{
				return;
			}
            var iconTransform = gameObject.transform.Find("Information/ToggleHud/AccountBalanceIcon");
            if (iconTransform == null) return;
			QMod.FCSCreditIconSprite = iconTransform.GetComponent<Image>().sprite;

			SMLHelper.V2.Handler.SpriteHandler.RegisterSprite(QMod.FCSConvertType, QMod.FCSCreditIconSprite);
            SMLHelper.V2.Handler.SpriteHandler.RegisterSprite(QMod.FCSConvertBackType, QMod.FCSCreditIconSprite);
        }

		// Token: 0x06003454 RID: 13396 RVA: 0x001201B3 File Offset: 0x0011E3B3
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
			ExchangeMenu.singleton = null;
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

		public void GetToolbarTooltip(int index, out string tooltipText, List<TooltipIcon> tooltipIcons)
		{
			tooltipText = null;
			if (index < 0 || index >= TabCount)
			{
				return;
			}
			tooltipText = groupNames[(ExchangeMenuTab)index];
		}

		// Token: 0x06003463 RID: 13411 RVA: 0x001203B7 File Offset: 0x0011E5B7
		public void OnToolbarClick(int index, int button)
		{
			if (button == 0)
			{
				SetCurrentTab(index);
			}
		}

		// Token: 0x06003464 RID: 13412 RVA: 0x001203C4 File Offset: 0x0011E5C4
		public void GetTooltip(string id, out string tooltipText, List<TooltipIcon> tooltipIcons)
		{
			TechType techType;
			if (items.TryGetValue(id, out techType))
			{
				//TooltipFactory.BuildTech(techType, locked, out tooltipText, tooltipIcons);
				WriteDetails(techType, out tooltipText);
				return;
			}
			tooltipText = null;
		}

		public void WriteDetails(TechType techType, out string tooltipText)
        {
			TooltipFactory.Initialize();
			StringBuilder stringBuilder = new StringBuilder();
			string key = techType.AsString(false);

			TooltipFactory.WriteTitle(stringBuilder, Language.main.Get(key) + (GameInput.GetButtonHeld(GameInput.Button.Sprint) ? " (5)" : ""));
			TooltipFactory.WriteDescription(stringBuilder, Language.main.Get(TooltipFactory.techTypeTooltipStrings.Get(techType)));

			if (techType != QMod.FCSConvertBackType)
				WriteCost(techType, stringBuilder, GameInput.GetButtonHeld(GameInput.Button.Sprint));
			else
				WriteFCSCost(techType, stringBuilder, GameInput.GetButtonHeld(GameInput.Button.Sprint));

            tooltipText = stringBuilder.ToString();
		}
		public void WriteCost(TechType techType, StringBuilder stringBuilder, bool isShiftDown = false)
        {
			stringBuilder.Append(Environment.NewLine);

			float current = QMod.SaveData.EMCAvailable;
			float amount = isShiftDown ? GetCost(techType) * 5 : GetCost(techType);
			bool flag = current >= amount || !GameModeUtils.RequiresIngredients();

			if (flag)
			{
				stringBuilder.Append("<color=#94DE00FF>");
			}
			else
			{
				stringBuilder.Append("<color=#DF4026FF>");
			}

			stringBuilder.Append("EMC:");

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
		public void WriteFCSCost(TechType techType, StringBuilder stringBuilder, bool isShiftDown = false)
		{
            stringBuilder.Append(Environment.NewLine);

			float current = (float)ExternalModCompat.GetFCSCredit();
            float amount = isShiftDown ? QMod.EMCConvertPerClick * QMod.EMCToFCSCreditRate * 5 : QMod.EMCConvertPerClick * QMod.EMCToFCSCreditRate;
			bool flag = !GameModeUtils.RequiresIngredients() || current >= amount;

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

        public static float GetCost(TechType techType, int depth = 0, bool useCreative = true, bool useEfficiency = true)
        {
			if (useCreative && !GameModeUtils.RequiresIngredients())
				return 0;

			if(QMod.config.ModifiedItemCosts.TryGetValue(techType, out var modifiedCost))
				return modifiedCost;

			if (QMod.config.BaseMaterialCosts.TryGetValue(techType, out var costs))
				return costs;

			if (QMod.config.OrganicMaterialsCosts.TryGetValue(techType, out var organicCosts))
				return organicCosts;

			if (techType == QMod.FCSConvertType)
				return QMod.EMCConvertPerClick;

			if (depth > 10) return 5;

			float totalCost = 0;

			var techData = CraftData.Get(techType, true);

			if (techData == null)
            {
				return PRICEOFUNMARKEDITEM;
            }


			for (var i = 0; i < techData.ingredientCount; i++)
			{
				var ingredient = techData.GetIngredient(i);
				if (ingredient != null)
				{
					for (var j = 0; j < ingredient.amount; j++)
						totalCost += GetCost(ingredient.techType, depth+1, useCreative, useEfficiency); 

				}
			}
            return useEfficiency ? totalCost * QMod.config.inefficiencyMultiplier : totalCost;
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
			if (button != 0)
			{
				return;
			}
			TechType techType;
			if (!items.TryGetValue(id, out techType))
			{
				return;
			}

			foreach (var listener in onPointerClick)
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

            if (techType == QMod.FCSConvertBackType)
            {
                ExchangeCreditForEMC(id);
                return;
            }
            bool sprintDown = GameInput.GetButtonHeld(GameInput.Button.Sprint);
            float cost = sprintDown ? GetCost(techType) * 5 : GetCost(techType);

			if(QMod.SaveData.EMCAvailable >= cost)
            {
                if (techType != QMod.FCSConvertType)
                {
                    int itemsToSpawn = sprintDown ? 5 : 1;
                    for (int i = 0; i < itemsToSpawn; i++)
                    {
                        GameObject gameObject = CraftData.InstantiateFromPrefab(techType, false);
                        if (gameObject != null)
                        {
                            gameObject.transform.position = MainCamera.camera.transform.position + MainCamera.camera.transform.forward * 3f;
                            CrafterLogic.NotifyCraftEnd(gameObject, techType);
                            Pickupable component = gameObject.GetComponent<Pickupable>();
                            if (component != null && !Inventory.main.Pickup(component, false))
                            {
                                ErrorMessage.AddError(Language.main.Get("InventoryFull"));
                            }
                        }
                    }
                }
				else
				{
					ExternalModCompat.AddFCSCredit((decimal)cost * QMod.EMCToFCSCreditRate);
					ErrorMessage.AddMessage($"Added {(decimal)cost * QMod.EMCToFCSCreditRate} alterra credit");
				}


				QMod.SaveData.EMCAvailable -= cost;
				FMODUWE.PlayOneShot(uGUI.main.craftingMenu.soundAccept, MainCamera.camera.transform.position, 1f);
			}
            else
            {
				FMODUWE.PlayOneShot(uGUI.main.craftingMenu.soundDeny, MainCamera.camera.transform.position, 1f);
			}
			if(iconGrid.icons.TryGetValue(id, out uGUI_IconGrid.IconData iconData))
            {
				float duration = 1f + UnityEngine.Random.Range(-0.2f, 0.2f);
				iconData.icon.PunchScale(5f, 0.5f, duration);
            }
		}
		public void ExchangeCreditForEMC(string id)
		{
			bool sprintDown = GameInput.GetButtonHeld(GameInput.Button.Sprint);

			int cost = sprintDown ? QMod.EMCConvertPerClick * QMod.EMCToFCSCreditRate * 5 : QMod.EMCConvertPerClick * QMod.EMCToFCSCreditRate;
			if(ExternalModCompat.GetFCSCredit() >= cost)
			{
				ExternalModCompat.RemoveFCSCredit(cost);
				QMod.AddAmount(sprintDown ? QMod.EMCConvertPerClick * 5 : QMod.EMCConvertPerClick);
                ErrorMessage.AddMessage($"Removed {cost} alterra credit");
                FMODUWE.PlayOneShot(uGUI.main.craftingMenu.soundAccept, MainCamera.camera.transform.position, 1f);
            }
			else
			{
                FMODUWE.PlayOneShot(uGUI.main.craftingMenu.soundDeny, MainCamera.camera.transform.position, 1f);
			}

            if (iconGrid.icons.TryGetValue(id, out uGUI_IconGrid.IconData iconData))
            {
                float duration = 1f + UnityEngine.Random.Range(-0.2f, 0.2f);
                iconData.icon.PunchScale(5f, 0.5f, duration);
            }
        }
		// Token: 0x06003468 RID: 13416 RVA: 0x00002319 File Offset: 0x00000519
		public void OnSortRequested()
		{
			UpdateItems();
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
			if (ExchangeMenu.singleton == null)
			{
				QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Error, "Tried to access Exchange Menu before being set up");
				return null;
			}
			return ExchangeMenu.singleton;
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
		}
		private ExchangeMenuTab GetTabTypeForTech(TechType type)
        {
			if (QMod.config.MovedItems.TryGetValue(type, out var movedTab))
				return movedTab;

			if (QMod.config.BaseMaterialCosts.ContainsKey(type))
				return ExchangeMenuTab.RawMaterials;

			if (QMod.config.OrganicMaterialsCosts.ContainsKey(type))
				return ExchangeMenuTab.BiologicalMaterials;

			if (TechTypeHandler.TryGetModdedTechType(type.ToString(), out TechType custom))
				return ExchangeMenuTab.ModdedItems;

			if (CraftData.GetEquipmentType(type) != EquipmentType.None)
				return ExchangeMenuTab.Equipment;

			if (CraftData.Get(type, true) != null)
				return ExchangeMenuTab.CraftedItems;

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
		public static ExchangeMenu singleton;

		// Token: 0x04002F42 RID: 12098
		[AssertNotNull]
		public uGUI_CanvasScaler canvasScaler;

		// Token: 0x04002F43 RID: 12099
		[AssertNotNull]
		public Text title;

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
		private Dictionary<ExchangeMenuTab, string> groupNames = new Dictionary<ExchangeMenuTab, string>()
        {
			{ ExchangeMenuTab.RawMaterials, TooltipFactory.Label("Raw Materials") },
			{ ExchangeMenuTab.BiologicalMaterials, TooltipFactory.Label("Biological Materials") },
			{ ExchangeMenuTab.CraftedItems, TooltipFactory.Label("Crafted Items") },
			{ ExchangeMenuTab.Equipment, TooltipFactory.Label("Equipment") },
			{ ExchangeMenuTab.Misc, TooltipFactory.Label("Misc") },
			{ ExchangeMenuTab.ModdedItems, TooltipFactory.Label("Modded Items") },
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