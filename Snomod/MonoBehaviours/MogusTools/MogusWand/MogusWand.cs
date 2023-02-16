using Snomod.Prefabs;
using System.Collections;
using UnityEngine;
using UWE;

namespace Snomod.MonoBehaviours
{
    internal class MogusWand : PlayerTool
    { 
        public override string animToolName => "knife";//makes the game use knife animations for this tool
        private static float range = 150;
        private static float damage = 150;
        private static float bulletSpeed = 15;
        private static float timeNextBullet = 0;
        private static float bulletCooldown = 0.05f;
        [SerializeField]
        private GameObject bulletPrefab;
        [SerializeField]
        private Transform barrelTransform;
        public override void OnToolUseAnim(GUIHand hand)
        {
            GameObject gameObject = null;
            Vector3 position = default;
            UWE.Utils.TraceFPSTargetPosition(Player.main.gameObject, range, ref gameObject, ref position, true);

            if (!gameObject) return;

            var liveMixin = gameObject.FindAncestor<LiveMixin>();
            if (!liveMixin || !Knife.IsValidTarget(liveMixin)) return;

            liveMixin.TakeDamage(damage, liveMixin.transform.position, DamageType.Starve, null);

            if (!liveMixin.IsAlive()) CoroutineHost.StartCoroutine(BecomeMogus(liveMixin.gameObject));
        }

        public static IEnumerator BecomeMogus(GameObject objectToMogify)
        {
            var pos = objectToMogify.transform.position;
            pos.y += 1f;
            var prefabRequest = CraftData.GetPrefabForTechTypeAsync(Amogus.TT);
            yield return prefabRequest;
            var prefab = prefabRequest.GetResult();

            var copy = Instantiate(prefab);
            copy.transform.position = pos;
            copy.SetActive(true);
            Destroy(objectToMogify);
        }

        public override bool OnLeftHandHeld()
        {
            if (Time.time < timeNextBullet) return false;

            var bullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
            bullet.gameObject.SetActive(true);
            bullet.Shoot(barrelTransform.position, MainCamera.camera.transform.rotation, bulletSpeed, 50);
            timeNextBullet = Time.time + bulletCooldown;
            return true;
        }
    }
}
