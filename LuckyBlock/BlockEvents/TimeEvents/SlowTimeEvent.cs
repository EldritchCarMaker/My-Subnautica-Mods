using LuckyBlock.MonoBehaviours;
using UnityEngine;

namespace LuckyBlock.BlockEvents.TimeEvents
{
    internal class SlowTimeEvent : TimeEventBase
    {
        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            Time.timeScale = 0.3f;//easier than doing .333333333...
            base.TriggerEffect(luckyBlock);
        }
        public override void Disable()
        {
            Time.timeScale = 1;
        }
    }
}
