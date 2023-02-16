using SMLHelper.V2.Assets;
using SMLHelper.V2.Utility;
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
    internal class Amogus : Spawnable
    {
        public static readonly string assetsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static readonly AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(assetsPath, "amogus"));
        public static TechType TT { get; private set; }
        public Amogus() : base("Amogusus", "Amogus", "May or may not be an imposter")
        {
            OnFinishedPatching += () => TT = TechType;
        }
        private static GameObject prefab;
        public override GameObject GetGameObject()
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
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            gameObject.Set(GetGameObject());
            yield return null;
        }
        protected override Sprite GetItemSprite()
        {
            return new Atlas.Sprite(Amogus.bundle.LoadAsset<UnityEngine.Sprite>("AmogusIconRed"));
        }
        public override WorldEntityInfo EntityInfo => new WorldEntityInfo() { cellLevel = LargeWorldEntity.CellLevel.Near, classId = ClassID, slotType = EntitySlot.Type.Small, localScale = Vector3.one, techType = TechType};
        public override List<LootDistributionData.BiomeData> BiomesToSpawnIn => GetBiomes();
        public List<LootDistributionData.BiomeData> GetBiomes()
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
