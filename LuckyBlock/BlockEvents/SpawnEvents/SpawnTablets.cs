using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.SpawnEvents
{
    internal class SpawnTablets : SpawnEventBase
    {
        public override TechType[] TypesToSpawn => new[] { TechType.PrecursorKey_Blue, TechType.PrecursorKey_Orange, TechType.PrecursorKey_Purple, TechType.PrecursorKey_Red, TechType.PrecursorKey_White };

        public override Vector2int MinMaxToSpawn => new(1, 3);

        public override string Message => "Hope you have a use for them";

        public override float GetWeight(LuckyBlockMono luckyBlock) => 10;

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return luckyBlock.Rarity == BlockType.Precursor;
        }
    }
}
