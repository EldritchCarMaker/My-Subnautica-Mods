using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuantumBase.Mono
{
    internal class QuantumExitDoor : MonoBehaviour
    {
        public bool DoNothing()
        {
            ErrorMessage.AddMessage("Nothing has been done.");
            return false;

            Application.Quit();
            return true;
        }
    }
}
