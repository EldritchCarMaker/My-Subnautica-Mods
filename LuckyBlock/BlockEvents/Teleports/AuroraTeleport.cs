using LuckyBlock.MonoBehaviours;
using UnityEngine;

namespace LuckyBlock.BlockEvents.Teleports
{
    internal class AuroraTeleport : TeleportEventBase
    {
        public override Vector3 position => !CrashedShipExploder.main.IsExploded() ? new Vector3(1000, 2, 276) : new Vector3(1126, 2, 136);

        public override string Message => !CrashedShipExploder.main.IsExploded() ? "Better repair it before it explodes!" : LeakingRadiation.main.GetNumLeaks() > 0 ? "Hope you packed radiation resistance" : "We're just here to waste time";

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return luckyBlock.Depth < 200 && Vector3.SqrMagnitude(position - Player.main.transform.position) < 1000 * 1000;
        }

        public override float GetWeight(LuckyBlockMono luckyBlock)
        {
            if ((int)luckyBlock.Rarity > 2) return base.GetWeight(luckyBlock);

            return 0.25f;
        }
    }
}
