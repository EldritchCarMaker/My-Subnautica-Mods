using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.InventoryEvents
{
    internal class ClearInventoryEvent : LuckyBlockEvent
    {
        public override string Message => "Whoops, whered your items go?";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
        }

        public override float GetWeight(LuckyBlockMono luckyBlock) => 0.01f;

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            Inventory.main.container.Clear();
        }
    }
}
