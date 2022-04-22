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
        ultimateForce += AvoidAsteroid();
        
        //if (strafe) Debug.Log(gameObject.name);

        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);
            
        ApplyForce(ultimateForce);

        if (!(distanceFromPlayer < fireDistance)) return;

        if (strafe) laserTimer = laserCooldown / 4;
        
        if (laserTimer <= 0)
        {
            //Enemy lasers have been changed to be faster, and to always spawn going towards the player, as it stands it is too safe to stay still, these changes will make it dangerous to stay still
            Vector3 toPlayer = (player.pos - this.Position).normalized;
            Quaternion rotation = Quaternion.LookRotation(toPlayer, Vector3.zero);
            GameObject laser = Instantiate(laserPrefab, Position + toPlayer * 3f, rotation);
            laser.GetComponent<Laser>().speed = 50.0f;
            laserTimer = laserCooldown;
        }
        else laserTimer -= dt;
    }
}