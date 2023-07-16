using LuckyBlock.MonoBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckyBlock.BlockEvents.SpawnEvents
{
    internal class RecursiveLuckyBlock : SpawnEventBase
    {
        public override TechType[] TypesToSpawn => null;

        public override string[] PathsToSpawn => null;

        public override Vector2int MinMaxToSpawn => new Vector2int(3, 5);

        public override string Message => "Woah, recursion...";

        public override float GetWeight(LuckyBlockMono luckyBlock) => 1 + ((int)luckyBlock.Rarity / 5);

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
        }

        public override GameObject GetGameObject()
        {
            return LuckyBlockSpawner.blockPrefab;
        }
    }
}
