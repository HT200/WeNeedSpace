using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
    // Game Manager Reference
    [SerializeField] protected GameManager m_gameManager;

    float m_gameSpaceRadius;

    // Asteroid belt variables
    // The asteroid belt can be visually represented as a torus
    protected Vector3 m_asteroidBeltCenter;
    protected float m_asteroidBeltInnerRadius = 600f;
    protected float m_asteroidBeltOuterRadius = 1000f;
    protected float m_asteroidBeltHeightRadius;
    float m_orbitSpeed = 0.2f;

    // Asteroid variables
    [SerializeField] private AsteroidController m_AsteroidBasePrefab;
    private int m_maxAsteroids = 600;

    void Start()
    {
        m_gameSpaceRadius = GameObject.Find("GameSpace").GetComponent<SphereCollider>().radius;
        m_asteroidBeltCenter = new Vector3(0, 0, -(m_asteroidBeltInnerRadius + m_asteroidBeltOuterRadius) / 2);
        m_asteroidBeltHeightRadius = m_gameSpaceRadius / 2;
        transform.position = m_asteroidBeltCenter;

        for (int i = 0; i < m_maxAsteroids; i++)
        {
            m_gameManager.asteroidList.Add(SpawnAsteroid());
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;
        transform.Rotate(new Vector3(0, m_orbitSpeed * dt, 0));

        if (transform.childCount < m_maxAsteroids)
        {
            m_gameManager.asteroidList.Add(SpawnAsteroid(true));
        }
    }

    /// <summary>
    /// Spawn a single asteroid within the asteroid belt.
    /// </summary>
    /// <param name="refill">Whether or not this function is respawning a destroyed asteroid.</param>
    /// <returns>A reference to the spawned asteroid</returns>
    AsteroidController SpawnAsteroid(bool refill = false)
    {
        AsteroidController asteroid = Instantiate(m_AsteroidBasePrefab, transform);

        Vector3 position = Random.onUnitSphere * Random.Range(m_asteroidBeltInnerRadius, m_asteroidBeltOuterRadius);
        if (refill)
        {
            // Ensure the asteroid is spawned on the opposite side of the asteroid belt from the game space
            while (position.y <= -m_asteroidBeltHeightRadius || position.y >= m_asteroidBeltHeightRadius
                || position.z > m_asteroidBeltCenter.z)
            {
                position = Random.onUnitSphere * Random.Range(m_asteroidBeltInnerRadius, m_asteroidBeltOuterRadius);
            }
        }
        else
        {
            while (position.y <= -m_asteroidBeltHeightRadius || position.y >= m_asteroidBeltHeightRadius)
            {
                position = Random.onUnitSphere * Random.Range(m_asteroidBeltInnerRadius, m_asteroidBeltOuterRadius);
            }
        }
        asteroid.transform.position = position + m_asteroidBeltCenter;

        return asteroid;
    }
}
