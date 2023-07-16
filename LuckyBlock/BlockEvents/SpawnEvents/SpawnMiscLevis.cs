using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.SpawnEvents
{
    internal class SpawnMiscLevis : SpawnEventBase
    {
        public override TechType[] TypesToSpawn
        {
            get
            {
                var list = new List<TechType>() { TechType.GhostLeviathanJuvenile, TechType.ReaperLeviathan, TechType.SeaDragon, TechType.SeaEmperorJuvenile };
                if (SMLHelper.V2.Handlers.TechTypeHandler.TryGetModdedTechType("gulperleviathan", out var type))
                    list.Add(type);
                return list.ToArray();
            }
        }

        public override float GetWeight(LuckyBlockMono luckyBlock) => 1 + ((int)luckyBlock.Rarity / 4);

        public override Vector2int MinMaxToSpawn => new(1, 4);

        public override string Message => "Many leviathans, and they're all named levi";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
        }
    }
}
