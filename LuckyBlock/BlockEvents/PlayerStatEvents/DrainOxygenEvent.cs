using LuckyBlock.MonoBehaviours;
using UnityEngine;

namespace LuckyBlock.BlockEvents.PlayerStatEvents
{
    internal class DrainOxygenEvent : LuckyBlockEvent
    {
        public override string Message => "You're breathtaking";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return GameModeUtils.RequiresOxygen();
        }

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            Player.main.oxygenMgr.RemoveOxygen(Random.Range(10, 20));
        }
    }
}
