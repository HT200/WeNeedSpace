using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay(Collider other)
    {
        //We can tweak these values if we want, currently the force acting on the enemy/asteroid has an inverse relationship with distance
        //If the distance between them is 1m, then they should be moving towards the blackhole at 100m/s^2 + current vel
        //The force acting on the velocity scales exponentially with distance
        // A = 20*d^-2
        //at d=1m, A = 20m/s^2 
        //at d=2m A = 5m/s^s
        //d=5m, A = 0.8 m/s^s
        //d=10m A= .2 m/s^s
        if(other.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyController>().vel += (this.transform.position - other.transform.position).normalized * (40 / (this.transform.position - other.transform.position).magnitude) * Time.deltaTime;
        }
        if(other.tag == "Asteroid")
        {
            other.gameObject.GetComponent<AsteroidController>().vel += (this.transform.position - other.transform.position).normalized * (40 / (this.transform.position - other.transform.position).magnitude) * Time.deltaTime;
        }
    }
}
