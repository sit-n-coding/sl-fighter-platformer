using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MakersWrath.Saving;
using MakersWrath.Stats;

namespace MakersWrath.Platformer.Combat {
    public class EnemyController : MonoBehaviour
    {

        [Header("Shooting Projectiles")]
        public float homingDistance = 16;
        public float bombDistance = 5;
        public float laserRange = 4;
        public float fireRate = 0.5f;

        [Header("Saving & Loading")]
        public float stayAwayDistance = 16;
        public float playerRadius = 5;
        public int numProjectilesToSave = 2;
        public float loadAnywaysAfterSeconds = 6;
        public float criticalHeatlhPoint = 0.3f;
        public float healthDeltaToLoad = 0.1f;
        public float loadCooldown = 3;

        float currentFireRate;
        SaveManager saveManager;
        Enemy enemy;
        Health health;
        MetalTrap trap;
        GameObject player;
        float pastTime = 0; // in seconds
        float saveTime = 0;
        float loadTime = 0;
        float saveHealth = 0;
        ProjectileFactory projectileFactory;
        List<Projectile> projectiles = new List<Projectile>();

        void Start()
        {
            saveManager = GetComponent<SaveManager>();
            trap = GameObject.Find("MetalTrap").GetComponent<MetalTrap>();
            player = GameObject.Find("Player");
            enemy = GetComponent<Enemy>();
            health = GetComponent<Health>();
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

        bool IsPlayerInBadSpot() {
            int close = 0;
            for (int i = 0; i < projectiles.Count; i++) {
                if ((projectiles[i].transform.position - player.transform.position).magnitude <= playerRadius && !projectiles[i].disabled) {
                    close++;
                }
            }
            return close >= numProjectilesToSave;
        }

        bool ShouldLoadAnyways() {
            return Time.time - saveTime >= loadAnywaysAfterSeconds;
        }

        bool IsPlayerFar() {
            return (transform.position - player.transform.position).magnitude >= stayAwayDistance;
        }

        bool IsInCriticalHealth() {
            // may get affected if player loads
            return health.HealthPoints / health.InitialHealthPoints <= criticalHeatlhPoint;
        }

        bool LostTooMuchHealth() {
            // may get affected if player loads
            return saveHealth - (health.HealthPoints / health.InitialHealthPoints) >= healthDeltaToLoad;
        }

        bool PlayerTrapped() {
            return false;
        }

        bool SaveReady() {
            return Time.time - loadTime >= loadCooldown;
        }

        void Update()
        {
            if (saveManager.hasSaveState) {
                if ((ShouldLoadAnyways() || LostTooMuchHealth()) && !trap.IsPlayerTrapped()) {
                    saveManager.Load();
                    loadTime = Time.time;
                }
            } else if (SaveReady() && ((IsPlayerFar() && IsInCriticalHealth()) || IsPlayerInBadSpot())) {
                saveManager.Save();
                saveTime = Time.time;
                saveHealth = health.HealthPoints / health.InitialHealthPoints;
            }

            if (Time.time - pastTime > fireRate || (Time.time - pastTime > fireRate / 2 && IsInCriticalHealth())) {
                Shoot();
                pastTime = Time.time;
            }          
        }
    }
}
