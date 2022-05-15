using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilityStuffs
{
    public static class Utility
    {
        public static bool EquipmentHasItem(TechType techtype, EquipmentType equipmentType = EquipmentType.Chip, Equipment equipment = null)
        {
            if (equipment == null) equipment = Inventory.main != null ? Inventory.main.equipment : null;

            if(equipment == null) return false;

            List<string> slots = new List<string>();
            equipment.GetSlots(equipmentType, slots);
            Equipment e = equipment;

            foreach (string slot in slots)
            {
                TechType tt = e.GetTechTypeInSlot(slot);
                if (tt == techtype)
                {
                    return true;
                }
            }
            return false;
        }
        public static FMODAsset GetFmodAsset(string audioPath)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = audioPath;
            return asset;
        }
    }
}
