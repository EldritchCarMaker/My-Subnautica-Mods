using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckyBlock.BlockEvents.SpawnEvents
{
    internal class SpawnVehicle : SpawnEventBase
    {
        public override TechType[] TypesToSpawn => new[] { TechType.Seamoth, TechType.Exosuit }
                                                   .Where(type => !GameInfoIcon.Has(type)) //only give a vehicle once, and only if the player doesn't already have one
                                                   .ToArray();

        public override string[] PathsToSpawn => null;

        public override Vector2int MinMaxToSpawn => new Vector2int(1, 1);

        public override float GetWeight(LuckyBlockMono luckyBlock) => 1 + ((int)luckyBlock.Rarity / 5);

        public override string Message => "Lucky you, you just found a vehicle!";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return TypesToSpawn.Length > 0;
        }

        public override GameObject GetGameObject()
        {
            var obj = base.GetGameObject();

            foreach (var target in obj.GetComponentsInChildren<ICraftTarget>())
                target.OnCraftEnd(TechType.Accumulator);//the techtype in the method call shouldn't matter

            return obj;
        }
    }
}
