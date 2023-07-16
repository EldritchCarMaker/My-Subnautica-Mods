using LuckyBlock.MonoBehaviours;
using UnityEngine;

namespace LuckyBlock.BlockEvents.InventoryEvents
{
    internal class GiveBatteriesEvent : LuckyBlockEvent
    {
        private enum BatteryType
        {
            Battery,
            Powercell,
            IonBattery,
            IonPowercell
        }
        public override string Message => "Some extra batteries";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
        }

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            var amount = Random.Range(1, 3);
            for (int i = 0; i < amount; i++)
                CraftData.AddToInventory(GetBatteryType(), 1, false);
        }

        public TechType GetBatteryType()
        {
            var bat = (BatteryType)LuckyBlockRarity.Weighted(4) - 1;
            switch(bat)//I'm sure I COULD make a less hardcoded one but... this is faster 
            {
                case BatteryType.Battery: return TechType.Battery;
                case BatteryType.Powercell: return TechType.PowerCell;
                case BatteryType.IonBattery: return TechType.PrecursorIonBattery;
                case BatteryType.IonPowercell: return TechType.PrecursorIonPowerCell;
            }
            return TechType.Battery;
        }
    }
}
