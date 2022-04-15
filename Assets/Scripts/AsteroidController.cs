using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : AsteroidManager
{
    public int m_baseRadius = 4;
    float m_maxRadiusVariation;
    int m_baseSubdivisions = 5;

    public Vector3 m_rotationDirection;
    float m_rotationSpeed = 12.0f;

    [SerializeField] GameObject[] m_powerups;
    public bool m_hasPowerup;

    public Vector3 vel;

    void Start()
    {
        m_maxRadiusVariation = m_baseRadius / 2;
        
        FormAsteroid(Random.onUnitSphere);

        // Decide if this asteroid has a powerup
        if (Random.Range(0f, 1f) < 0.2f)
        {
            m_hasPowerup = true;
        }
        else
        {
            m_hasPowerup = false;
        }

        vel = Vector3.zero;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        transform.position += vel * dt;
        if(vel.sqrMagnitude != 0)
        {
            vel -= vel * 0.5f * dt;
        }

        transform.Rotate(m_rotationDirection * m_rotationSpeed * dt);
    }

    /// <summary>
    /// Form a single asteroid with the specified rotation direction.
    /// </summary>
    /// <param name="rotation">The direction in which to rotate this asteroid.</param>
    void FormAsteroid(Vector3 rotation)
    {
        Vector3[] vertices = new Vector3[(m_baseSubdivisions + 1) * (m_baseSubdivisions + 1)];
        int[] triangles = new int[vertices.Length * 6];
        Vector2[] uv = new Vector2[vertices.Length];

        Vector3 point;
        float theta = 2.0f * Mathf.PI / m_baseSubdivisions;
        float phi = Mathf.PI / m_baseSubdivisions;

        List<List<Vector3>> lon = new List<List<Vector3>>();
        int index = 0;
        bool northPole = false;
        bool southPole = false;
        bool firstLon = false;
        for (int i = 0; i <= m_baseSubdivisions; i += 1)
        {
            List<Vector3> lat = new List<Vector3>();
            for (int j = 0; j <= m_baseSubdivisions; j += 1)
            {
                float variation = Random.Range(0.0f, m_maxRadiusVariation);
                float magnitude = m_baseRadius + variation;

                point = new Vector3(
                    Mathf.Cos(theta * i) * Mathf.Sin(phi * j),
                    Mathf.Cos(phi * j),
                    Mathf.Sin(theta * i) * Mathf.Sin(phi * j)
                );
                point = point.normalized * magnitude;
                if (northPole && j == 0) { point = lon[0][0]; }
                if (southPole && j == m_baseSubdivisions) { point = lon[0][m_baseSubdivisions]; }
                if (firstLon && i == m_baseSubdivisions) { point = lon[0][j]; }

                lat.Add(point);

                vertices[index] = point;
                uv[index] = new Vector2(point.x / (float)m_baseSubdivisions, point.z / (float)m_baseSubdivisions);
                index += 1;

                if (j == 0) { northPole = true; }
                if (j == m_baseSubdivisions) { southPole = true; }
                if (i == 0) { firstLon = true; }
            }

            lon.Add(lat);
        }

        index = 0;
        for (int i = 0; i < lon.Count; i += 1)
        {
            for (int j = 0; j < lon[i].Count - 1; j += 1)
            {
                triangles[index++] = ((j + 1) % lon[i].Count) + (((i + 1) % lon.Count) * lon[i].Count);
                triangles[index++] = ((j + 1) % lon[i].Count) + (i * lon[i].Count);
                triangles[index++] = j + (((i + 1) % lon.Count)) * lon[i].Count;

                triangles[index++] = ((j + 1) % lon[i].Count) + (i * lon[i].Count);
                triangles[index++] = j + (i * lon[i].Count);
                triangles[index++] = j + (((i + 1) % lon.Count)) * lon[i].Count;
            }
        }

        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;

        m_rotationDirection = rotation;
    }

    public void SpawnPowerup()
    {
        int randomPowerup;
        // 10% chance for Black Hole (index 4) powerup
        // 30% chance for Health (index 0) powerup
        // 20% chance for x2 (index 1), Triple Shot (index 2), and Rapid Fire (index 3) powerups
        float percentage = Random.Range(0f, 1f);
        if (percentage >= 0.0f && percentage < 0.1f) randomPowerup = (int)Powerup.BLACKHOLE;
        else if (percentage >= 0.1f && percentage < 0.4f) randomPowerup = (int)Powerup.HEALTHUP;
        else if (percentage >= 0.4f && percentage < 0.6f) randomPowerup = (int)Powerup.X2SCORE;
        else if (percentage >= 0.6f && percentage < 0.8f) randomPowerup = (int)Powerup.TRIPLESHOT;
        else randomPowerup = (int)Powerup.RAPIDFIRE;

        GameObject.Instantiate(m_powerups[randomPowerup], transform.position, Quaternion.identity);
    }
}
