using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakersWrath.Platformer.Combat {
    public class Laser : Projectile
    {
        public float damage = 10;

        public override Vector3 UpdateVelocity() {
            return new Vector3(isLeft ? -1 : 1, 0, 0) * speed;
        }

        // assumes z value is 0
        public override Vector3 UpdateRight() {
            return new Vector3(isLeft ? -1 : 1, 0, 0);
        }

        public override float GetDamage() {
            return damage;
        }
        
    }
}
