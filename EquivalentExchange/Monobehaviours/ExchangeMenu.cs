using SMLHelper.V2.Handlers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EquivalentExchange.Monobehaviours
{
    public class ExchangeMenu : uGUI_InputGroup, uGUI_IIconGridManager, uGUI_IToolbarManager, uGUI_IButtonReceiver, INotificationListener
	{
		public bool state { get; private set; }

		public int TabOpen
		{
			get
			{
				return selected;
			}
		}

		public int TabCount
		{
			get
			{
				return ExchangeMenu.groups.Count;
			}
		}

		void INotificationListener.OnAdd(NotificationManager.Group group, string key)
		{
			TechType techType = key.DecodeKey();
			if (KnownTech.Contains(techType))
			{
				int techTypeTechGroupIdx = GetTechTypeTechGroupIdx(techType);
				groupNotificationCounts[techTypeTechGroupIdx]++;
			}
		}

		// Token: 0x0600344F RID: 13391 RVA: 0x0011FFD0 File Offset: 0x0011E1D0
		void INotificationListener.OnRemove(NotificationManager.Group group, string key)
		{
			TechType techType = key.DecodeKey();
			if (KnownTech.Contains(techType))
			{
				int techTypeTechGroupIdx = GetTechTypeTechGroupIdx(techType);
				groupNotificationCounts[techTypeTechGroupIdx]--;
			}
		}

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


			ExchangeMenu.EnsureTechGroupTechTypeDataInitialized();
			ClearNotificationCounts();
			iconGrid.iconSize = new Vector2(iconSize, iconSize);
			iconGrid.minSpaceX = minSpaceX;
			iconGrid.minSpaceY = minSpaceY;
			iconGrid.Initialize(this);
			int count = ExchangeMenu.groups.Count;
			Atlas.Sprite[] array = new Atlas.Sprite[count];
			for (int i = 0; i < count; i++)
			{
				TechGroup value = ExchangeMenu.groups[i];
				string str = techGroupNames.Get(value);
				array[i] = SpriteManager.Get(SpriteManager.Group.Tab, "group" + str);
			}
			object[] array2 = array;
			toolbar.Initialize(this, array2, null, 15);
			toolbar.Select(selected);
			UpdateItems();
			KnownTech.onChanged += OnChanged;
			PDAScanner.onAdd += OnLockedAdd;
			PDAScanner.onRemove += OnLockedRemove;
			NotificationManager.main.Subscribe(this, NotificationManager.Group.Builder, string.Empty);
		}

		// Token: 0x06003452 RID: 13394 RVA: 0x0012018C File Offset: 0x0011E38C
		private List<TechType> GetTechTypesForGroup(int groupIdx)
		{
			return ExchangeMenu.groupsTechTypes[groupIdx];
		}

		// Token: 0x06003453 RID: 13395 RVA: 0x00120195 File Offset: 0x0011E395
		private void Start()
		{
			Language.main.OnLanguageChanged += OnLanguageChanged;
			OnLanguageChanged();
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
			if(Input.GetKeyDown(KeyCode.K))
            {
				Open();
            }
		}

		// Token: 0x06003456 RID: 13398 RVA: 0x00120232 File Offset: 0x0011E432
		private void LateUpdate()
		{
			if (state)
			{
				//UpdateToolbarNotificationNumbers();
			}
		}

		// Token: 0x06003457 RID: 13399 RVA: 0x00120244 File Offset: 0x0011E444
		private void OnDestroy()
		{
			NotificationManager.main.Unsubscribe(this);
			KnownTech.onChanged -= OnChanged;
			PDAScanner.onAdd -= OnLockedAdd;
			PDAScanner.onRemove -= OnLockedRemove;
			Language main = Language.main;
			if (main)
			{
				main.OnLanguageChanged -= OnLanguageChanged;
			}
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

		// Token: 0x0600345F RID: 13407 RVA: 0x00120389 File Offset: 0x0011E589
		private void OnChanged(HashSet<TechType> techList)
		{
			UpdateItems();
		}

		// Token: 0x06003460 RID: 13408 RVA: 0x00120389 File Offset: 0x0011E589
		private void OnLockedAdd(PDAScanner.Entry entry)
		{
			UpdateItems();
		}

		// Token: 0x06003461 RID: 13409 RVA: 0x00120389 File Offset: 0x0011E589
		private void OnLockedRemove(PDAScanner.Entry entry)
		{
			UpdateItems();
		}

		// Token: 0x06003462 RID: 13410 RVA: 0x00120391 File Offset: 0x0011E591
		public void GetToolbarTooltip(int index, out string tooltipText, List<TooltipIcon> tooltipIcons)
		{
			tooltipText = null;
			if (index < 0 || index >= toolbarTooltips.Count)
			{
				return;
			}
			tooltipText = toolbarTooltips[index];
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
				bool locked = !CrafterLogic.IsCraftRecipeUnlocked(techType);
				TooltipFactory.BuildTech(techType, locked, out tooltipText, tooltipIcons);
				return;
			}
			tooltipText = null;
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
			if (!KnownTech.Contains(techType))
			{
				return;
			}
			GameObject buildPrefab = CraftData.GetBuildPrefab(techType);
			SetState(false);
			Builder.Begin(buildPrefab);
		}

		// Token: 0x06003468 RID: 13416 RVA: 0x00002319 File Offset: 0x00000519
		public void OnSortRequested()
		{
		}

		// Token: 0x06003469 RID: 13417 RVA: 0x00120438 File Offset: 0x0011E638
		private void UpdateToolbarNotificationNumbers()
		{
			for (int i = 0; i < ExchangeMenu.groups.Count; i++)
			{
				toolbar.SetNotificationsAmount(i, groupNotificationCounts[i]);
			}
		}

		// Token: 0x0600346A RID: 13418 RVA: 0x00120470 File Offset: 0x0011E670
		private static void EnsureTechGroupTechTypeDataInitialized()
		{
			if (!ExchangeMenu.groupsTechTypesInitialized)
			{
				for (int i = 0; i < ExchangeMenu.groups.Count; i++)
				{
					ExchangeMenu.groupsTechTypes[i] = new List<TechType>();
					List<TechType> list = ExchangeMenu.groupsTechTypes[i];
					CraftData.GetBuilderGroupTech(ExchangeMenu.groups[i], list, false);
					for (int j = 0; j < list.Count; j++)
					{
						TechType key = list[j];
						ExchangeMenu.techTypeToTechGroupIdx.Add(key, i);
					}
				}
				ExchangeMenu.groupsTechTypesInitialized = true;
			}
		}

		// Token: 0x0600346B RID: 13419 RVA: 0x001204EC File Offset: 0x0011E6EC
		private void ClearNotificationCounts()
		{
			NotificationManager main = NotificationManager.main;
			for (int i = 0; i < ExchangeMenu.groups.Count; i++)
			{
				groupNotificationCounts[i] = 0;
			}
		}

		// Token: 0x0600346C RID: 13420 RVA: 0x00120520 File Offset: 0x0011E720
		private int GetTechTypeTechGroupIdx(TechType inTechType)
		{
			int result;
			if (ExchangeMenu.techTypeToTechGroupIdx.TryGetValue(inTechType, out result))
			{
				return result;
			}
			throw new ArgumentException("TechType not associated with any of the tech groups.");
		}

		// Token: 0x0600346D RID: 13421 RVA: 0x00120548 File Offset: 0x0011E748
		private void CacheToolbarTooltips()
		{
			toolbarTooltips.Clear();
			for (int i = 0; i < ExchangeMenu.groups.Count; i++)
			{
				TechGroup value = ExchangeMenu.groups[i];
				toolbarTooltips.Add(TooltipFactory.Label(string.Format("Group{0}", techGroupNames.Get(value))));
			}
		}

		// Token: 0x0600346E RID: 13422 RVA: 0x001205A7 File Offset: 0x0011E7A7
		private void OnLanguageChanged()
		{
			title.text = Language.main.Get("CraftingLabel");
			CacheToolbarTooltips();
		}

		// Token: 0x0600346F RID: 13423 RVA: 0x001205CC File Offset: 0x0011E7CC
		private static ExchangeMenu GetInstance()
		{
			if (ExchangeMenu.singleton == null)
			{
				GameObject gameObject = Resources.Load<GameObject>("ExchangeMenu");
				if (gameObject == null)
				{
					Debug.LogError("Cannot find main ExchangeMenu prefab in Resources folder at path 'ExchangeMenu'");
					Debug.Break();
					return null;
				}
				UnityEngine.Object.Instantiate<GameObject>(gameObject);
				ExchangeMenu.singleton.state = true;
				ExchangeMenu.singleton.SetState(false);
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
			}
		}

		// Token: 0x06003471 RID: 13425 RVA: 0x0012069C File Offset: 0x0011E89C
		private void SetCurrentTab(int index)
		{
			if (index < 0 || index >= ExchangeMenu.groups.Count)
			{
				return;
			}
			if (index == selected)
			{
				return;
			}
			toolbar.Select(index);
			selected = index;
			UpdateItems();
			iconGrid.UpdateNow();
			GamepadInputModule.current.SetCurrentGrid(iconGrid);
		}
		

		// Token: 0x06003472 RID: 13426 RVA: 0x001206FC File Offset: 0x0011E8FC
		private void UpdateItems()
		{
			iconGrid.Clear();
			items.Clear();

			int iteration = 0;
			foreach (TechType type in QMod.SaveData.learntTechTypes)
			{
				items.Add(iteration.ToString(), type);
				iconGrid.AddItem(iteration.ToString(), SpriteManager.Get(type), SpriteManager.GetBackground(type), false, iteration);
				iconGrid.RegisterNotificationTarget(iteration.ToString(), NotificationManager.Group.Builder, type.EncodeKey());
			}
			/*
			foreach(string str in QMod.SaveData.learntTechTypes)
            {
				TechType tt = GetTechType(str);
            }*/

			return;

			TechGroup techGroup = ExchangeMenu.groups[selected];
			List<TechType> techTypesForGroup = GetTechTypesForGroup(selected);
			int num = 0;
			for (int i = 0; i < techTypesForGroup.Count; i++)
			{
				TechType techType = techTypesForGroup[i];
				TechUnlockState techUnlockState = KnownTech.GetTechUnlockState(techType);
				if (techUnlockState == TechUnlockState.Available || techUnlockState == TechUnlockState.Locked)
				{
					string stringForInt = IntStringCache.GetStringForInt(num);
					items.Add(stringForInt, techType);
					iconGrid.AddItem(stringForInt, SpriteManager.Get(techType), SpriteManager.GetBackground(techType), techUnlockState == TechUnlockState.Locked, num);
					iconGrid.RegisterNotificationTarget(stringForInt, NotificationManager.Group.Builder, techType.EncodeKey());
					num++;
				}
			}
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

		// Token: 0x04002F3B RID: 12091
		private const string prefabPath = "ExchangeMenu";

		// Token: 0x04002F3C RID: 12092
		private static readonly List<TechGroup> groups = new List<TechGroup>
	{
		TechGroup.BasePieces,
		TechGroup.ExteriorModules,
		TechGroup.InteriorPieces,
		TechGroup.InteriorModules,
		TechGroup.Miscellaneous
	};

		// Token: 0x04002F3D RID: 12093
		private const NotificationManager.Group notificationGroup = NotificationManager.Group.Builder;

		// Token: 0x04002F3E RID: 12094
		private static ExchangeMenu singleton;

		// Token: 0x04002F3F RID: 12095
		private static readonly List<TechType>[] groupsTechTypes = new List<TechType>[ExchangeMenu.groups.Count];

		// Token: 0x04002F40 RID: 12096
		private static Dictionary<TechType, int> techTypeToTechGroupIdx = new Dictionary<TechType, int>();

		// Token: 0x04002F41 RID: 12097
		private static bool groupsTechTypesInitialized = false;

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
		private new int selected;

		// Token: 0x04002F4E RID: 12110
		private CachedEnumString<TechGroup> techGroupNames = new CachedEnumString<TechGroup>(CraftData.sTechGroupComparer);

		// Token: 0x04002F4F RID: 12111
		private List<string> toolbarTooltips = new List<string>();

		// Token: 0x04002F50 RID: 12112
		private int[] groupNotificationCounts = new int[ExchangeMenu.groups.Count];
	}

}