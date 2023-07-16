using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.SpawnEvents
{
    internal class SpawnMiscFish : SpawnEventBase
    {
        public override TechType[] TypesToSpawn => new[] 
        { 
            TechType.Peeper, 
            TechType.Bladderfish,
            TechType.Hoverfish,
            TechType.GarryFish,
            TechType.Spadefish,

        };

        public override Vector2int MinMaxToSpawn => new Vector2int(5, 32);

        public override string Message => "Look at all those chickens";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
        }
    }
}
