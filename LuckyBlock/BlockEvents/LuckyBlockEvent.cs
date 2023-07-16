using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents
{
    public abstract class LuckyBlockEvent
    {
        public string GetName() => ToString().Split(".".ToCharArray()).Last();

        public abstract void TriggerEffect(LuckyBlockMono luckyBlock);
        public abstract bool CanTrigger(LuckyBlockMono luckyBlock);
        public abstract string Message { get; }
        public virtual float GetWeight(LuckyBlockMono luckyBlock) => 1;
    }
}
