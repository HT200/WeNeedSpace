using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    private Vector3 position;
    private Vector3 direction;
    private Vector3 velocity;
    private Vector3 acceleration;

    public GameObject gManager;
    public GameObject ship;
    protected GameManager gameManager;
    protected PlayerController player;
    
    public Vector3 Position => position;
    
    public float maxSpeed = 2f;
    public float maxForce = 2f;

    private float radius;

    private Vector3 right = Vector3.right;
    public float obstacleViewDistance = 3f;

    public MeshRenderer mesh;

    public float personalSpace = 1f;
    
    [SerializeField][Min(0.001f)] private float mass = 1;
    [SerializeField][Min(1)] private int health = 1;
    
    public int Health => health;
    
    void Start()
    {
        gameManager = gManager.GetComponent<GameManager>();
        player = ship.GetComponent<PlayerController>();
        
        position = transform.position;
        radius = mesh.bounds.extents.x;
        direction = Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateSteeringForces();
        
        UpdatePosition();
        SetTransform();
    }
    
    /// <summary>
    /// Updates the position of the vehicle using force-based movement
    /// </summary>
    private void UpdatePosition()
    {
        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        if (velocity != Vector3.zero)
        {
            direction = velocity.normalized;

            right = Vector3.Cross(direction, Vector3.up);
        }
        acceleration = Vector3.zero;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.zero);
    }
    
    /// <summary>
    /// Set the vehicle's transform to the position set in UpdatePosition()
    /// </summary>
    private void SetTransform()
    {
        transform.position = position;
    }

    /// <summary>
    /// Applies a given force to affect the vehicle's acceleration
    /// </summary>
    /// <param name="force">The force to act upon this vehicle</param>
    protected void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }
    
    /// <summary>
    /// Calculate the seek steering force to make the enemy target on the player
    /// </summary>
    /// <param name="targetPosition">Player's position</param>
    /// <returns>Seek steering force</returns>
    private Vector3 Seek(Vector3 targetPosition)
    {
        // Calculate desired velocity
        Vector3 desiredVelocity = targetPosition - position;

        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculate the seek steering force
        return desiredVelocity - velocity;
    }
    
    // Caller method
    protected Vector3 Seek(PlayerController targetObject)
    {
        return Seek(targetObject.Pos);
    }
    
    /// <summary>
    /// Calculate the flee steering force to make the enemy run away from the player
    /// </summary>
    /// <param name="targetPosition">Player's position</param>
    /// <returns>Flee steering force</returns>
    private Vector3 Flee(Vector3 targetPosition)
    {
        // Calculate desired velocity
        Vector3 desiredVelocity = 2 * position - targetPosition;
        
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculate the flee steering force
        return desiredVelocity - velocity;
    }

    /// <summary>
    /// Determine where to seek the player
    /// </summary>
    /// <param name="seconds">Future timeframe</param>
    /// <returns></returns>
    public Vector3 Pursue(float seconds = 2f)
    {
        Vector3 futurePos = player.GetFuturePosition(seconds);
        float futureDistance = Vector3.SqrMagnitude(player.Pos - futurePos);
        float distFromTarget = GetSqrDistance(player.Pos);

        // If the enemy is within the future distance of the player, seek the player instead
        return distFromTarget < futureDistance ? Seek(player) : Seek(futurePos);
    }
    
    /// <summary>
    /// Return the square distance between the player and this enemy
    /// </summary>
    /// <returns>Square distance between the player and this enemy</returns>
    protected float GetSqrDistance(Vector3 objectPosition)
    {
        return Vector3.SqrMagnitude(objectPosition - Position);
    }
    
    protected Vector3 Separate(List<Enemy> vehicles)
    {
        Vector3 separationForce = Vector3.zero;

        foreach (Enemy other in vehicles)
        {
            float sqrDistance = GetSqrDistance(other.Position);

            if (sqrDistance < Mathf.Epsilon)
            {
                continue;
            }

            if (sqrDistance < 0.001)
            {
                sqrDistance = 0.001f;
            }

            float personalSpaceRadius = personalSpace * personalSpace;
            
            if (sqrDistance < personalSpaceRadius)
            {
                separationForce += Flee(other) * (personalSpaceRadius / sqrDistance);
            }
        }

        return separationForce;
    }
    
    /// <summary>
    /// Destroy this enemy
    /// </summary>
    void DestroyEnemy()
    {
        print("Enemy Destroyed");

        // Add to the player's combo
        gameManager.SetCombo(gameManager.GetCombo() + 1);

        Destroy(gameObject);
    }
    
    protected abstract void CalculateSteeringForces();
    
    #region Caller methods
    protected Vector3 Flee(PlayerController targetObject)
    {
        return Flee(targetObject.transform.position);
    }
    
    protected Vector3 Flee(Enemy targetObject)
    {
        return Flee(targetObject.transform.position);
    }
    #endregion
}
