using System.Collections.Generic;
using UnityEngine;

namespace Temp.Enemy
{
    public abstract class Enemy : MonoBehaviour
    {
        private Vector3 position;
        private Vector3 direction;
        private Vector3 velocity;
        private Vector3 acceleration;

        [SerializeField] private GameObject gManager;
        [SerializeField] protected GameObject ship;
        [SerializeField] protected GameManager gameManager;
        [SerializeField] protected PlayerController player;

        [SerializeField][Min(2f)] protected float maxSpeed = 2f;
        [SerializeField][Min(2f)] protected float maxForce = 2f;

        private float radius;
        private Vector3 target;

        private Vector3 right = Vector3.right;
        [SerializeField][Min(3f)] private float obstacleViewDistance;

        public MeshRenderer mesh;

        public float personalSpace = 1f;
    
        [SerializeField][Min(1f)] private float mass;
        [SerializeField][Min(1)] private int health;

        private bool goSpawn = false;
    
        public int Health => health;
        public Vector3 Position => position;
    
        void Start()
        {
            gameManager = gManager.GetComponent<GameManager>();
            player = ship.GetComponent<PlayerController>();
        
            position = transform.position;
            radius = mesh.bounds.extents.x;
            direction = Vector3.forward;
        
            Vector3 halfToPlayer = position + (player.Pos - position) / 2;
            target = halfToPlayer + Random.onUnitSphere * halfToPlayer.magnitude;
        }

        // Update is called once per frame
        void Update()
        {
            if (!goSpawn) MoveToSpawnTarget();
            else CalculateSteeringForces();
        
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
            return Vector3.SqrMagnitude(objectPosition - position);
        }
    
        /// <summary>
        /// Prevent enemies from colliding onto one another
        /// </summary>
        /// <param name="enemies">Enemy list</param>
        /// <returns></returns>
        protected Vector3 Separate(List<Enemy> enemies)
        {
            Vector3 separationForce = Vector3.zero;

            foreach (Enemy other in enemies)
            {
                float sqrDistance = GetSqrDistance(other.Position);

                if (sqrDistance < Mathf.Epsilon) continue;

                if (sqrDistance < 0.001) sqrDistance = 0.001f;

                float personalSpaceRadius = personalSpace * personalSpace;
            
                if (sqrDistance < personalSpaceRadius) 
                    separationForce += Flee(other) * (personalSpaceRadius / sqrDistance);
            }

            return separationForce;
        }
    
        /*public Vector3 AvoidObstacle(Vector3 obstaclePos, float obstacleRadius)
    {
        // Get a vector from this vehicle to the obstacle
        Vector3 vToObs = obstaclePos - position;
        
        // Check if the obstacle is behind the vehicle
        float fwdToObsDot /*forward to obstacle dot products#1# = Vector3.Dot(Vector3.forward, vToObs);
        
        // If the obstacle is behind the object
        if (fwdToObsDot < 0)
        {
            return Vector3.zero;
        }
        
        // Check to see if the obstacle is too far to the left or right
        float rightToObsDot = Vector3.Dot(Vector3.right, vToObs);
        
        // If the obstacle is too far on the left/right
        if (Mathf.Abs(rightToObsDot) > obstacleRadius + radius)
        {
            return Vector3.zero;
        }
        
        // Check to see if the obstacle is in our view range
        if (fwdToObsDot > obstacleViewDistance)
        {
            return Vector3.zero;
        }
        
        // Create a weight based on how close we are to the obstacle
        float weight = obstacleViewDistance / Mathf.Max(fwdToObsDot, 0.001f);

        Vector3 desiredVelocity;
        if (rightToObsDot > 0)
        {
            // If the obstacle is on the right, steer left
            desiredVelocity = right * -maxSpeed;
        }
        else
        {
            // If the obstacle is on the left, steer right
            desiredVelocity = right * maxSpeed;
        }
        
        // Calculate our steering force from our desired velocity
        Vector3 steeringForce = (desiredVelocity - velocity) * weight;

        // return our steering force
        return steeringForce;
    }

    public Vector3 AvoidObstacle(GameObject obstacle)
    {
        return AvoidObstacle(obstacle.transform.position, obstacle.Radius);
    }

    public Vector3 AvoidAllObstacles(List<GameObject> obstacles)
    {
        return obstacles.Aggregate(Vector3.zero, (current, obstacle) => current + AvoidObstacle(obstacle));
    }*/
    
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

        /// <summary>
        /// Move the enemy to the spawn target
        /// </summary>
        private void MoveToSpawnTarget()
        {
            Vector3 ultimateForce = Vector3.zero;
            ultimateForce += Seek(target);
            //ultimateForce += Separate();
            //ultimateForce += AvoidAllObstacles();
            ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);
            
            ApplyForce(ultimateForce);
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
}
