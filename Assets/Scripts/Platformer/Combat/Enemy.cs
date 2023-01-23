using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using MakersWrath.Saving;

public class Enemy : MonoBehaviour, ISaveable
{

    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float speed = 100f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 5000f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;


    private Path path;
    private int currentWaypoint = 0;
    public bool isLeft = true;
    RaycastHit2D isGrounded;
    Seeker seeker;
    Rigidbody2D rb;

    //public AIPath aiPath;
    public int health = 5;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
        //rb.velocity = transform.right * speed;
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled){
            PathFollow();
        }
    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone()){
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null){
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count){
            return;
        }

        // See if colliding with anything
        Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);
        isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.05f);
        
        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        // set dir
        isLeft = direction.x < 0;

        // Jump
        if (jumpEnabled && isGrounded){
            if (direction.y > jumpNodeHeightRequirement){
                if(direction.y < 0.8){
                    rb.AddForce(Vector2.up * speed * jumpModifier);
                }
                else{
                    rb.AddForce(Vector2.up * speed * jumpModifier * 2);
                } 
            }
        }

        // Movement
        rb.AddForce(force);

        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance){
            currentWaypoint++;
        }

        // Direction Graphics Handling
        if (directionLookEnabled){
            if (rb.velocity.x >= 0.01f){
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x <= -0.01f){
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error){
            path = p;
            currentWaypoint = 0;
        }
    }


    public void Damage(int damage)
    {
        health -= damage;
        if(health <= 0){
            Destroy(this.gameObject);
        }
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
