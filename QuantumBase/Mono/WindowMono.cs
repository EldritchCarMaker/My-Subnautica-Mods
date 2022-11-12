using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuantumBase.Mono
{
    internal class WindowMono : MonoBehaviour
    {
        public const float lerpSpeed = 1;
        public const float openDuration = 5;

        public KeypadDoorConsole keypad;

        private GameObject topWindowPane;
        private GameObject bottomWindowPane;

        private Vector3 openWindowScale = new Vector3(2.5f, 1, 2f);
        private Vector3 closedWindowScale = new Vector3(2.5f, 1, 3);

        private float progress = 1;
        private float timeSpentOpen = 0;
        private bool windowOpen = false;

        public void MakeWindow(GameObject windowPrefabObject)
        {
            topWindowPane = Instantiate(windowPrefabObject); 
            Destroy(topWindowPane.GetComponent<Constructable>());
            topWindowPane.transform.parent = transform;
            topWindowPane.transform.localPosition = new Vector3(1.43f, -9.21f, -7.05f);
            topWindowPane.transform.localEulerAngles = new Vector3(89, 90, 0);
            topWindowPane.transform.localScale = closedWindowScale;


            bottomWindowPane = Instantiate(windowPrefabObject);
            Destroy(bottomWindowPane.GetComponent<Constructable>());
            bottomWindowPane.transform.parent = transform;
            bottomWindowPane.transform.localPosition = new Vector3(1.49f, -13.15f, -7.05f);
            bottomWindowPane.transform.localEulerAngles = new Vector3(270, 90, 0);
            bottomWindowPane.transform.localScale = closedWindowScale;
        }

        public void UnlockDoor()
        {
            ErrorMessage.AddMessage("Opening window");
            ToggleWindow();
        }
        public void ToggleWindow()
        {
            windowOpen = !windowOpen;
            progress = 0;
            timeSpentOpen = 0;
        }
        public void Update()
        {
            if (progress >= 1)
            {
                if (windowOpen)
                {
                    timeSpentOpen += Time.deltaTime;
                    if(timeSpentOpen >= openDuration)
                    {
                        ToggleWindow();
                        if(keypad)
                        {
                            keypad.ResetNumberField();
                            keypad.keypadUI.SetActive(true);
                            keypad.unlockIcon.SetActive(false);
                        }
                    }
                }
                return;
            }

            var startScale = windowOpen ? closedWindowScale : openWindowScale;//start closed if the window is opening, else start open if the window is closing
            var endScale = windowOpen ? openWindowScale : closedWindowScale;//end open if opening, end closed if closing

            var scale = Vector3.Lerp(startScale, endScale, progress);
            progress += Time.deltaTime * lerpSpeed;
            progress = Mathf.Clamp01(progress);

            topWindowPane.transform.localScale = scale;
            bottomWindowPane.transform.localScale = scale;
        }
    }
}
