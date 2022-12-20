using System.Collections;
using UnityEngine;
using UWE;

namespace Shared
{
    public class ECMCapsule : MonoBehaviour, IHandTarget
    {
        public GameObject content;

        public GameObject inspectPrefab;

        public FMODAsset useSound;

        public string animParam;

        public string viewAnimParam;

        public float viewAnimDuration = 2f;

        public string id;

        private GameObject inspectObject;
        public static void MakeCapsule(Vector3 position, string id)
        {
            CoroutineHost.StartCoroutine(MakeCapsuleAsync(position, id));
        }
        private static IEnumerator MakeCapsuleAsync(Vector3 position, string id)
        {
            yield return new WaitUntil(() => Player.main);//wait until player is loaded

            var task = CraftData.GetPrefabForTechTypeAsync(TechType.TimeCapsule);
            yield return task;
            var capsuleObj = GameObject.Instantiate(task.GetResult());

            var capsule = capsuleObj.GetComponent<TimeCapsule>();
            var ECMCap = capsuleObj.EnsureComponent<ECMCapsule>();

            ECMCap.content = capsule.content;
            ECMCap.inspectObject = capsule.inspectObject;
            ECMCap.inspectPrefab = capsule.inspectPrefab;
            ECMCap.useSound = capsule.useSound;
            ECMCap.animParam = capsule.animParam;
            ECMCap.viewAnimParam = capsule.viewAnimParam;
            ECMCap.viewAnimDuration = capsule.viewAnimDuration;

            ECMCap.id = id;

            capsuleObj.transform.position = position;
            Destroy(capsule);
        }

        public void Start()
        {
            content.SetActive(true);
        }

        public void OnHandHover(GUIHand hand)
        {
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
            HandReticle.main.SetInteractText("TimeCapsuleOpen");
        }
        public void OnHandClick(GUIHand hand)
        {
            StartCoroutine(Open());
            content.SetActive(false);
        }

        private IEnumerator Open()
        {
            bool hasViewAnim = !string.IsNullOrEmpty(viewAnimParam);
            float seconds = 0f;
            if (hasViewAnim)
            {
                seconds = Player.main.armsController.StartHolsterTime(viewAnimDuration);
            }
            yield return new WaitForSeconds(seconds);
            if (hasViewAnim)
            {
                ArmsController armsController = Player.main.armsController;
                armsController.TriggerAnimParam(viewAnimParam, viewAnimDuration);
                if (inspectPrefab != null)
                {
                    inspectObject = UnityEngine.Object.Instantiate<GameObject>(inspectPrefab);
                    Transform transform = inspectObject.transform;
                    transform.SetParent(armsController.leftHandAttach, false);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    if (!string.IsNullOrEmpty(animParam))
                    {
                        Animator component = inspectObject.GetComponent<Animator>();
                        if (component != null)
                        {
                            component.SetTrigger(animParam);
                        }
                    }
                    if (useSound != null)
                    {
                        Utils.PlayFMODAsset(useSound, transform, 20f);
                    }
                }
                yield return new WaitForSeconds(viewAnimDuration);
                if (inspectObject != null)
                {
                    UnityEngine.Object.Destroy(inspectObject);
                }
            }
            Collect();
            yield break;
        }
        private void Collect()
        {
            UnlockPDAEntry();

            var item = new TimeCapsuleItem() { techType = TechType.CrabsquidEgg };
            Inventory.main.ForcePickup(item.Spawn());

            if (!QModManager.API.QModServices.Main.ModPresent("ECMModLogger")) return;

            ReflectionHelp.FindMethod("ECMModLogger", "ECMModLogger.ECMCapsules.CapsuleManager", "CapsuleCollected").Invoke(null, new[] { id });
        }
        private void UnlockPDAEntry()
        {

        }
    }
}
