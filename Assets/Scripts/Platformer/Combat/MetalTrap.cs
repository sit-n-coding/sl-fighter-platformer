using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MakersWrath.Saving;

namespace MakersWrath.Platformer.Combat
{
    public enum TrapState
        {
            Waiting,
            Dropping,
            Captured,
            CapturedNull,
        }

    public readonly struct MetalTrapState {
        public MetalTrapState(Vector3 _position, TrapState _trapState, bool _dropIndicatorSpriteEnabled) {
            position = _position;
            trapState = _trapState;
            dropIndicatorSpriteEnabled = _dropIndicatorSpriteEnabled;
        }
        public TrapState trapState { get; }
        public Vector3 position { get; }
        public bool dropIndicatorSpriteEnabled { get; }
    }

    public class MetalTrap : MonoBehaviour, ISaveable
    {
        [Header("General")]
        [Range(0f, 1f)]
        [SerializeField] float dropSpeed = 1f;

        [Range(0f, 1f)]
        [SerializeField] float repeatRate = 1f;

        [SerializeField] bool isTouchingPlayer = false;   // Serializefield only show for debug
        [SerializeField] TrapState trapState = TrapState.Waiting;

        [Header("Spawning")]
        [Range(0, 18f)]
        [SerializeField] float spawnRangeX = 16f;

        [Range(8f, 20f)]
        [SerializeField] float spawnY = 15f;

        [Range(0f, 3f)]
        [Tooltip("Delay between showing indicator icon and dropping.")]
        [SerializeField] float delayBeforeDrop = 1f;

        [Tooltip("Delay before empty trap respawns.")]
        [Range(1f, 3f)]
        [SerializeField] float delayBeforeEmptyRespawn = 3f;

        // [Range(5f, 20f)]
        // [Tooltip("Time before the next trap is activated (15-20 is probably good, 10 for testing).")]
        // [SerializeField] float timeBetweenTraps = 10f;

        [Header("Icon")]
        [SerializeField] SpriteRenderer dropIndicatorSprite;

        [Header("Ground Check")]
        [SerializeField] GameObject groundCheck = null;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] bool isGrounded = false;   // Serializefield only show for debug

        private SaveableClock clock;

        public bool IsPlayerTrapped() {
            return trapState == TrapState.Captured;
        }

        private void Awake() {
            clock = GetComponent<SaveableClock>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == null) return;
            if (!collision.CompareTag("Player")) return;

            isTouchingPlayer = true;

            // Collides with player here
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision == null) return;
            if (!collision.CompareTag("Player")) return;

            isTouchingPlayer = false;
        }

        private void Start()
        {
            SpawnCage();
        }

        private void Update()
        {
            GameObject player = GameObject.Find("Player");
            if (player)
            {
                if (trapState == TrapState.Captured && !isTouchingPlayer) {
                    trapState = TrapState.CapturedNull;
                    clock.AddTimedFunctionCall(SpawnCage, delayBeforeEmptyRespawn);
                    return;
                }

                if (trapState != TrapState.Waiting) return;

                // only follow player if trap is hanging/waiting
                Vector3 pos = player.transform.position;
                transform.position = new Vector3(pos.x, transform.position.y, transform.position.z);
            }
        }

        #region Show/Hide Indicator
        private void ShowDropIndicator()
        {
            dropIndicatorSprite.enabled = true;
        }

        private bool HideDropIndicator()
        {
            dropIndicatorSprite.enabled = false;
            return true;
        }
        #endregion

        private bool BeginDropCage()
        {
            trapState = TrapState.Dropping;
            ShowDropIndicator();
            clock.AddTimedFunctionCall(HideDropIndicator, delayBeforeDrop);
            clock.AddTimedFunctionCall(ContinuouslyDropCage, delayBeforeDrop);
            return true;
        }

        private bool ContinuouslyDropCage()
        {
            if (trapState != TrapState.Dropping) { 
                clock.AddTimedFunctionCall(ContinuouslyDropCage, repeatRate);
                return true;
             }

            transform.position = new Vector3(transform.position.x, transform.position.y - dropSpeed, transform.position.z);

            isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, groundLayer);

            if (!isGrounded) { 
                clock.AddTimedFunctionCall(ContinuouslyDropCage, repeatRate);
                return true;
             }
            if (!isTouchingPlayer) 
            {
                if (transform.position.y > -6) { 
                clock.AddTimedFunctionCall(ContinuouslyDropCage, repeatRate);
                return true;
             }

                // grounded, not touching player, and below -6?
                // Cage has hit the ground without catching player
                trapState = TrapState.CapturedNull;
                clock.AddTimedFunctionCall(SpawnCage, delayBeforeEmptyRespawn);
                return true;
            }

            // Cage has hit ground and caught the player
            trapState = TrapState.Captured;
            return true;
            // TODO: Recall to escape cage or something
        }

        [Tooltip("Spawn/respawn trap.")]
        private bool SpawnCage()
        {
            trapState = TrapState.Waiting;
            float spawnOffsetX = Random.Range(-spawnRangeX, spawnRangeX);
            transform.position = new Vector3(transform.position.x + spawnOffsetX, spawnY, transform.position.z);
            clock.AddTimedFunctionCall(BeginDropCage, 5f);
            return true;
        }

        // For testing purposes
        #region ContextMenu
        [ContextMenu("Drop Cage")]
        private void TestDropCage()
        {
            BeginDropCage();
        }

        [ContextMenu("Spawn Cage")]
        private void TestSpawnCage()
        {
            SpawnCage();
        }
        #endregion

        public object CaptureState()
        {  
            return new MetalTrapState(transform.position, trapState, dropIndicatorSprite.enabled);
        }

        public void RestoreState(object _state) {
            MetalTrapState state = (MetalTrapState) _state;
            transform.position = state.position;
            trapState = state.trapState;
            dropIndicatorSprite.enabled = state.dropIndicatorSpriteEnabled;
        }
    }
}