using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Snomod.MonoBehaviours;
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

            prefab.SetGameObject(GetGameObject);

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
        public static GameObject GetGameObject()
        {
            var prefab = bundle.LoadAsset<GameObject>("Amogus");
            prefab.SetActive(true);

            prefab.AddComponent<MogusSounds>();//Have to add the component here rather than in the editor, because I lost the unity project that had it and would have to make it over again.

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
    }
}
