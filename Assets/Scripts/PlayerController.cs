using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Game Manager Reference
    [SerializeField] private GameManager m_gameManager;
    // Player UI Reference
    [SerializeField] private PlayerUI m_playerUI;
    // Player Camera Reference
    [SerializeField] private Camera m_camera;

    float deathtimer;
    public bool outOfBounds;
    // The player's health and damage output (temporary values?)
    public float health = 100.0f;
    public float damage = 5.0f;

    // All the physics vectors for updating movement (since thrust is changed on a frame by frame basis it doesnt need to be here)
    private Vector3 pos;
    private Vector3 vel;
    private Vector3 acc;

    // Laser variables
    public GameObject laserfire;
    float lasercooldown;
    
    //This is a bit a misnomer, this is actually the current force at the back of thie ship, its used to meter the max/min acceleration
    float speed;

    // Start is called before the first frame update
    void Start()
    {
        outOfBounds = false;
        deathtimer = 10.00f;
        if (m_gameManager == null)
        {
            m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        lasercooldown = 0;
        speed = 0.0000f;
        pos = transform.position;
        vel = transform.forward * 0.00f;
        acc = transform.forward * speed;
    }
    //Starting speed is 10^-2 unity blocks per frame
    //acceleration increments by 10^-7 unity blocks per frame^2

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        //Out of bounds logic
        if (deathtimer <= 0.0f)
        {
            Destroy(this.gameObject);
        }

        if (outOfBounds)
        {
            deathtimer -= dt;
        }else if(deathtimer < 10.00f)
        {
            deathtimer = 10.00f;
        }
        //end of out of bounds logic


        // Acceleration controls
        if (Input.GetKey(KeyCode.W))
        {
            if (speed < 0.5f)
            {
                speed += 0.1f * Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (speed > 0.0f)
            {
                speed -= 0.1f * Time.deltaTime;
            }
        }

        // Rotation Controls via keyboard (Roll)
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, 0, 0.5f, Space.Self);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, 0, -0.5f, Space.Self);
        }

        //ALL ROTATIONS SHOULD BE APPLIED BEFORE VECTOR CHANGES
        //Since acc is dependent on the transform.forward

        //Vector changes applied

        acc = transform.forward * speed;
        if (vel.magnitude >= 5.0f)
        {
            //This means that velocity is already at max

            //If velocity is at max, you cant acclerate FORWARD, but you can accelerate in the sense of turning
            //To replicate this, if at max speed, update only the direction of the velocity not the magnitude (unless acceleration would take you out of max speed)

            if((vel+acc).magnitude > 5.0f)
            {
                vel = (vel + acc * dt).normalized * 5.0f;
            }
            else
            {
                vel += acc* dt;
            }
        }
        else
        {
            vel += acc * Time.deltaTime;
        }

        pos += vel * Time.deltaTime;
        transform.position = pos;

        if (lasercooldown > 0.0f)
        {
            lasercooldown -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Fire a single laser. This method is called when allowed by the PlayerUI script.
    /// </summary>
    public void FireLaser()
    {
        if (lasercooldown <= 0.0f)
        {
            // Currently this has the laser instantiated at the cylinder end, and the rotation is perfectly alignned with the "cannon"
            // In the future it might be best to have the rotation be slightly randomized to allow for "bullet" spread
            // Additionally it might be best to tweak the code so it doesnt perform physical movement on a frame by frame basis but
            // instead uses actual time since the last frame to account for differing computer speeds
            GameObject temp = Instantiate(laserfire, transform.position + transform.forward * 1.5f, transform.rotation);

            // Convert the crosshair/mouse position to a 3D point in world space (assume the UI is 10 units away from the camera)
            Vector3 worldPos = m_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            // Point the newly created laser GameObject towards the point in world space
            temp.transform.LookAt(worldPos);
            Laser tempScript = temp.GetComponent<Laser>();
            tempScript.speed = 20.0f;

            // The interval between shots is 1/5th of a second
            lasercooldown = 0.2f;
            m_gameManager.DecrementCombo();
        }
    }

    /// <summary>
    /// Update the player's health
    /// </summary>
    public void UpdateHealth(float num)
    {
        if (num < 0)
        {
            // Player has taken damage, reset combo
            m_gameManager.SetCombo(1);
        }

        health += num;
        print(num + " Health");
    }

    /// <summary>
    /// Update the player's damage output
    /// </summary>
    public void UpdateDamage(float change)
    {
        // TODO: Upgrade weapons for more damage?
        damage += change;
        print("Player damage output changed to " + damage);
    }
}
