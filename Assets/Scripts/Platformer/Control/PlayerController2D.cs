using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MakersWrath.Saving;

namespace MakersWrath.Platformer.Control
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SaveManager))]
    public class PlayerController2D : MonoBehaviour, ISaveable
    {
        [Header("General")]
        [SerializeField] float speed;
        [SerializeField] float jumpForce;
        [SerializeField] Transform groundCheck;

        [Header("Dashing")]
        [SerializeField] float dashCooldownLength;
        [SerializeField] float dashForce;
        [SerializeField] float dashLength;
        [SerializeField] GameObject dashParticles;

        int moveDir = 0; // -1 for left, 1 for right
        int facingDir = 1;

        bool isGrounded = false;
        float checkRadius = 0.2f;
        Collider2D playerCollider;
        Rigidbody2D rb;
        SaveManager saveManager;
        float dashCooldown = 0; // think about a cooldown manager if we need more cooldown based things
        bool isDashing = false;
        bool usedDash = false; // track dash usage to not allow multiple midair dashes
        float dashTime = 0;

        private void Awake()
        {
            playerCollider = GetComponent<Collider2D>();
            rb = GetComponent<Rigidbody2D>();
            saveManager = GetComponent<SaveManager>();
        }

        private void Dash()
        {
            if (dashCooldown > 0) return;
            if (usedDash) return;

            rb.velocity = new Vector2(facingDir * dashForce, 0);
            dashCooldown = dashCooldownLength;

            isDashing = true;
            usedDash = true;
            dashTime = dashLength;

            Instantiate(dashParticles, transform.position, Quaternion.Euler(dashParticles.transform.rotation.eulerAngles + new Vector3(facingDir == 1 ? 180 : 0, 0, 0)));
        }

        private void UpdateCooldowns()
        {
            if (dashCooldown > 0)
            {
                dashCooldown -= Time.deltaTime;
            }
            if (dashTime > 0)
            {
                dashTime -= Time.deltaTime;
            }
            if (isGrounded)
            {
                usedDash = false;
            }
        }

        private void TakePlayerInput()
        {
            // TODO: ifelseifelseifelseifelseifelse
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                moveDir = Input.GetKey(KeyCode.A) ? -1 : 1;
                //now we check if we flip the player
                if (facingDir != moveDir)
                { // we flip
                    transform.Rotate(0, 180, 0); // TODO: Change as this affects the sprite
                }
                facingDir = moveDir;
            }
            else
            {
                moveDir = 0;
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.W) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            // Dash
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Dash();
            }

            // save
            if (Input.GetKey(KeyCode.K))
            {
                saveManager.Save();
            }
            // or load
            else if (Input.GetKey(KeyCode.L))
            {
                saveManager.Load();
            }
        }

        void Update()
        {
            UpdateCooldowns();
            TakePlayerInput();
        }

        private void FixedUpdate()
        {
            // Check grounded
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, checkRadius);
            //Check if any of the overlapping colliders are not player collider, if so, set isGrounded to true
            isGrounded = false;
            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] != playerCollider)
                    {
                        isGrounded = true;
                        break;
                    }
                }
            }
            // Ground check debug
            Debug.DrawLine(groundCheck.position, groundCheck.position + Vector3.up * checkRadius, isGrounded ? Color.green : Color.red);
            Debug.DrawLine(groundCheck.position, groundCheck.position - Vector3.up * checkRadius, isGrounded ? Color.green : Color.red);

            // Apply movement
            if (dashTime <= 0)
            {
                isDashing = false;
            }
            Vector2 velocity = isDashing ? new Vector2(rb.velocity.x, 0) : new Vector2(moveDir * speed, rb.velocity.y);
            rb.velocity = velocity;
        }

        public object CaptureState()
        {  
            return transform.position;
        }

        public void RestoreState(object state) {
            transform.position = (Vector3) state;
            FixedUpdate();
        }
    }
}
