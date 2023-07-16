using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.SpawnEvents
{
    internal class SpawnBonesharks : SpawnEventBase
    {
        public override TechType[] TypesToSpawn => new[] { TechType.BoneShark };

        public override string[] PathsToSpawn => null;

        public override Vector2int MinMaxToSpawn => new Vector2int(4, 10);

        public override string Message => "Boneshark attack!";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return luckyBlock.Depth < 100;
        }
    }
}
