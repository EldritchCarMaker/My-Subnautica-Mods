using LuckyBlock.MonoBehaviours;
using Oculus.Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.InventoryEvents
{
    internal class DropInventoryEvent : LuckyBlockEvent
    {
        public override string Message => "Whoops, butter fingers";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
            //temporary until I can get good way to iterate through equipment
        }

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            var list = new List<InventoryItem>(Inventory.main.container);
            //list.AddRange((IEnumerable<InventoryItem>)((IItemsContainer)Inventory.main.equipment).GetEnumerator());

            foreach (var invItem in list)
            {
                Inventory.main.InternalDropItem(invItem.item, false);
            }
        }
    }
}
