using LuckyBlock.BlockEvents.SpawnEvents;
using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.RotAEvents
{
    internal class SpawnRotaResources : SpawnEventBase, IBlockEventDependency
    {
        public override TechType[] TypesToSpawn => new[] { ArchitectsLibrary.Handlers.AUHandler.CobaltTechType, ArchitectsLibrary.Handlers.AUHandler.EmeraldTechType, ArchitectsLibrary.Handlers.AUHandler.MorganiteTechType };

        public override Vector2int MinMaxToSpawn => new(2, 4);

        public override string Message => "RotA resources!";

        public string[] Dependencies => new[] { "ProjectAncients" };

        public override float GetWeight(LuckyBlockMono luckyBlock) => 10;

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return luckyBlock.Rarity == BlockType.Precursor;
        }
    }
}
