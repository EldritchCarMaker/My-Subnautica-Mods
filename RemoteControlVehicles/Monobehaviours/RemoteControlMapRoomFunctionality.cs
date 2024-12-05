using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RemoteControlVehicles.Monobehaviours
{
    internal class RemoteControlMapRoomFunctionality : MapRoomFunctionality
    {
        public new void Start()
        {
            base.Subscribe(true);
        }
		public static new void GetMapRoomsInRange(Vector3 position, float range, ICollection<MapRoomFunctionality> outlist)
		{
			float num = range * range;
			for (int i = 0; i < MapRoomFunctionality.mapRooms.Count; i++)
			{
				MapRoomFunctionality mapRoomFunctionality = MapRoomFunctionality.mapRooms[i];
				if ((mapRoomFunctionality.transform.position - position).sqrMagnitude <= num)
				{
					outlist.Add(mapRoomFunctionality);
				}
			}
		}
#if SN1
		// Token: 0x06000400 RID: 1024 RVA: 0x0001AE20 File Offset: 0x00019020
		public new void OnResourceDiscovered(ResourceTracker.ResourceInfo info)
		{
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x0001AE6C File Offset: 0x0001906C
		public new void OnResourceRemoved(ResourceTracker.ResourceInfo info)
		{
		}
#endif

		// Token: 0x06000402 RID: 1026 RVA: 0x0001AE89 File Offset: 0x00019089
		public new TechType GetActiveTechType()
		{
			return this.typeToScan;
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x0001AE94 File Offset: 0x00019094
		private new void OnPostRebuildGeometry(Base b)
		{
			Int3 @int = b.NormalizeCell(b.WorldToGrid(base.transform.position));
			Base.CellType cell = b.GetCell(@int);
			if (cell != Base.CellType.MapRoom && cell != Base.CellType.MapRoomRotated)
			{
				Debug.Log(string.Concat(new object[]
				{
				"map room had been destroyed, at cell ",
				@int,
				" new celltype is ",
				cell
				}));
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x0001AF07 File Offset: 0x00019107
		public new void ReloadMapWorld()
		{
			UnityEngine.Object.Destroy(this.mapWorld);
			base.StartCoroutine(this.LoadMapWorld());
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x0001AF21 File Offset: 0x00019121
		private new IEnumerator LoadMapWorld()
		{
			yield return null;
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x0001AF58 File Offset: 0x00019158
		private new void Update()
		{
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x0001B01C File Offset: 0x0001921C
		private new void UpdateModel()
		{
			int count = this.storageContainer.container.count;
			for (int i = 0; i < this.upgradeSlots.Length; i++)
			{
				this.upgradeSlots[i].SetActive(i < count);
			}
#if SN1
			this.modelUpdatePending = false;
#else
			this.containerIsDirty = false;
#endif
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x0001B0C4 File Offset: 0x000192C4
		private new void ObtainResourceNodes(TechType typeToScan)
		{
			
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x0001B174 File Offset: 0x00019374
		public new void StartScanning(TechType newTypeToScan)
		{
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x0001B244 File Offset: 0x00019444
		private new void UpdateBlips()
		{
			this.scanActive = false;
			if (this.scanActive)
			{
				Vector3 position = this.mapBlipRoot.transform.position;
				int num = Mathf.Min(this.numNodesScanned + 1, this.resourceNodes.Count);
				if (num != this.numNodesScanned)
				{
					this.numNodesScanned = num;
				}
				for (int i = 0; i < num; i++)
				{
					Vector3 vector = (this.resourceNodes[i].position - position) * this.mapScale;
					if (i >= this.mapBlips.Count)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.blipPrefab, vector, Quaternion.identity);
						gameObject.transform.SetParent(this.mapBlipRoot.transform, false);
						this.mapBlips.Add(gameObject);
					}
					this.mapBlips[i].transform.localPosition = vector;
					this.mapBlips[i].SetActive(true);
				}
				for (int j = num; j < this.mapBlips.Count; j++)
				{
					this.mapBlips[j].SetActive(false);
				}
			}
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x0001B368 File Offset: 0x00019568
		private new void UpdateCameraBlips()
		{
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x0001B49C File Offset: 0x0001969C
		private new void UpdateScanning()
		{
			
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x0001B5A4 File Offset: 0x000197A4
		private new void OnDestroy()
		{
		}

	}
}
