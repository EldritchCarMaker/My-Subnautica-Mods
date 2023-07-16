using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckyBlock.BlockEvents.SpawnEvents
{
    internal class SpawnReaper : SpawnEventBase
    {
        public override TechType[] TypesToSpawn => new[] { TechType.ReaperLeviathan };

        public override string[] PathsToSpawn => null;

        public override Vector2int MinMaxToSpawn => new Vector2int(1, 2);

        public override string Message => "Watch out behind you!";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
        }
        public override Vector3 GetSpawnPosition(LuckyBlockMono luckyBlock)
        {
            return MainCamera.camera.transform.position + (-25 * MainCamera.camera.transform.forward);
        }
    }
}
