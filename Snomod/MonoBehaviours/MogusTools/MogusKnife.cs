using System.Collections;
using UnityEngine;
using UWE;

namespace Snomod.MonoBehaviours
{
    internal class MogusKnife : Knife
    {
        private const float constReach = 500;
        private const float constDamage = 2000;
        //private float velocity = 20;
        public RangedAttackLastTarget.RangedAttackType fireballAttack;
        public override void Awake()
        {
            base.Awake();
            CoroutineHost.StartCoroutine(LoadFireball());
        }
        public IEnumerator LoadFireball()
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.LavaLizard, false);
            yield return task;
            fireballAttack = task.GetResult().GetComponent<LavaLiazardRangedAttack>().attackTypes[0];
        }
        public override string animToolName => "knife";
        public void Update()
        {
            //mods occassionally change the damage and distance, because this is a knife. Will just ignore those lul
            base.damage = constDamage;
            base.attackDist = constReach;
        }
        public override void OnToolUseAnim(GUIHand hand)
        {
            Vector3 position = default(Vector3);
            GameObject gameObject = null;
            UWE.Utils.TraceFPSTargetPosition(Player.main.gameObject, attackDist, ref gameObject, ref position, true);
            if (!gameObject)
            {
                InteractionVolumeUser component = Player.main.gameObject.GetComponent<InteractionVolumeUser>();
                if (component != null && component.GetMostRecent() != null)
                {
                    gameObject = component.GetMostRecent().gameObject;
                }
                if(hand.GetActiveTarget() == null)
                {
                    if (Player.main.IsUnderwater())
                    {
                        Utils.PlayFMODAsset(underwaterMissSound, transform, 20f);
                        return;
                    }
                    Utils.PlayFMODAsset(surfaceMissSound, transform, 20f);
                }
                return;
            }

            LiveMixin liveMixin = gameObject.FindAncestor<LiveMixin>();
            if (Knife.IsValidTarget(liveMixin))
            {
                if (liveMixin)
                {
                    bool wasAlive = liveMixin.IsAlive();
                    liveMixin.TakeDamage(damage, position, damageType, null);
                    GiveResourceOnDamage(gameObject, liveMixin.IsAlive(), wasAlive);
                }
                Utils.PlayFMODAsset(attackSound, transform, 200f);
                SpawnFireball(position);
                VFXSurface component2 = gameObject.GetComponent<VFXSurface>();
                Vector3 euler = MainCameraControl.main.transform.eulerAngles + new Vector3(300f, 90f, 0f);
                VFXSurfaceTypeManager.main.Play(component2, vfxEventType, position, Quaternion.Euler(euler), Player.main.transform);
            }
            else
            {
                gameObject = null;
            }
        }
        public void SpawnFireball(Vector3 position)
        {
            GameObject gameObject = Utils.SpawnPrefabAt(fireballAttack.ammoPrefab, null, position);
            //gameObject.GetComponent<Rigidbody>().AddForce((1f + UnityEngine.Random.Range(-0.05f, 0.05f)) * velocity * directionToTarget, ForceMode.VelocityChange);
        }
    }
}
