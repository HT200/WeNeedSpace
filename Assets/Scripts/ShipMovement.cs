using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    Vector3 pos;
    Vector3 vel;
    Vector3 acc;

    public GameObject laserfire;

    float lasercooldown;
    float speed;
    // Start is called before the first frame update
    void Start()
    {
        lasercooldown = 0;
        speed = 0.0000f;
        pos = transform.position;
        vel = transform.forward * 0.00f;
        acc = transform.forward*speed;
    }
    //Starting speed is 10^-2 unity blocks per frame
    //acceleration increments by 10^-7 unity blocks per frame^2

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        //Acceleration controls
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (speed < 1.0f)
            {
                speed += 0.1f * dt;
            }
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (speed > 0.0f)
            {
                speed -= 0.1f * dt;
            }
        }

        //Rotation Controls
        if (!Input.GetKey(KeyCode.R))
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(0, -0.1f, 0, Space.Self);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(0, 0.1f, 0, Space.Self);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(0, 0, 0.1f, Space.Self);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(0, 0, -0.1f, Space.Self);
            }
        }



        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(0.1f, 0, 0,Space.Self);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(-0.1f, 0, 0,Space.Self);
        }


        //Firing mechanism
        if (Input.GetKey(KeyCode.Space) && lasercooldown <= 0.0f)
        {
            //Currently this has the laserfire instantiated at the cylinder end, and the rotation is perfectly alignned with the "cannon"
            //In the future it might be best to have the rotation be slightly randomized to allow for "bullet" spread
            //Additionally it might be best to tweak the code so it doesnt perform physical movement on a frame by frame basis but
            //instead uses actual time since the last frame to account for differing computer speeds

            //You may be wondering why im creating a temp object when the Instantiate() method can be called alone, creating
            //A temp object allows us to reference it and its subcomponents if we want to after firing
            //So if we create more complex laser fire it might be necessary
            GameObject temp = Instantiate(laserfire, transform.position + transform.forward * 1.5f, transform.rotation);
            // the interval between shots is 1/5th of a second
            lasercooldown = 0.2f;
        }else if (lasercooldown > 0.0f)
        {
            lasercooldown -= dt;
        }




        //Vector changes applied

        acc = transform.forward * speed;
        if(vel.magnitude >= 5.0f)
        {
            //This means that velocity is already at max

            //If velocity is at max, you cant acclerate FORWARD, but you can accelerate in the sense of turning
            //To replicate this, if  at max speed, update only the direction of the velocity not the magnitude

            //EDIT: because of how this works there is no loss of speed while turning
            //as a result you can turn on a dime and suddenly be moving in a new direction easily
            //We need to create a way to LOSE speed not only during turns but in general as well

            vel = (vel + acc).normalized * 5f;

        }
        else
        {
            vel += acc * dt;
        }

        pos += vel * dt;
        transform.position = pos;
        OnGUI();
    }

    public void OnGUI()
    {
        GUI.color = Color.white;
        GUI.skin.box.fontSize = 15;
        GUI.skin.box.wordWrap = false;

        GUI.Box(new Rect(0, 0, 300, 30), "Current velocity: " + Mathf.Round(vel.magnitude * 10000)/100 + " x 10^-2");
        GUI.Box(new Rect(0, 30, 300, 30), "Acceleration: " + speed);
    }
}
