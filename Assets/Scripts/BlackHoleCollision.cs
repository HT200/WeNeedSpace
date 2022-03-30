using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleCollision : MonoBehaviour
{
    private Vector3 vel;

    // Start is called before the first frame update
    void Start()
    {
        vel = new Vector3(0.0f,0.0f,0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += vel*Time.deltaTime;
    }


    private void OnTriggerStay(Collider other)
    {
        //We can tweak these values if we want, currently the force acting on the enemy/asteroid has an inverse relationship with distance
        //If the distance between them is 1m, then they should be moving towards the blackhole at 100m/s^2 + current vel
        //The force acting on the velocity scales exponentially with distance
        // A = 1000*d^-1
        //The high acceleration means that it must be limtied when too close, so the black hole will not apply force changes when within 5 meters
        //at d=1m, A = 1000m/s^2 
        //at d=2m A = 500/s^s
        //d=5m, A = 200 m/s^s
        //Past the five meter mark however, its still very strong, but not so strong it throws its  objects out of range, I feel these tweaks give it a good feel but change them if you can find a better balance
        //d=10m A= 100 m/s^s


        if(other.CompareTag("Bullet"))
        {
            if ((this.transform.position - other.transform.position).sqrMagnitude > 25)
            {
                other.gameObject.GetComponent<Laser>().vel += (this.transform.position - other.transform.position).normalized * (1000 / (this.transform.position - other.transform.position).magnitude) * Time.deltaTime;
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        if(other.CompareTag("Enemy"))
        {
            if ((this.transform.position - other.transform.position).sqrMagnitude > 25)
            {
                other.gameObject.GetComponent<EnemyController>().velocity += (this.transform.position - other.transform.position).normalized * (1000 / (this.transform.position - other.transform.position).magnitude) * Time.deltaTime;
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        if(other.CompareTag("Asteroid"))
        {
            if ((this.transform.position - other.transform.position).sqrMagnitude > 25)
            {
                other.gameObject.GetComponent<AsteroidController>().vel += (this.transform.position - other.transform.position).normalized * (1000 / (this.transform.position - other.transform.position).magnitude) * Time.deltaTime;
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }
}
