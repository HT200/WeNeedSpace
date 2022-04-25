using UnityEngine;

public class EasyEnemy : EnemyController
{
    protected override void CalculateSteeringForces()
    {
        Vector3 ultimateForce = Vector3.zero;
        ultimateForce += Pursue();
        ultimateForce += Separate(gameManager.enemyList) / 3;
        ultimateForce += AvoidAsteroid();

        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);
            
        ApplyForce(ultimateForce);
    }
}
