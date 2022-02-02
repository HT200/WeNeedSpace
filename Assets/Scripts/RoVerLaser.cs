using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoVerLaser : MonoBehaviour
{ 
    public GameObject Point1;

    private Vector3 oceanFloorPosition;

    //Exercise 12 requires that you enable the x and z components of offset to be incremented/decremented
    //from the SimulationManager script when up/down/left/right arrow keys pressed

    public Vector3 offset;  //adjusts the x and z location of the spot on the ocean floor where the laser hits
    
    void Start()
    {
        offset = Vector3.zero;
    }

    void Update()  
    {

        gameObject.GetComponent<Laser>().from = Point1.transform.position;
        gameObject.GetComponent<Laser>().to = Point1.transform.forward*3f + Point1.transform.position;
    }

    Vector3 getOceanPos()
    {
        return this.oceanFloorPosition;
    }
}

