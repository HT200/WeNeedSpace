using UnityEngine;

public class MediumEnemy : EnemyController
{
    protected override void CalculateSteeringForces()
    {
        Vector3 ultimateForce = Vector3.zero;
        ultimateForce += m_goSpawn ? GetSqrDistance(player.pos) < GetSqrDistance(m_target) ? Pursue() : Seek(m_target) 
            : Pursue();
        ultimateForce += Separate(gameManager.enemyList) / 3;
        ultimateForce += AvoidAsteroid();

        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);
        
        if (m_goSpawn && GetSqrDistance(m_target) <= 0.01f) m_goSpawn = false;
            
        ApplyForce(ultimateForce);
    }
}
