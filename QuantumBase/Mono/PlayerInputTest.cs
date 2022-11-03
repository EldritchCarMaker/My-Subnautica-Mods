using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuantumBase.Mono
{
    internal class PlayerInputTest : MonoBehaviour
    {

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.K))
            {
                if(!(Player.main.currentSub is QuantumBase))
                {
                    QuantumBase.main.SetPlayerInBase();
                }
                else
                {
                    QuantumBase.main.SetPlayerOutBase();
                }
            }
        }
    }
}
