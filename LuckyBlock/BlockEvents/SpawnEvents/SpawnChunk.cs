using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.SpawnEvents
{
    internal class SpawnChunk : SpawnEventBase
    {
        public override TechType[] TypesToSpawn => new[] { TechType.LimestoneChunk, TechType.ShaleChunk, TechType.SandstoneChunk };

        public override string[] PathsToSpawn => null;

        public override Vector2int MinMaxToSpawn => new Vector2int(5, 10);

        public override string Message => "Get used to clicking";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
        }

        public override float GetWeight(LuckyBlockMono luckyBlock) => 1 + ((int)luckyBlock.Rarity / 4);
    }
}
