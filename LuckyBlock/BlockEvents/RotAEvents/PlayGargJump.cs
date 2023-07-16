using LuckyBlock.MonoBehaviours;
using LuckyBlock.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace LuckyBlock.BlockEvents.RotAEvents
{
    internal class PlayGargJump : LuckyBlockEvent, IBlockEventDependency
    {
        public override string Message => "Something huge is stirring... get to the surface and watch it!";

        public string[] Dependencies => new[] { "ProjectAncients" };

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return luckyBlock.Rarity == BlockType.Precursor && Vector3.SqrMagnitude(luckyBlock.Position - new Vector3(369, -2, 1163)) < (300 * 300);
            //position is just a position kinda nearby the gun island
            //just where I happened to be in game at the time
        }

        public override float GetWeight(LuckyBlockMono luckyBlock) => 10;

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            var tempObj = new GameObject("TempGargAnimObject");

            tempObj.AddComponent<SunbeamGargMarker>();
            var gargController = tempObj.AddComponent<RotA.Mono.Cinematics.SunbeamGargController>();
            //gargController._wreck = tempObj.AddComponent<RotA.Mono.VFX.SunbeamWreck>();
            //this was to prevent the null reference exception in EndCinematic but... it just made the garg invisible for some reason.

            gargController._defaultFarplane = gargController.CurrentFarplaneDistance;
            gargController._farplaneTarget = 20000;
            gargController._initialized = true;

            gargController.Invoke(nameof(gargController.SpawnGarg), 10);
            gargController.Invoke(nameof(gargController.SpawnBreachSplash), 17.7f);
            gargController.Invoke(nameof(gargController.SpawnDiveSplash), 20.6f);
            gargController.Invoke(nameof(gargController.StartFadingOut), 80);
            gargController.Invoke(nameof(gargController.EndCinematic), 115f);
        }
    }
}
