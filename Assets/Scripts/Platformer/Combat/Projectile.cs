using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MakersWrath.Saving;
using MakersWrath.Stats;

namespace MakersWrath.Platformer.Combat {

    public readonly struct ProjectileState {
        public ProjectileState(Vector3 _position, bool _disabled) {
            position = _position;
            disabled = _disabled;
        }
        public bool disabled { get; }
        public Vector3 position { get; }
    }

    public abstract class Projectile : MonoBehaviour, ISaveable
    {
        public GameObject explosionEffect;
        public Rigidbody2D rb;
        public float speed;
        bool prevDisabled = false;
        public bool disabled = false;
        string ownerTag = "";
        Renderer mRenderer;
        PolygonCollider2D cb;
        public bool isLeft = true;

        public abstract float GetDamage();

        public void SetSpeed(float _speed) {
            speed = _speed;
        }

        // Start is called before the first frame update
        public virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            mRenderer = GetComponent<Renderer>();
            cb = GetComponent<PolygonCollider2D>();
        }

        public void SetOwnerTag(string _ownerTag) {
            ownerTag = _ownerTag;
        }

        public void SetDir(bool _isLeft) {
            isLeft = _isLeft;
        }

        public virtual void OnBecameInvisible() {
            disabled = true;
        }


        public virtual void Update()
        {
            
            if (!disabled) {
                rb.velocity = UpdateVelocity();
                transform.right = UpdateRight();
                if (prevDisabled) {
                    EnableGameObject(true);
                }
            } else {
                if (!prevDisabled) {
                    EnableGameObject(false);
                }
            }
            prevDisabled = disabled;
        }

        public abstract UnityEngine.Vector3 UpdateVelocity();

        public virtual UnityEngine.Vector3 UpdateRight() {
            return transform.right;
        }

        public virtual object CaptureState()
        {
            return new ProjectileState(transform.position, disabled);
        }

        public virtual void RestoreState(object state) 
        {
            transform.position = ((ProjectileState) state).position;
            disabled = ((ProjectileState) state).disabled;
        }

        public void EnableGameObject(bool enable) {
            cb.enabled = enable;
            mRenderer.enabled = enable;
            rb.isKinematic = !enable;
        }

        public virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Solid") || (collision.gameObject.CompareTag("SolidPlayerOnly") && ownerTag != "Enemy")) {
                EnableGameObject(false);
                disabled = true;
                Instantiate(explosionEffect, transform.position, transform.rotation);
            }
            else if (!collision.gameObject.CompareTag(ownerTag) && collision.TryGetComponent(out Health opponentHealth)) {
                opponentHealth.TakeDamage(GetDamage());
                EnableGameObject(false);
                disabled = true;
                Instantiate(explosionEffect, transform.position, transform.rotation);
            }       
        }
    }
}
