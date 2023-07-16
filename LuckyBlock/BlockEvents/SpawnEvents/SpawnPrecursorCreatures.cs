using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.SpawnEvents
{
    internal class SpawnPrecursorCreatures : SpawnEventBase
    {
        public override TechType[] TypesToSpawn => new[] { TechType.PrecursorDroid, TechType.Warper };

        public override Vector2int MinMaxToSpawn => new(1, 3);

        public override float GetWeight(LuckyBlockMono luckyBlock) => 10;

        public override string Message => "Precursor creatures in a precursor cube, who'da thunk it";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return luckyBlock.Rarity == BlockType.Precursor;
        }
    }
}
