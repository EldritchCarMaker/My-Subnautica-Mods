using LuckyBlock.MonoBehaviours;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LuckyBlock.BlockEvents.BlueprintUnlockEvents
{
    internal class CyclopsBlueprintEvent : LuckyBlockEvent
    {
        private static List<TechType> cyclopsBlueprints = new()
        {
            TechType.CyclopsBridgeBlueprint,
            TechType.CyclopsEngineBlueprint,
            TechType.CyclopsHullBlueprint,
        };

        public override string Message => "You unlocked part of the cyclops!";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return !KnownTech.Contains(TechType.Cyclops) && !cyclopsBlueprints.Any(type => KnownTech.Contains(type));
        }

        public override float GetWeight(LuckyBlockMono luckyBlock) => 1 + ((int)luckyBlock.Rarity / 4);

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            TechType typeToLearn = TechType.None;
            while(typeToLearn == TechType.None || KnownTech.Contains(typeToLearn))
            {
                typeToLearn = cyclopsBlueprints[Random.Range(0, cyclopsBlueprints.Count)];
            }
            KnownTech.Add(typeToLearn);
        }
    }
}
