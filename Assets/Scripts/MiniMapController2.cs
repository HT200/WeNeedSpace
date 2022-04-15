using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController2 : MonoBehaviour
{
    // Game Manager Reference
    GameManager m_gameManager;

    // Game Space Bounds, Center, and Radius
    [SerializeField] GameObject m_gameSpace;
    Vector3 m_gameSpaceCenter;
    float m_gameSpaceRadius;
    // Reference to player's mini-map object
    GameObject m_miniMapPlayer;
    // Mini-map radius
    float m_miniMapRadius = 0.2f;

    // List of enemies in the game
    EnemyController[] m_gameEnemies;
    [SerializeField] private Transform m_spawnPosition;
    // List of enemies in the mini-map
    List<GameObject> m_miniMapEnemies;

    // Prefabs
    [SerializeField] GameObject m_playerMiniMapPrefab;
    [SerializeField] GameObject m_enemyMiniMapPrefab;

    void Start()
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        SphereCollider bounds = m_gameSpace.GetComponent<SphereCollider>();
        m_gameSpaceCenter = bounds.center;
        m_gameSpaceRadius = bounds.radius;

        m_miniMapPlayer = Instantiate(m_playerMiniMapPrefab, transform.position, Quaternion.identity);
        m_miniMapEnemies = new List<GameObject>();
    }

    void LateUpdate()
    {
        // Move player on mini-map
        Vector3 playerPosition = m_gameManager.player.transform.position;

        float percentage = Vector3.Distance(m_gameSpaceCenter, playerPosition) / m_gameSpaceRadius;
        Vector3 position = transform.position + playerPosition.normalized * m_miniMapRadius * percentage;

        m_miniMapPlayer.transform.position = position;

        // Move enemies on mini-map
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
                m_miniMapEnemies.Add(Instantiate(m_enemyMiniMapPrefab, transform.position, Quaternion.identity));
            }
        }

        for (int i = 0; i < m_miniMapEnemies.Count; i++)
        {
            Vector3 enemyPosition = m_gameEnemies[i].transform.position;

            percentage = Vector3.Distance(m_gameSpaceCenter, enemyPosition) / m_gameSpaceRadius;
            position = transform.position + enemyPosition.normalized * m_miniMapRadius * percentage;

            m_miniMapEnemies[i].transform.position = position;
        }
    }
}
