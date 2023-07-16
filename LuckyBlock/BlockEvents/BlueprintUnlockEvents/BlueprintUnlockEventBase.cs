using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.BlueprintUnlockEvents
{
    internal class BlueprintUnlockEventBase : LuckyBlockEvent
    {
        public override string Message => "You unlocked a blueprint! Or at least you would have, if I had added this event properly. but it's just such a boring event I don't care enough. Now it just means that something has gone wrong, because you shouldn't be able to get this event";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return false;
        }

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {

        }
    }
}
