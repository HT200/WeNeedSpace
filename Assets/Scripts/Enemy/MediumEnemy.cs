using UnityEngine;

public class MediumEnemy : EnemyController
{
    protected override void CalculateSteeringForces()
    {
        Vector3 ultimateForce = Vector3.zero;
        ultimateForce += Pursue();
        ultimateForce += Separate(gameManager.enemyList);
        //ultimateForce += AvoidAllAsteroids(gameManager.asteroidList);

        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);
            
        ApplyForce(ultimateForce);
    }
}
