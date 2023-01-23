using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MakersWrath.Platformer.Control;

namespace MakersWrath.Platformer.Combat {

    public class Missle : Projectile
    {
        public Vector3 right;
        public float damage = 7;

        public override Vector3 UpdateRight() {
            PlayerController2D player = FindObjectOfType<PlayerController2D>();
            Vector3 rotationVector = (player.transform.position - transform.position).normalized;
            rotationVector.z = 0;
            return rotationVector;
        }

        public override Vector3 UpdateVelocity() {
            PlayerController2D player = FindObjectOfType<PlayerController2D>();
            Vector3 velocity = (player.transform.position - transform.position).normalized * speed;
            velocity.z = 0;
            return velocity;
        }

        public override float GetDamage() {
            return damage;
        }

    }
}
