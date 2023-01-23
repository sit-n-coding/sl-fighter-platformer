using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MakersWrath.Platformer.Control;

namespace MakersWrath.Platformer.Combat {

    public class Shooter : MonoBehaviour
    {
        public Transform shootingPoint;
        public GameObject laserPrefab;
        public PlayerController2D player;


        void Start() {
            player = GetComponent<PlayerController2D>();
        }

    // TODO: Should we only allow shooting iff on the ground (kind of like melee)
        void Update()
        {
            if(Keyboard.current.spaceKey.wasPressedThisFrame){
                GameObject go = Instantiate(laserPrefab, shootingPoint.position, transform.rotation);
                Laser laser = go.GetComponent<Laser>();
                laser.SetOwnerTag("Player");
                laser.SetDir(player.transform.rotation.y < 0);
            }
        }
    }

}
