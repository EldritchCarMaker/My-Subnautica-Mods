using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.SpawnEvents
{
    internal class SpawnOre : SpawnEventBase
    {
        public override TechType[] TypesToSpawn => new[] { TechType.Copper, TechType.Titanium, TechType.Gold, TechType.Silver, TechType.Lead, TechType.Diamond, TechType.AluminumOxide, TechType.Lithium, TechType.Nickel, TechType.Kyanite };

        public override string[] PathsToSpawn => null;

        public override float GetWeight(LuckyBlockMono luckyBlock) => 1 + ((int)luckyBlock.Rarity / 4);

        public override Vector2int MinMaxToSpawn => new Vector2int(3, 6);

        public override string Message => "Wooh, free items!";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
        }
    }
}
