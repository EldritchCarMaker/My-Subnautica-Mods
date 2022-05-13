using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CyclopsWindows
{
    public class SubRootMarker : MonoBehaviour
    {
        private static readonly FMODAsset deployBeaconSound = GetFmodAsset("event:/sub/cyclops/load_decoy");
        public void FlipLockers()
        {
            CyclopsLocker singleLocker = GetComponentInChildren<CyclopsLocker>();
            GameObject parent = singleLocker.gameObject.transform.parent.parent.parent.gameObject;
            CyclopsLocker[] lockers = parent.GetComponentsInChildren<CyclopsLocker>();
            int i = 0;
            foreach (CyclopsLocker locker in lockers)
            {
                i++;
                GameObject correctObject = locker.transform.parent.parent.gameObject;
                correctObject.transform.eulerAngles += new Vector3(0, 0, 180);
                correctObject.transform.position -= (i != 2 ? 2.65f : 2f) * locker.transform.right;
                correctObject.transform.position -= 1f * locker.transform.up;
            }
            PlayWindowSound();
        }
        public void PlayWindowSound()
        {
            Utils.PlayFMODAsset(deployBeaconSound, transform.position - transform.forward);
        }

        //thanks Lee!
        public static FMODAsset GetFmodAsset(string audioPath)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = audioPath;
            return asset;
        }
    }
}
