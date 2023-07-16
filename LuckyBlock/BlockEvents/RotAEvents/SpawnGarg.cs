using LuckyBlock.MonoBehaviours;
using LuckyBlock.BlockEvents.SpawnEvents;
using SMLHelper.V2.Handlers;

namespace LuckyBlock.BlockEvents.RotAEvents
{
    internal class SpawnGarg : SpawnEventBase, IBlockEventDependency
    {
        public override TechType[] TypesToSpawn 
        { 
            get 
            { 
                //TechTypeHandler.TryGetModdedTechType("GargantuanVoid", out var voidType);
                return new[] { RotA.Mod.gargJuvenilePrefab.TechType, RotA.Mod.gargBabyPrefab.TechType }; 
            } 
        }

        public override Vector2int MinMaxToSpawn => new(1, 1);

        public override string Message => "Hey wait that's not from this mod";

        public override float GetWeight(LuckyBlockMono luckyBlock) => 5;

        public string[] Dependencies => new[] { "ProjectAncients" };

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return luckyBlock.Rarity == BlockType.Precursor;
        }
    }
}
