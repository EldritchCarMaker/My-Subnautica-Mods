using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedsFromHarvesting.Patches
{
    [HarmonyPatch(typeof(Planter))]
    internal class PlanterPatches
    {
        [HarmonyPatch(nameof(Planter.IsAllowedToAdd))]
        public static void Postfix(Planter __instance, Pickupable pickupable, ref bool __result)
        {
            if(QMod.Config.AllowGrownPlants) return;


            Plantable plantable = pickupable.GetComponent<Plantable>();
            if (!plantable)
            {
                return;
            }
            if(!plantable.isSeedling)
            {
                TechType techType = pickupable.GetTechType();

#if SN
                TechType harvestOutputData = CraftData.GetHarvestOutputData(techType);
#else
                TechType harvestOutputData = TechData.GetHarvestOutput(techType);
#endif
                if (harvestOutputData != TechType.None)
                {
                    ErrorMessage.AddMessage("Can't plant fully grown plants!");
                    __result = false;
                }
            }
        }
    }
}
