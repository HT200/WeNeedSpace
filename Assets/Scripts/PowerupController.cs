using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Powerup { X2SCORE, HEALTHUP, TRIPLESHOT, RAPIDFIRE, BLACKHOLE }

public class PowerupController : MonoBehaviour
{
    // Game Manager Reference
    GameManager m_gameManager;
    // Player Controller Reference
    PlayerController m_playerController;
    // Score Manager Reference
    ScoreManager m_scoreManager;

    public Powerup m_powerup;
    public delegate IEnumerator PowerupBehavior();
    public PowerupBehavior Apply;

    float m_powerupDuration;

    float rotationSpeed = 12.0f;

    void Start()
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_playerController = m_gameManager.player.GetComponent<PlayerController>();
        m_scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();

        switch (m_powerup)
        {
            case Powerup.X2SCORE:
                Apply = new PowerupBehavior(x2Powerup);
                m_powerupDuration = 10.0f;
                break;
            case Powerup.HEALTHUP:
                Apply = new PowerupBehavior(healthUpPowerup);
                m_powerupDuration = 0.0f;
                break;
            case Powerup.TRIPLESHOT:
                Apply = new PowerupBehavior(tripleShotPowerup);
                m_powerupDuration = 20.0f;
                break;
            case Powerup.RAPIDFIRE:
                Apply = new PowerupBehavior(rapidFirePowerup);
                m_powerupDuration = 10.0f;
                break;
            case Powerup.BLACKHOLE:
                Apply = new PowerupBehavior(blackHolePowerup);
                m_powerupDuration = 0.0f;
                break;
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;

        transform.Rotate(Vector3.up * rotationSpeed * dt);
    }

    IEnumerator x2Powerup()
    {
        m_scoreManager.m_scoreMultiplier += 2;

        yield return new WaitForSecondsRealtime(m_powerupDuration);

        m_scoreManager.m_scoreMultiplier = 1;
    }
    IEnumerator healthUpPowerup()
    {
        m_playerController.HealPlayer();

        yield return new WaitForSecondsRealtime(m_powerupDuration);
    }
    IEnumerator tripleShotPowerup()
    {
        m_playerController.tripleShot = true;

        yield return new WaitForSecondsRealtime(m_powerupDuration);

        m_playerController.tripleShot = false;
    }
    IEnumerator rapidFirePowerup()
    {
        m_playerController.lasercooldown = m_playerController.m_laserCooldownDefault;
        m_playerController.m_laserSpeed = 120.0f;

        yield return new WaitForSecondsRealtime(m_powerupDuration);

        m_playerController.m_laserSpeed = m_playerController.m_laserSpeedDefault;
    }
    IEnumerator blackHolePowerup()
    {
        m_playerController.blackHoleCount++;

        yield return new WaitForSecondsRealtime(m_powerupDuration);
    }
}
