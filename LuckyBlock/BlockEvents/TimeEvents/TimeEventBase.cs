using LuckyBlock.MonoBehaviours;
using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace LuckyBlock.BlockEvents.TimeEvents
{
    internal abstract class TimeEventBase : LuckyBlockEvent
    {
        private const float duration = 5;
        public override string Message => "Something weird is happening with time!";
        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
        }
        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            CoroutineHost.StartCoroutine(WaitForDuration());
        }
        private IEnumerator WaitForDuration()
        {
            yield return new WaitForSecondsRealtime(duration);
            Disable();
        }
        public abstract void Disable();
    }
}
