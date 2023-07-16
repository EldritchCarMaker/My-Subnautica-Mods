using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents
{
    internal class RefillStatEvent : LuckyBlockEvent
    {
        public override string Message => "your stats have been refilled!";

        public override float GetWeight(LuckyBlockMono luckyBlock) => 1 + ((int)luckyBlock.Rarity / 4);

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
        }

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            Player.main.oxygenMgr.AddOxygen(999999);
            var survival = Player.main.GetComponent<Survival>();
            survival.food = 100;
            survival.water = 100;
            survival.liveMixin.AddHealth(9999);
        }
    }
}
