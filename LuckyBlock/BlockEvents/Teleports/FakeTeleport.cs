using LuckyBlock.MonoBehaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;

namespace LuckyBlock.BlockEvents.Teleports
{
    internal class FakeTeleport : TeleportEventBase
    {
        private static List<Vector3> positions = new List<Vector3>()
        {
            new Vector3(0, 300, 0),
            new Vector3(450, -100, 10),

        };
        public override Vector3 position => positions[Random.Range(0, positions.Count)];

        public override string Message => "Teleported To Random Location";

        public override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            var prevPos = Player.main.transform.position;
            base.TriggerEffect(luckyBlock);
            CoroutineHost.StartCoroutine(TeleportBack(prevPos, 5));
        }
        public IEnumerator TeleportBack(Vector3 pos, float time)
        {
            yield return new WaitForSeconds(time);
            Player.main.SetPosition(pos);
            ErrorMessage.AddMessage("Ha fake teleport");
        }
        public override bool CanTrigger(LuckyBlockMono luckyBlock)
        {
            return true;
        }
    }
}
