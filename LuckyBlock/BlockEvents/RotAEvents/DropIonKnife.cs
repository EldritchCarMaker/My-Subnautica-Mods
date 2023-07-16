using LuckyBlock.BlockEvents.SpawnEvents;
using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.RotAEvents
{
    internal class DropIonKnife : SpawnEventBase, IBlockEventDependency
    {
        public override TechType[] TypesToSpawn => new[] { RotA.Mod.ionKnife.TechType };

        public override Vector2int MinMaxToSpawn => new(1, 1);

        public override string Message => "Woah, looks so fancy";

        public string[] Dependencies => new[] { "ProjectAncients" };

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return luckyBlock.Rarity == BlockType.Precursor;
        }
    }
}
