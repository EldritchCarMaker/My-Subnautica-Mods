using LuckyBlock.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents.TimeEvents
{
    internal class StopTimeEvent : TimeEventBase
    {
        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            if (!StasisRifle.sphere) CraftData.GetPrefabForTechType(TechType.StasisRifle).GetComponent<StasisRifle>().Awake();
            StasisRifle.sphere.transform.position = Player.main.transform.position;
            StasisRifle.sphere.radius = 1000;
            StasisRifle.sphere.time = 500;
            StasisRifle.sphere.EnableField();
            base.TriggerEffect(luckyBlock);
        }

        public override void Disable()
        {
            StasisRifle.sphere.CancelAll();
        }

        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return false;
        }
    }
}
