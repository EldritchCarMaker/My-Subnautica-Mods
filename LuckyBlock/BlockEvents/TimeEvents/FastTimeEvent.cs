using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckyBlock.BlockEvents.TimeEvents
{
    internal class FastTimeEvent : TimeEventBase
    {
        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            Time.timeScale = 3;
            base.TriggerEffect(luckyBlock);
        }
        public override void Disable()
        {
            Time.timeScale = 1;
        }
    }
}
