using SMLHelper.Assets;
using SMLHelper.Assets.Gadgets;
using SMLHelper.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;
#if SN
using Sprite = Atlas.Sprite;
#endif

namespace Snomod.Prefabs
{
    internal class Amogus
    {
        public static readonly string assetsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static readonly AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(assetsPath, "amogus"));


        internal static void Patch()
        {
            var sprite = bundle.LoadAsset<UnityEngine.Sprite>("AmogusIconRed");

            var prefab = new CustomPrefab("Amogusus", "Amogus", "May or may not be an imposter", sprite);

            prefab.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);

            prefab.SetGameObject(GetGameObject());

            prefab.SetSpawns
            (
                new WorldEntityInfo() 
                { 
                    cellLevel = LargeWorldEntity.CellLevel.Near, 
                    classId = prefab.Info.ClassID, 
                    slotType = EntitySlot.Type.Small, 
                    localScale = Vector3.one, 
                    techType = prefab.Info.TechType 
                }, 
                GetBiomes().ToArray()
            );

            prefab.Register();
            TT = prefab.Info.TechType;
        }
        public static TechType TT { get; private set; }
        private static GameObject prefab;
        public static GameObject GetGameObject()
        {
            if(!prefab)
            {
                prefab = bundle.LoadAsset<GameObject>("Amogus");
                prefab.SetActive(false);
                var crawl = prefab.GetComponent<CaveCrawler>();
                crawl.jumpSound = GetFmodAsset("event:/creature/crawler/jump");
                crawl.walkingSound.SetAsset(GetFmodAsset("event:/creature/crawler/idle"));
            }
            
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }

        public static List<LootDistributionData.BiomeData> GetBiomes()
        {
            var list = new List<LootDistributionData.BiomeData>();
            foreach(BiomeType biome in Enum.GetValues(typeof(BiomeType)))
            {
                var data = new LootDistributionData.BiomeData()
                {
                    biome = biome,
                    count = 2,
                    probability = 1f
                };

                list.Add(data);
            }
            return list;
        }
        public static FMODAsset GetFmodAsset(string audioPath)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = audioPath;
            return asset;
        }
    }
}
