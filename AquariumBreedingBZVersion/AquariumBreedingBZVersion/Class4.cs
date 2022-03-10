using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using CyclopsNuclearReactor;

namespace RALIV.Subnautica.AquariumBreeding
{
    [HarmonyPatch(typeof(CyclopsNuclearReactor.QPatch.))
    public class ImBored
    {
        public static void Bored2()
        {
            
        }
    }
    // Token: 0x02000006 RID: 6
    /*[HarmonyPatch(typeof(Aquarium))]
    [HarmonyPatch("LateUpdate")]
    public class Aquarium_Update_Patch
    {
        // Token: 0x0600000C RID: 12 RVA: 0x00002218 File Offset: 0x00000418
        [HarmonyPostfix]
        public static void Postfix(Aquarium __instance)
        {
            double timePassed = DayNightCycle.main.timePassed;
            AquariumInfo aquariumInfo = AquariumInfo.Get(__instance);
            if (aquariumInfo == null)
            {
                return;
            }
            if (!__instance.storageContainer.container.HasRoomFor(1, 1))
            {
                return;
            }
            List<AquariumInfo.AquariumBreedTime> breedInfo = aquariumInfo.BreedInfo;
            bool flag = false;
            for (int i = 0; i < breedInfo.Count; i++)
            {
                AquariumInfo.AquariumBreedTime aquariumBreedTime = breedInfo[i];
                if (!flag && aquariumBreedTime.BreedTime <= timePassed)
                {
                    flag = Aquarium_Update_Patch.Breed(__instance, aquariumBreedTime.FishType, aquariumBreedTime.BreedCount);
                }
                aquariumBreedTime.BreedTime += 600.0;
            }
        }

        // Token: 0x0600000D RID: 13 RVA: 0x000022B0 File Offset: 0x000004B0
        private static bool Breed(Aquarium aquarium, TechType fishType, int breedCount)
        {
            Vector2int itemSize = TechData.GetItemSize(fishType);
            ItemsContainer container = aquarium.storageContainer.container;
            for (int i = 0; i < breedCount; i++)
            {
                if (!container.HasRoomFor(itemSize.x, itemSize.y))
                {
                    return false;
                }
                GameObject gameObject1;
                GetGameObjectAsync(out<GameObject> gameObject1);
                Pickupable pickupable = prefab.GetComponent<Pickupable>();
                aquarium.storageContainer.container.AddItem(pickupable);
            }
            return true;
        }
        public static IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            GameObject prefab = null;
            if (prefab == null)
            {
                CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.ColdSuitGloves, verbose: true);
                yield return task;

                prefab = GameObject.Instantiate(task.GetResult());
            }

            GameObject go = GameObject.Instantiate(prefab);
            gameObject.Set(go);
        }
    }*/
}
