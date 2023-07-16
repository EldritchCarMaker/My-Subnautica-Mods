using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.InventoryEvents
{
    internal class FillInventoryEvent : LuckyBlockEvent
    {
        public override string Message => ":)";//looks weird here, but looks normal in game

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
        }

        public override float GetWeight(LuckyBlockMono luckyBlock) => 0.5f;

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            CraftData.AddToInventory(TechType.SeaTreaderPoop, 100, true, true);
        }
    }
}
