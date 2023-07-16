using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents
{
    internal class BonesharkRain : LuckyBlockEvent
    {
        public override string Message => "It's raining bonesharks!";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return luckyBlock.Depth < 150;
        }

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            Player.main.gameObject.AddComponent<BoneSharkRainer>();
        }
    }
}
