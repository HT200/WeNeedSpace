using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EnemyType { EASY, MEDIUM, HARD }

public abstract class EnemyController : MonoBehaviour
{
    private Vector3 position;
    private Vector3 direction;
    [HideInInspector] public Vector3 velocity;
    private Vector3 acceleration;
    
    public EnemyType enemyType;
    
    [SerializeField] protected ScoreManager scoreManager;
    protected PlayerController player;
    public GameManager gameManager;

    [SerializeField][Min(2f)] protected float maxSpeed = 2f;
    [SerializeField][Min(2f)] protected float maxForce = 2f;
    
    private float radius;
    private Vector3 right = Vector3.right;
    [SerializeField][Min(3f)] private float obstacleViewDistance;

    public MeshRenderer mesh;

    public float personalSpace = 1f;

    [SerializeField][Min(1f)] private float mass;
    [SerializeField][Min(1)] private int health;

    protected bool m_goSpawn = true;
    protected Vector3 m_target;
    private RaycastHit hit;

    public bool outOfBounds;

    public int Health => health;
    protected Vector3 Position => position;
    protected Vector3 Direction => direction;

    void Start()
    {
        //Why do we have the enemycontroller search for the gamemanager when the gamemanager is the one that spawns them? we can just set this when we instantiate it
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = gameManager.player.GetComponent<PlayerController>();
    
        position = transform.position;
        radius = mesh.bounds.extents.x;
        direction = Vector3.forward;
        
        float fourthToPlayer = position.z + (player.pos.z - position.z) / 4;
        m_target = new Vector3(position.x + Random.Range(-30, 30), position.y + Random.Range(-10, 10), fourthToPlayer);

        // Debug.Log(m_target);
        outOfBounds = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_goSpawn) MoveToSpawnTarget();
        else CalculateSteeringForces();
        if (outOfBounds) velocity -= position.normalized * 6.0f * Time.deltaTime;  
        UpdatePosition();
        transform.position = position;
    }

    /// <summary>
    /// Updates the position of the enemy using force-based movement
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
    /// Applies a given force to affect the enemy's acceleration
    /// </summary>
    /// <param name="force">The force to act upon this enemy</param>
    protected void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    /// <summary>
    /// Calculate the seek steering force to make the enemy target on the player
    /// </summary>
    /// <param name="targetPosition">Player's position</param>
    /// <returns>Seek steering force</returns>
    protected Vector3 Seek(Vector3 targetPosition)
    {
        // Calculate desired velocity
        Vector3 desiredVelocity = targetPosition - position;

        desiredVelocity = desiredVelocity.normalized * maxSpeed;
        
        // Calculate the seek steering force
        return desiredVelocity - velocity;
    }

    /// <summary>
    /// Calculate the flee steering force to make the enemy run away from the player
    /// </summary>
    /// <param name="targetPosition">Player's position</param>
    /// <returns>Flee steering force</returns>
    protected Vector3 Flee(Vector3 targetPosition)
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
    protected Vector3 Pursue(float seconds = 0.3f)
    {
        Vector3 futurePos = player.GetFuturePosition(seconds);
        float futureDistance = Vector3.SqrMagnitude(player.pos - futurePos);
        float distFromTarget = GetSqrDistance(player.pos);

        // If the enemy is within the future distance of the player, seek the player instead
        return distFromTarget < futureDistance * 5 ? Seek(player) : Seek(futurePos);
    }
    
    /// <summary>
    /// Determine where to flee the player
    /// </summary>
    /// <param name="seconds">Future timeframe</param>
    /// <returns></returns>
    protected Vector3 Evade(float seconds = 0.5f)
    {
        Vector3 futurePos = player.GetFuturePosition(seconds);
        float futureDistance = Vector3.SqrMagnitude(player.pos - futurePos);
        float distFromTarget = GetSqrDistance(player.pos);
 
        return distFromTarget < futureDistance ? Flee(player) : Flee(futurePos);
    }

    /// <summary>
    /// Return the square distance between the player and this enemy
    /// </summary>
    /// <returns>Square distance between the player and this enemy</returns>
    protected float GetSqrDistance(Vector3 objectPosition)
    {
        return Vector3.SqrMagnitude(objectPosition - position);
    }

    /// <summary>
    /// Prevent enemies from colliding onto one another
    /// </summary>
    /// <param name="enemies">Enemy list</param>
    /// <returns></returns>
    protected Vector3 Separate(List<EnemyController> enemies)
    {
        Vector3 separationForce = Vector3.zero;

        foreach (EnemyController other in enemies)
        {
            float sqrDistance = GetSqrDistance(other.Position);

            if (sqrDistance < Mathf.Epsilon) continue;

            if (sqrDistance < 0.001) sqrDistance = 0.001f;

            float personalSpaceRadius = personalSpace * personalSpace;
        
            if (sqrDistance < personalSpaceRadius) 
                separationForce += Flee(other) * (personalSpaceRadius / sqrDistance) / 4;
        }

        return separationForce;
    }

    /// <summary>
    /// Avoid Asteroids if there's one ahead of the enemy ship
    /// </summary>
    /// <returns>Steering Force</returns>
    protected Vector3 AvoidAsteroid()
    {
        if (hit.articulationBody == null) return Vector3.zero;

        if (Physics.Raycast(position, direction, obstacleViewDistance)
            && hit.transform.CompareTag("Asteroid"))
        {
            Debug.Log("Asteroid Ahead");
            return hit.normal * 20;
        }

        return Vector3.zero;
    }

    /// <summary>
    /// Move the enemy to the spawn target
    /// </summary>
    protected virtual void MoveToSpawnTarget()
    {
        Vector3 ultimateForce = Vector3.zero;
        ultimateForce += GetSqrDistance(player.pos) < GetSqrDistance(m_target) ? Pursue() : Seek(m_target);
        ultimateForce += Separate(gameManager.enemyList) / 3;
        ultimateForce += AvoidAsteroid();
        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);

        if (GetSqrDistance(m_target) <= 0.01f) m_goSpawn = false;

        ApplyForce(ultimateForce);
    } 

    /// <summary>
    /// Destroy this enemy
    /// </summary>
    void DestroyEnemy()
    {
        print("Enemy Destroyed");

        // Add to the player's combo
        scoreManager.SetCombo(scoreManager.GetCombo() + 1);
    }
    
    /// <summary>
    /// Update this enemy's health
    /// </summary>
    public void UpdateHealth(int num)
    {
        health += num;
        print("Health: " + health);

        if (health > 0) return;
        DestroyEnemy();
    }

    protected abstract void CalculateSteeringForces();

    #region Caller methods
    private Vector3 Flee(PlayerController targetObject)
    {
        return Flee(targetObject.transform.position);
    }

    private Vector3 Flee(EnemyController targetObject)
    {
        return Flee(targetObject.transform.position);
    }
    
    private Vector3 Seek(PlayerController targetObject)
    {
        return Seek(targetObject.pos);
    }
    #endregion
}
