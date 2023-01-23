using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakersWrath.Platformer.Combat {

    public class Bomb : Projectile
    {
        public float damage = 25;
        public float jumpForce = 100;
        bool jumped = false;

        public override Vector3 UpdateVelocity() {
            if (!jumped) {
                jumped = true;
                return new Vector2((isLeft ? -1 : 1 ) * speed, jumpForce);
            }
            return rb.velocity;
        }

        // assumes z value is 0
        public override Vector3 UpdateRight() {
            return transform.right;
        }

        public override float GetDamage() {
            return damage;
        }

    }
}
