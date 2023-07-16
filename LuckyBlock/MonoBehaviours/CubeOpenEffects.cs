using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckyBlock.MonoBehaviours
{
    internal class CubeOpenEffects : MonoBehaviour
    {
        public static float explosionForce = 200;

        [SerializeField]
        private List<Rigidbody> _horizontalFaces;
        [SerializeField]
        private List<Rigidbody> _verticalFaces;

        public IEnumerable<Rigidbody> AllFaces 
        { 
            get 
            { 
                var list = new List<Rigidbody>(_horizontalFaces); 
                list.AddRange(_verticalFaces); 
                return list.AsEnumerable(); 
            }
        }

        public void PlayOpenEffects()
        {
            Destroy(GetComponent<Collider>());//destroy main box collider now that its been opened
            foreach(var face in AllFaces)
            {
                //colliders are normally triggers to stop them from colliding with the main box collider
                face.GetComponent<Collider>().isTrigger = false;

                face.isKinematic = false;
                face.AddExplosionForce(explosionForce, transform.position, 500);
            }
            Destroy(gameObject, 25);
        }
    }
}
