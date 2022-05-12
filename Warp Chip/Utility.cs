using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WarpChip
{
    public static class Utility
    {
        public static bool EquipmentHasItem(TechType techtype, Equipment equipment = null)
        {
            if (equipment == null) equipment = Inventory.main.equipment;

            List<string> chipSlots = new List<string>();
            equipment.GetSlots(EquipmentType.Chip, chipSlots);
            Equipment e = equipment;

            foreach (string slot in chipSlots)
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
