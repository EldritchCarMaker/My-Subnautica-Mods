using LuckyBlock.MonoBehaviours;
using UnityEngine;

namespace LuckyBlock.BlockEvents.Teleports
{
    internal abstract class TeleportEventBase : LuckyBlockEvent
    {
        public const float defaultTeleportChance = 0.5f;//teleports aren't any fun when they're too common.
        public abstract Vector3 position { get; }

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            Player.main.SetPosition(position);
        }
        public override float GetWeight(LuckyBlockMono luckyBlock) => defaultTeleportChance;
    }
}
