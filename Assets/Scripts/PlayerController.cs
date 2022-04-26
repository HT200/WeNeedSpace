using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public GameObject Speedlines;
    // Game Manager Reference
    [SerializeField] private GameManager m_gameManager;
    // Score Manager Reference
    [SerializeField] private ScoreManager m_scoreManager;
    // Player UI Reference
    [SerializeField] private PlayerUI m_playerUI;
    // Player Camera Reference
    [SerializeField] private Camera m_camera;

    // Health and Shield values (temporary values)
    [SerializeField] private int m_maxHealth = 3;
    private int m_currentHealth;
    [SerializeField] private int m_maxShield = 3;
    private int m_currentShield;
    float deathtimer;
    public bool outOfBounds;
    // The player's health and damage output (temporary values?)
    float iFramesCooldown;

    // All the physics vectors for updating movement (since thrust is changed on a frame by frame basis it doesnt need to be here)
    public Vector3 pos;
    public Vector3 vel;
    private Vector3 acc;

    //bool for when game is over so rotation/acceleration letter controls dont interfere with name typing
    public bool gameover;

    //Black Hole Variables
    public GameObject BlackHoleObject;
    public int blackHoleCount;
    private float blackHoleCooldown;

    // Laser variables
    public GameObject laserfire;
    public float lasercooldown;
    public float m_laserCooldownDefault = 0.2f;
    public bool tripleShot = false;
    public float m_laserSpeed;
    public float m_laserSpeedDefault;

    //This is a bit a misnomer, this is actually the current force at the back of thie ship, its used to meter the max/min acceleration
    float speed;

    // Audio
    [SerializeField] private AudioSource m_accelerationAudioSource;
    [SerializeField] private AudioSource m_laserAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_laserSpeedDefault = 80.0f;
        iFramesCooldown = 0.0f;
        blackHoleCooldown = 0.0f;
        blackHoleCount = 2;
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
        m_laserSpeed = m_laserSpeedDefault;

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
        if(iFramesCooldown > 0.0f)
        {
            iFramesCooldown -= dt;
        }
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
            vel += -pos.normalized * 6f * dt;



        }
        else if (deathtimer < 10.00f)
        {
            //If you aren't out of bounds and the death timer is out of sync, resync it
            deathtimer = 10.00f;
        }
        //end of out of bounds logic



        //None of this should be run if the game is over
        if (!gameover)
        {

            //Firing Black Hole
            if (Input.GetMouseButtonDown(1) && blackHoleCooldown <= 0.0f && blackHoleCount != 0)
            {
                FireBlackHole();
            }

            // Acceleration controls, Higher increments of acceleration (both from the player and friction) give a better sense of control, this idea is largely inspired by celeste
            //Currently the player comes from full speed to a complete stop in about 26 meters (recognize this is with a starting velocity of 0)
            if (Input.GetKey(KeyCode.W))
            {
                if (speed < 4.0f)
                {
                    if (!m_accelerationAudioSource.isPlaying)
                    {
                        m_accelerationAudioSource.pitch = 1;
                        m_accelerationAudioSource.PlayOneShot(m_gameManager.m_acceleration, m_accelerationAudioSource.volume);
                    }
                    speed += 2.0f * dt;
                }
            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (!m_accelerationAudioSource.isPlaying)
                {
                    m_accelerationAudioSource.pitch = 0.6f;
                    m_accelerationAudioSource.PlayOneShot(m_gameManager.m_acceleration, m_accelerationAudioSource.volume);
                }
                if (speed > 0.0f)
                {
                    speed -= 1.2f * dt;
                }
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                StartCoroutine(AudioFadeOut.FadeOut(m_accelerationAudioSource, 0.5f));
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                StartCoroutine(AudioFadeOut.FadeOut(m_accelerationAudioSource, 0.5f));
            }

            if (speed > 0.0f)
            {
                speed -= 0.6f * dt;
            }

            // Rotation Controls via keyboard (Roll)
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0, 0, 90.0f * dt, Space.Self);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0, 0, -90.0f * dt, Space.Self);
            }
            //ALL ROTATIONS SHOULD BE APPLIED BEFORE VECTOR CHANGES
            //Since acc is dependent on the transform.forward

            //Vector changes applied
            //If there is no force, then don't bother with these calculations, instead, start decelerating the velocity
            if (speed > 0)
            {
                acc = transform.forward * speed;
                if (vel.magnitude >= 10.0f)
                {
                    //This means that velocity is already at max

                    //If velocity is at max, you cant acclerate FORWARD, but you can accelerate in the sense of turning
                    //To replicate this, if at max speed, update only the direction of the velocity not the magnitude (unless acceleration would take you out of max speed)

                    if ((vel + acc * dt).magnitude > 10.0f)
                    {
                        vel = (vel + acc * dt * 2).normalized * 10.0f;
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
                vel -= vel * 0.5f * dt;
            }

            pos += vel * dt;
            transform.position = pos;

            if (lasercooldown > 0.0f)
            {
                lasercooldown -= dt;
            }
            if(blackHoleCooldown > 0.0f)
            {
                blackHoleCooldown -= dt;
            }


            //Particle System

            //at speedlines at v -> lim 0 should be: Radius 16, Rate over time 15, Start Speed 5
            //at Speedline v = 10 -> Radius = 13, Rate: 30, Start speed 15
            //Also to have the lines sort of "twist" when your turning
            if(vel.sqrMagnitude <= 5 && Speedlines.activeInHierarchy)
            {
                Speedlines.SetActive(false);
            }
            else if(vel.sqrMagnitude >= 5 && !Speedlines.activeInHierarchy)
            {
                Speedlines.SetActive(true);
            }

            if (Speedlines.activeInHierarchy)
            {
                //Vel goes from 0 to 10

                var ps = Speedlines.GetComponent<ParticleSystem>();
                var newshape = ps.shape;
                var newMain = ps.main;
                var newEmission = ps.emission;
                newshape.radius = 16 - (vel.magnitude * 0.3f);
                newMain.startSpeed = 5 + vel.magnitude;
                newEmission.rateOverTime = 15 + vel.magnitude * 1.5f;

                Speedlines.transform.forward = -vel;
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
            if (!tripleShot)
            {
                m_laserAudioSource.PlayOneShot(m_gameManager.m_laserAudio, m_laserAudioSource.volume);

                // Currently this has the laser instantiated at the cylinder end, and the rotation is perfectly alignned with the "cannon"
                // In the future it might be best to have the rotation be slightly randomized to allow for "bullet" spread
                GameObject temp = Instantiate(laserfire, transform.position + transform.forward * 1.5f, transform.rotation);
                // Convert the crosshair/mouse position to a 3D point in world space (assume the UI is 50 units away from the camera)
                Vector3 worldPos = m_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 50));
                // Point the newly created laser GameObject towards the point in world space
                temp.transform.LookAt(worldPos);
                Laser tempScript = temp.GetComponent<Laser>();
                tempScript.speed = m_laserSpeed;
                tempScript.pBullet = true;

            }
            else
            {
                m_laserAudioSource.PlayOneShot(m_gameManager.m_laserAudio, m_laserAudioSource.volume);
                m_laserAudioSource.PlayOneShot(m_gameManager.m_laserAudio, m_laserAudioSource.volume);
                m_laserAudioSource.PlayOneShot(m_gameManager.m_laserAudio, m_laserAudioSource.volume);
                float laserSpread = 40.0f;

                GameObject left = Instantiate(laserfire, transform.position + transform.forward * 1.5f, transform.rotation);
                left.transform.LookAt(m_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x - laserSpread, Input.mousePosition.y, 50)));
                left.GetComponent<Laser>().speed = m_laserSpeed;
                left.GetComponent<Laser>().pBullet = true;
                GameObject middle = Instantiate(laserfire, transform.position + transform.forward * 1.5f, transform.rotation);
                middle.transform.LookAt(m_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 50)));
                middle.GetComponent<Laser>().speed = m_laserSpeed;
                middle.GetComponent<Laser>().pBullet = true;
                GameObject right = Instantiate(laserfire, transform.position + transform.forward * 1.5f, transform.rotation);
                right.transform.LookAt(m_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + laserSpread, Input.mousePosition.y, 50)));
                right.GetComponent<Laser>().speed = m_laserSpeed;
                right.GetComponent<Laser>().pBullet = true;
            }

            // The interval between shots is 1/5th of a second by default
            lasercooldown = m_laserCooldownDefault;
            m_scoreManager.DecrementCombo();
        }
    }

    public void FireBlackHole()
    {
        Debug.Log("Current transform forward: " + transform.forward);
        GameObject temp = Instantiate(BlackHoleObject, transform.position + transform.forward * 10.0f, transform.rotation);
        temp.GetComponent<BlackHoleCollision>().m_gameManager = m_gameManager;
        blackHoleCount--;
        blackHoleCooldown = 15.0f;
    }

    /// <summary>
    /// Update the player's Health and Shield when they take damage.
    /// This damage amount should always be 1.
    /// </summary>
    public void DamagePlayer()
    {

        if (iFramesCooldown <= 0.0f)
        {
            // Player has taken damage so reset the combo
            m_scoreManager.SetCombo(1);

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

            UpdateHealthAndShield();
            iFramesCooldown = 0.3f;
        }
    }

    /// <summary>
    /// Update the player's Health and Shield when they receive health.
    /// The healing amount should always be 1.
    /// </summary>
    public void HealPlayer()
    {
        // If the player's health is not full
        if (m_currentShield < 3)
        {
            Debug.Log("Player Health increased from " + m_currentHealth + " to " + (m_currentHealth + 1));
            m_currentHealth += 1;
        }

        UpdateHealthAndShield();
    }

    /// <summary>
    /// Update the Health and Shield Bars.
    /// </summary>
    void UpdateHealthAndShield()
    {
        // Update the UI using Health and Shield percentages of their max values.
        float healthPercent = (float)m_currentHealth / (float)m_maxHealth;
        float shieldPercent = (float)m_currentShield / (float)m_maxShield;
        m_playerUI.UpdateHealthAndShield(healthPercent, shieldPercent);

        // If both the Shield and Health values are depleted, it is Game Over
        if (m_currentHealth == 0 && m_currentShield == 0)
        {
            // Handle Game Over here
            m_gameManager.SafeShutdown();
        }
    }

    /// <summary>
    /// Regenerate the player's Shield. Happens automatically at the end of each wave.
    /// </summary>
    public void RegenerateShield()
    {
        m_currentShield = m_maxShield;
        UpdateHealthAndShield();
    }

    //This is a read only get function so the gamemangager can find it
    public PlayerUI GetPlayerUI()
    {
        return this.m_playerUI;
    }

    /// <summary>
    /// Get the predicted positions where this vehicle should be in x seconds (used for enemy predictions)
    /// </summary>
    /// <param name="seconds">how many seconds ahead to look</param>
    /// <returns>predicted future positions</returns>
    public Vector3 GetFuturePosition(float seconds)
    {
        return pos + vel * seconds;
    }
}