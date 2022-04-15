using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    // Game Manager Reference
    GameManager m_gameManager;

    // Game space bounds, center, and radius
    [SerializeField] GameObject m_gameSpace;
    Vector3 m_gameSpaceCenter;
    float m_gameSpaceRadius;
    // Reference to player's mini-map object
    GameObject m_miniMapPlayer;
    // Mini-map radii
    float m_miniMapInnerRadius;
    float m_miniMapOuterRadius;

    // List of enemies in the game
    EnemyController[] m_gameEnemies;
    [SerializeField] Transform m_spawnPosition;
    // List of enemies in the mini-map
    List<GameObject> m_miniMapEnemies;

    // Enemy indicator
    [SerializeField] GameObject m_enemyIndicator;
    float startWidth = 0.02f;
    Color startColor = Color.cyan;
    Color endColor = Color.red;

    void Start()
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        SphereCollider bounds = m_gameSpace.GetComponent<SphereCollider>();
        m_gameSpaceCenter = bounds.center;
        m_gameSpaceRadius = bounds.radius;

        m_miniMapInnerRadius = 0.1f;
        m_miniMapOuterRadius = 0.2f;

        m_miniMapEnemies = new List<GameObject>();
    }

    void LateUpdate()
    {
        m_gameEnemies = m_spawnPosition.GetComponentsInChildren<EnemyController>();

        // Check if an enemy has been destroyed
        if (m_miniMapEnemies.Count != m_gameEnemies.Length)
        {
            // Clear all enemies from the mini-map
            foreach (GameObject enemy in m_miniMapEnemies)
            {
                Destroy(enemy);
            }
            m_miniMapEnemies.Clear();

            // Add enemies back to the mini-map
            foreach (EnemyController enemy in m_gameEnemies)
            {
                GameObject indicator = Instantiate(m_enemyIndicator, Vector3.zero, Quaternion.identity);
                m_miniMapEnemies.Add(indicator);

                LineRenderer lineRenderer = indicator.GetComponent<LineRenderer>();
                lineRenderer.startWidth = startWidth;
                lineRenderer.endWidth = startWidth * 2;
                lineRenderer.startColor = startColor;
                lineRenderer.endColor = endColor;
            }
        }

        for (int i = 0; i < m_gameEnemies.Length; i++)
        {
            EnemyController enemy = m_gameEnemies[i];
            LineRenderer lineRenderer = m_miniMapEnemies[i].GetComponent<LineRenderer>();

            Vector3 from = transform.position + (enemy.transform.position - transform.position).normalized * m_miniMapInnerRadius;
            Vector3 to = transform.position + (enemy.transform.position - transform.position).normalized * m_miniMapOuterRadius;
            lineRenderer.SetPosition(0, from);
            lineRenderer.SetPosition(1, to);
        }
    }
}
