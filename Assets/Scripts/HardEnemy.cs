using UnityEngine;

public class HardEnemy : EnemyController
{
    [SerializeField][Min(100f)] private float strafeDistance;
    [SerializeField][Min(100f)] private float strafeArea;
    [SerializeField][Min(10f)] private float fireDistance;
    [SerializeField][Min(0.1f)] private float laserCooldown = 1f;
    [SerializeField] private GameObject laserPrefab;
        
    private float laserTimer = 0;
    private bool strafe = false;

    protected override void CalculateSteeringForces()
    {
        float dt = Time.deltaTime;
            
        Vector3 ultimateForce = Vector3.zero;
        float distanceFromPlayer = GetSqrDistance(player.pos);

        if (distanceFromPlayer < strafeDistance - strafeArea) strafe = true;
        if (strafe && distanceFromPlayer > strafeDistance) strafe = false;
        
        ultimateForce += strafe ? Evade() : Pursue();
        ultimateForce += Separate(gameManager.enemyList);

        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);
            
        ApplyForce(ultimateForce);

        if (!(distanceFromPlayer < fireDistance)) return;

        if (laserTimer <= 0)
        {
            Quaternion rotation = Quaternion.LookRotation(Direction, Vector3.zero);
            GameObject laser = Instantiate(laserPrefab, Position + transform.forward * 3f, rotation);
            laser.GetComponent<Laser>().speed = 20.0f;
            laserTimer = laserCooldown;
        }
        else laserTimer -= dt;
    }
}