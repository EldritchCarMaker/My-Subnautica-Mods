using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckyBlock.BlockEvents.Teleports
{
    internal class DelayedTeleport : TeleportEventBase, IDelayedEffect
    {
        private const float maxTeleportDistance = 300;
        public override Vector3 position => EscapePod.main.transform.position + new Vector3(0, 6, 0);

        string IDelayedEffect.Message => "Nothing Seems to have happened...";

        public float Delay => 5;

        public override string Message => "Got em";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return luckyBlock.Depth < 100 && Vector3.SqrMagnitude(luckyBlock.Position - EscapePod.main.transform.position) < maxTeleportDistance * maxTeleportDistance;
        }
    }
}
