using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EquivalentExchange.Monobehaviours;
using HarmonyLib;

namespace EquivalentExchange.Patches
{
    internal class EasyCraftPatches
    {
        public static bool CanAutoConvert()
        {
            return PlayerHasChip() || EasyConversionAntenna.AntennaInRange();
        }
        public static bool PlayerHasChip()
        {
            if (Inventory.main.equipment.equippedCount.TryGetValue(AutoItemConversionChip.techType, out int amount) && amount > 0) return true;
            return false;
        }

        public static void PatchEasyCraft(Harmony harmony)
        {
            var prefix = AccessTools.Method(typeof(EasyCraftPatches), nameof(GetPickupCountPrefix));

            var targetMethod = AccessTools.Method(typeof(EasyCraft.ClosestItemContainers), nameof(EasyCraft.ClosestItemContainers.GetPickupCount));

            harmony.Patch(targetMethod, new HarmonyMethod(prefix));

            var prefix2 = AccessTools.Method(typeof(EasyCraftPatches), nameof(DestroyItemPrefix));

            var targetMethod2 = AccessTools.Method(typeof(EasyCraft.ClosestItemContainers), nameof(EasyCraft.ClosestItemContainers.DestroyItem));

            harmony.Patch(targetMethod2, new HarmonyMethod(prefix2));
        }

        public static bool GetPickupCountPrefix(TechType techType, ref int __result)
        {
            if (!CanAutoConvert()) return true;

            if (!QMod.SaveData.learntTechTypes.Contains(techType)) return true;

            int amountCanConvert = (int)(QMod.SaveData.ECMAvailable / ExchangeMenu.GetCost(techType));

            if (amountCanConvert < 1) return true;

            __result = amountCanConvert;
            return false;
        }

        public static bool DestroyItemPrefix(TechType techType, ref bool __result, int count = 1)
        {
            if (!CanAutoConvert()) return true;

            if (!QMod.SaveData.learntTechTypes.Contains(techType)) return true;

            var cost = (ExchangeMenu.GetCost(techType)) * count;

            if (QMod.SaveData.ECMAvailable < cost) return true;


            QMod.SaveData.ECMAvailable -= cost;
            __result = true;
            return false;
        }
    }
}
