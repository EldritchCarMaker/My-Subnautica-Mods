using LuckyBlock.BlockEvents;
using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckyBlock
{
    internal class GetLow : LuckyBlockEvent
    {
        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
        }

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            Player.main.rigidBody.AddForce(Vector3.up * -50, ForceMode.VelocityChange);
        }

        public override float GetWeight(LuckyBlockMono luckyBlock) => 1;

        public override string Message => "Get low";

    }
}
