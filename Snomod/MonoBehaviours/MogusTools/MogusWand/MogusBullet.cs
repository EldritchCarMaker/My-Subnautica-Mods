using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Snomod.MonoBehaviours
{
    internal class MogusBullet : Bullet
    {
        public float damage = 100;
        public DamageType damageType = DamageType.Starve;
        public override void OnHit(RaycastHit hitInfo)
        {
            if(!hitInfo.collider)
            {
                base.OnHit(hitInfo);
                return;
            }
            var mixin = hitInfo.collider.gameObject.FindAncestor<LiveMixin>();
            if (mixin)
                mixin.TakeDamage(damage, hitInfo.point, damageType, go);
            base.OnHit(hitInfo);
        }
    }
}
