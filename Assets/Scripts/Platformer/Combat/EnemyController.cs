using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MakersWrath.Saving;

namespace MakersWrath.Platformer.Combat {
    public class EnemyController : MonoBehaviour
    {

        public float homingDistance = 0;
        public float bombDistance = 0;
        public float laserRange = 0;
        public float fireRate = 0.5f;

        float currentFireRate;
        SaveManager saveManager;
        Enemy enemy;
        GameObject player;
        float pastTime = 0; // in seconds
        float pastTime2 = 0;
        int stratIndex = 0;
        ProjectileFactory projectileFactory;
        List<Projectile> projectiles = new List<Projectile>();

        void Start()
        {
            saveManager = GetComponent<SaveManager>();
            player = GameObject.Find("Player");
            enemy = GetComponent<Enemy>();
            projectileFactory = GetComponent<ProjectileFactory>();
        }

        void Shoot() {
            // launch homing missles
            if ((transform.position - player.transform.position).magnitude >= homingDistance) {
                GameObject go = projectileFactory.Init(ProjectileType.MISSLE, transform.position, enemy.isLeft, "Enemy");
                projectiles.Add(go.GetComponent<Projectile>());
            }
            else if ((transform.position - player.transform.position).magnitude <= bombDistance) {
                GameObject go = projectileFactory.Init(ProjectileType.BOMB, transform.position, enemy.isLeft, "Enemy");
                projectiles.Add(go.GetComponent<Projectile>());
            }
            else if (Mathf.Abs(transform.position.y - player.transform.position.y) <= laserRange) {
                GameObject go = projectileFactory.Init(ProjectileType.LASER, transform.position, enemy.isLeft, "Enemy");
                projectiles.Add(go.GetComponent<Projectile>());
            }
        }

        // bool IsPlayerInBadSpot() {
        //     for (int i = 0; i < tasks.Count; i++) {
        //     } 
        // }

        void Update()
        {
            if (Time.time - pastTime2 > 4) {
                if (stratIndex == 0) {
                    saveManager.Save();
                } else {
                    saveManager.Load();
                }
                stratIndex = (stratIndex + 1) % 2;
                pastTime2 = Time.time;
            }  
            if (Time.time - pastTime > fireRate) {
                Shoot();
                pastTime = Time.time;
            }          
        }
    }
}
