using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckyBlock.BlockEvents
{
    internal class KillNearbyLevis : LuckyBlockEvent
    {
        private const float range = 200;

        public override string Message => "Nearby leviathans have been killed. Their blood is on your hands";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return GameObject.FindObjectsOfType<EcoTarget>().Any(target => target.type == EcoTargetType.Leviathan && Vector3.SqrMagnitude(target.transform.position - luckyBlock.Position) < range * range);
        }

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            GameObject.FindObjectsOfType<EcoTarget>()
                .Where(target => target.type == EcoTargetType.Leviathan && Vector3.SqrMagnitude(target.transform.position - luckyBlock.Position) < range * range)
                .ForEach(target => { if (target.TryGetComponent(out LiveMixin live)) live.Kill(); });
        }
    }
}
