using System;
using System.Collections.Generic;

namespace RALIV.Subnautica.AquariumBreeding
{
	// Token: 0x02000002 RID: 2
	public class AquariumInfo
	{
		// Token: 0x06000002 RID: 2 RVA: 0x0000205C File Offset: 0x0000025C
		internal static void Update(Aquarium aquarium)
		{
			if (aquarium.storageContainer.IsEmpty())
			{
				AquariumInfo._aquariumInfo.Remove(aquarium);
				return;
			}
			AquariumInfo aquariumInfo;
			AquariumInfo._aquariumInfo.TryGetValue(aquarium, out aquariumInfo);
			if (aquariumInfo == null)
			{
				AquariumInfo._aquariumInfo.Add(aquarium, aquariumInfo = new AquariumInfo(aquarium));
			}
			aquariumInfo.Update();
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020B0 File Offset: 0x000002B0
		internal static AquariumInfo Get(Aquarium aquarium)
		{
			AquariumInfo result;
			AquariumInfo._aquariumInfo.TryGetValue(aquarium, out result);
			return result;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020CC File Offset: 0x000002CC
		private AquariumInfo(Aquarium aquarium)
		{
			this._aquarium = aquarium;
			this.BreedInfo = new List<AquariumInfo.AquariumBreedTime>();
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020E8 File Offset: 0x000002E8
		private void Update()
		{
			double timePassed = DayNightCycle.main.timePassed;
			double num = timePassed % 1200.0;
			double num2 = timePassed - num;
			List<AquariumInfo.AquariumBreedTime> list = new List<AquariumInfo.AquariumBreedTime>();
			ItemsContainer container = this._aquarium.storageContainer.container;
			foreach (TechType techType in container.GetItemTypes())
			{
				double num3 = 10 % 12 / 24.0 * 1200.0;
				double num4;
				for (num4 = num2 + num3; num4 < timePassed; num4 += 600.0)
				{
				}
				IList<InventoryItem> items = container.GetItems(techType);
				AquariumInfo.AquariumBreedTime item = new AquariumInfo.AquariumBreedTime
				{
					FishType = techType,
					BreedTime = num4,
					BreedCount = items.Count / 2
				};
				list.Add(item);
			}
			this.BreedInfo = list;
		}

		// Token: 0x04000001 RID: 1
		private static Dictionary<Aquarium, AquariumInfo> _aquariumInfo = new Dictionary<Aquarium, AquariumInfo>();

		// Token: 0x04000002 RID: 2
		private Aquarium _aquarium;

		// Token: 0x04000003 RID: 3
		public List<AquariumInfo.AquariumBreedTime> BreedInfo;

		// Token: 0x02000003 RID: 3
		public struct AquariumBreedTime
		{
			// Token: 0x04000004 RID: 4
			public TechType FishType;

			// Token: 0x04000005 RID: 5
			public double BreedTime;

			// Token: 0x04000006 RID: 6
			internal int BreedCount;
		}
	}
}
