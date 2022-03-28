using UnityEngine;

namespace Temp.Enemy
{
    public class Normal : Enemy
    {
        protected override void CalculateSteeringForces()
        {
            Vector3 ultimateForce = Vector3.zero;
            ultimateForce += Pursue();
            //ultimateForce += Separate();

            ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);
            
            ApplyForce(ultimateForce);
        }
    }
}
