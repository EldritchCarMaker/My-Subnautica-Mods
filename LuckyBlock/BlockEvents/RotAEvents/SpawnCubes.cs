using LuckyBlock.BlockEvents.SpawnEvents;
using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.RotAEvents
{
    internal class SpawnCubes : SpawnEventBase, IBlockEventDependency
    {
        public string[] Dependencies => new[] { "ProjectAncients" };

        public override TechType[] TypesToSpawn => new[] { TechType.PrecursorIonCrystal, ArchitectsLibrary.Handlers.AUHandler.ElectricubeTechType, ArchitectsLibrary.Handlers.AUHandler.RedIonCubeTechType };

        public override Vector2int MinMaxToSpawn => new(1, 3);

        public override float GetWeight(LuckyBlockMono luckyBlock) => 10;

        public override string Message => "Oooh fancy";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return luckyBlock.Rarity == BlockType.Precursor;
        }
    }
}
