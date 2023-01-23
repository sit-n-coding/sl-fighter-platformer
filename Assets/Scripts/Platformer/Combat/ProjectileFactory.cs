using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MakersWrath.Saving;

namespace MakersWrath.Platformer.Combat {

    // types of missiles we have
    public enum ProjectileType {
        LASER,
        MISSLE,
        BOMB
    }

    // missile factory that initializes missile objects
    public class ProjectileFactory : MonoBehaviour
    {
        public GameObject laser;
        public GameObject bomb;
        public GameObject missle;

        public GameObject Init(ProjectileType type, Vector3 position, bool isLeft, string ownerTag) {
            GameObject newObj;
            if (type == ProjectileType.BOMB) {
                newObj = Instantiate(bomb, position, new Quaternion());
            } else if (type == ProjectileType.MISSLE) {
                newObj = Instantiate(missle, position, new Quaternion());
            } else {
                newObj = Instantiate(laser, position, new Quaternion());
            }
            SaveableEntity saveEntity = newObj.GetComponent<SaveableEntity>();
            saveEntity.RegenUniqueIdentifier();
            Projectile p = newObj.GetComponent<Projectile>();
            p.SetOwnerTag(ownerTag);
            p.SetDir(isLeft);
            return newObj;
        }
    }
}
