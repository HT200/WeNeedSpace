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

    // Health and Shield values (temporary values)
    [SerializeField] private int m_maxHealth = 3;
    private int m_currentHealth;
    [SerializeField] private int m_maxShield = 3;
    private int m_currentShield;
    // Damage value (temporary)
    public int m_damage = 1;
    float deathtimer;
    public bool outOfBounds;
    // The player's health and damage output (temporary values?)
    public float health = 100.0f;
    public float damage = 5.0f;

    // All the physics vectors for updating movement (since thrust is changed on a frame by frame basis it doesnt need to be here)
    public Vector3 pos;
    public Vector3 vel;
    private Vector3 acc;

    //bool for when game is over so rotation/acceleration letter controls dont interfere with name typing
    bool gameover;

    // Laser variables
    public GameObject laserfire;
    float lasercooldown;
    //This is a bit a misnomer, this is actually the current force at the back of thie ship, its used to meter the max/min acceleration
    float speed;

    // Start is called before the first frame update
    void Start()
    {
        gameover = false;
        outOfBounds = false;
        deathtimer = 10.00f;
        if (m_gameManager == null)
        {
            m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        // Initialize current Health and Shield values to their maximums
        m_currentHealth = m_maxHealth;
        m_currentShield = m_maxShield;

        lasercooldown = 0;
        speed = 2.0000f;
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
            gameover = true;
            m_gameManager.SafeShutdown();
            m_gameManager.RemoveWarning();
        }

        if (outOfBounds)
        {
            //Tell the game manager you're out of bounds so it can warn you
            m_gameManager.BoundWarning(deathtimer);
            //Increment the timer
            deathtimer -= dt;
            //tractor beam
            pos += -pos.normalized * 3f * dt;



        }else if(deathtimer < 10.00f)
        {
            //If you aren't out of bounds and the death timer is out of sync, resync it
            deathtimer = 10.00f;
        }
        //end of out of bounds logic



        //None of this should be run if the game is over
        if (!gameover)
        {

            // Use only for development purposes for testing when the Player takes damage
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DamagePlayer();
            }

            // Acceleration controls, Higher increments of acceleration (both from the player and friction) give a better sense of control, this idea is largely inspired by celeste
            //Currently the player comes from full speed to a complete stop in about 26 meters (recognize this is with a starting velocity of 0)
            if (Input.GetKey(KeyCode.W))
            {
                if (speed < 2.0f)
                {
                    speed += 0.5f * dt;
                }
            }

            if (speed > 0)
            {
                speed -= 0.3f * dt;
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

            if (Input.GetKey(KeyCode.UpArrow))
            {
                pos += new Vector3(0.0f, 0.0f, 5.0f) * dt;
            }
            //ALL ROTATIONS SHOULD BE APPLIED BEFORE VECTOR CHANGES
            //Since acc is dependent on the transform.forward

            //Vector changes applied
            //If there is no force, then don't bother with these calculations, instead, start decelerating the velocity
            if (speed > 0)
            {
                acc = transform.forward * speed;
                if (vel.magnitude >= 5.0f)
                {
                    //This means that velocity is already at max

                    //If velocity is at max, you cant acclerate FORWARD, but you can accelerate in the sense of turning
                    //To replicate this, if at max speed, update only the direction of the velocity not the magnitude (unless acceleration would take you out of max speed)

                    if ((vel + acc * dt).magnitude > 5.0f)
                    {
                        vel = (vel + acc * dt).normalized * 5.0f;
                    }
                    else
                    {
                        vel += acc * dt;
                    }
                }
                else
                {
                    vel += acc * dt;
                }
            }
            else
            {
                //Velocity loses 50% of its current speed a second without acceleration
                vel *= 0.5f * dt;
            }

            pos += vel * dt;
            transform.position = pos;

            if (lasercooldown > 0.0f)
            {
                lasercooldown -= dt;
            }
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
    /// Update the player's Health and Shield when they take damage.
    /// This damage amount should always be 1.
    /// </summary>
    public void DamagePlayer()
    {
        // Player has taken damage so reset the combo
        m_gameManager.SetCombo(1);

        // If the Shield has been depleted, apply damage to Health instead.
        if (m_currentShield == 0)
        {
            Debug.Log("Player Health damaged from " + m_currentHealth + " to " + (m_currentHealth - 1));
            m_currentHealth -= 1;
        }
        else
        {
            Debug.Log("Player Shield damaged from " + m_currentShield + " to " + (m_currentShield - 1));
            m_currentShield -= 1;
        }

        // Update the UI using Health and Shield percentages of their max values.
        float healthPercent = (float)m_currentHealth / (float)m_maxHealth;
        float shieldPercent = (float)m_currentShield / (float)m_maxShield;
        m_playerUI.UpdateHealthAndShield(healthPercent, shieldPercent);

        // If both the Shield and Health values are depleted, it is Game Over
        if (m_currentHealth == 0 && m_currentShield == 0)
        {
            // Handle Game Over here
        }
    }

    /// <summary>
    /// Regenerate the player's Shield. Happens automatically at the end of each wave.
    /// </summary>
    public void RegenerateShield()
    {
        m_currentShield = m_maxShield;
    }

    /// <summary>
    /// Update the player's damage output
    /// </summary>
    public void UpdateDamage(int change)
    {
        // TODO: Upgrade weapons for more damage?
        Debug.Log("Player Damage updated from " + m_damage + " to " + (m_damage + change));
        m_damage += change;
    }
    //This is a read only get function so the gamemangager 
    public PlayerUI GetPlayerUI()
    {
        return this.m_playerUI;
    }
}
