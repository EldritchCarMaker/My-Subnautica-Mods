using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace CyclopsTorpedoes.Patches
{
    [HarmonyPatch(typeof(CyclopsExternalCams))]
    internal class CyclopsExternalCamsPatches
    {
        public static TorpedoType[] torpedoTypes;


        [HarmonyPatch(nameof(CyclopsExternalCams.HandleInput))]
        public static void Postfix(CyclopsExternalCams __instance)
        {
            if(Input.GetKeyDown(QMod.config.torpedoKey))
            {
                if(torpedoTypes == null)
                {
                    torpedoTypes = CraftData.GetPrefabForTechType(TechType.Seamoth).GetComponent<SeaMoth>().torpedoTypes;
                }

                TorpedoType torpedoType = GetPriorityType();

                if(GameModeUtils.RequiresIngredients())
                {
                    torpedoType = ConsumeAvailableTorpedo(__instance.GetComponentInParent<SubRoot>().GetComponentInChildren<CyclopsDecoyLoadingTube>(), torpedoType.techType);
                }

                var muzzle = Camera.main.transform;

                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(torpedoType.prefab);
                gameObject.GetComponent<Transform>();
                Bullet component = gameObject.GetComponent<SeamothTorpedo>();
                Vector3 zero = Vector3.zero;
                Transform aimingTransform = Camera.main.transform;
                Rigidbody componentInParent = muzzle.GetComponentInParent<Rigidbody>();
                Vector3 rhs = (componentInParent != null) ? componentInParent.velocity : Vector3.zero;
                float speed = Vector3.Dot(aimingTransform.forward, rhs);
                component.Shoot(muzzle.position, aimingTransform.rotation, speed, -1f);
            }
        }
        public static TorpedoType ConsumeAvailableTorpedo(CyclopsDecoyLoadingTube tube, TechType priorityTorpType)
        {
            foreach(KeyValuePair<string, InventoryItem> pair in tube.decoySlots.equipment)
            {
                if(pair.Value.item.GetTechType() == priorityTorpType)
                {
                    tube.decoySlots.RemoveItem(pair.Key, true, false);
                    return torpedoTypes.First(c => c.techType == pair.Value.item.GetTechType());
                }
            }
            foreach(TorpedoType torpType in torpedoTypes)
            {
                foreach(KeyValuePair<string, InventoryItem> pair in tube.decoySlots.equipment)
                {
                    if (pair.Value.item.GetTechType() == torpType.techType)
                    {
                        return torpType;
                    }
                }
            }
            return null;
        }

        public static TorpedoType GetPriorityType()
        {
            TechType torpedoPriorityTechType = TechType.GasTorpedo;
            int lowest = 999;
            foreach (KeyValuePair<TechType, int> pair in QMod.config.torpedoTypePriority)
            {
                if (pair.Value < lowest)
                {
                    lowest = pair.Value;
                    torpedoPriorityTechType = pair.Key;
                }
            }
            foreach (TorpedoType type in torpedoTypes)
            {
                if (type.techType == torpedoPriorityTechType) return type;
            }
            return torpedoTypes[0];
        }
    }
}
