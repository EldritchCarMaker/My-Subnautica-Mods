using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckyBlock.MonoBehaviours
{
    internal class DestroyWhenFar : MonoBehaviour
    {
        public float distance = 100;
        public void Update()
        {
            if (Vector3.SqrMagnitude(transform.position - Player.main.transform.position) > (distance * distance)) Destroy(gameObject, 2);
        }
    }
}
