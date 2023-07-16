using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.InventoryEvents
{
    internal class RefillBatteriesEvent : LuckyBlockEvent
    {
        public override string Message => "Your batteries have been refilled!";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            //here until I find a better way to iterate through equipment cause damn this is annoying me. This is an invalid cast but I don't know how to do it properly
            //the game does it, I should be able to do it, but I can't.
            return 
                   //((IEnumerable<InventoryItem>)Inventory.main.equipment).Any(item => item.item.GetComponentInChildren<IBattery>() != null) || //look for batteries in hotbar
                   Inventory.main.container.Any(item => item.item.GetComponentInChildren<IBattery>() != null);//look for batteries in inventory
        }

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            var list = new List<InventoryItem>(Inventory.main.container);
            //list.AddRange((IEnumerable<InventoryItem>)((IItemsContainer)Inventory.main.equipment).GetEnumerator());

            foreach (var invItem in list)
            {
                foreach (var bat in invItem.item.GetComponentsInChildren<IBattery>())
                    bat.charge = bat.capacity;
            }
        }

        public override float GetWeight(LuckyBlockMono luckyBlock) => 1 + ((int)luckyBlock.Rarity / 4);
    }
}
