using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    float bounds = 100;

    public int baseRadius = 4;
    float maxRadiusVariation;
    int baseSubdivisions = 5;

    public Vector3 rotationDirection;
    float rotationSpeed = 12.0f;

    [SerializeField] GameObject[] m_powerups;
    public bool hasPowerup;

    public Vector3 vel;

    void Start()
    {
        vel = Vector3.zero;
        maxRadiusVariation = baseRadius / 2;

        if (Random.Range(0f, 1f) < 0.2f)
        {
            hasPowerup = true;
        }
        else
        {
            hasPowerup = false;
        }


        CreateAsteroid(Random.insideUnitSphere * bounds, Random.onUnitSphere);
        //Ac = v^2/r = 1000/r
        // v = sqrt(1000)
        //V must be applied on the transform.right initally

        //Only activate this code if you want to see something cool (The asteroids orbiting the black hole)
        /*
        transform.forward = transform.position.normalized;
        vel = transform.right * Mathf.Sqrt(1000);
        */
    }

    void Update()
    {
        float dt = Time.deltaTime;
        transform.position += vel * dt;
        if(vel.sqrMagnitude != 0)
        {
            vel -= vel * 0.5f * dt;
        }

        transform.Rotate(rotationDirection * rotationSpeed * dt);
    }

    /// <summary>
    /// Creates a single asteroid at the specified location with the specified rotation direction.
    /// </summary>
    /// <param name="position">The position at which to spawn this asteroid.</param>
    /// <param name="rotation">The direction in which to rotate this asteroid.</param>
    void CreateAsteroid(Vector3 position, Vector3 rotation)
    {
        Vector3[] vertices = new Vector3[(baseSubdivisions + 1) * (baseSubdivisions + 1)];
        int[] triangles = new int[vertices.Length * 6];
        Vector2[] uv = new Vector2[vertices.Length];

        Vector3 point;
        float theta = 2.0f * Mathf.PI / baseSubdivisions;
        float phi = Mathf.PI / baseSubdivisions;

        List<List<Vector3>> lon = new List<List<Vector3>>();
        int index = 0;
        bool northPole = false;
        bool southPole = false;
        bool firstLon = false;
        for (int i = 0; i <= baseSubdivisions; i += 1)
        {
            List<Vector3> lat = new List<Vector3>();
            for (int j = 0; j <= baseSubdivisions; j += 1)
            {
                float variation = Random.Range(0.0f, maxRadiusVariation);
                float magnitude = baseRadius + variation;

                point = new Vector3(
                    Mathf.Cos(theta * i) * Mathf.Sin(phi * j),
                    Mathf.Cos(phi * j),
                    Mathf.Sin(theta * i) * Mathf.Sin(phi * j)
                );
                point = point.normalized * magnitude;
                if (northPole && j == 0) { point = lon[0][0]; }
                if (southPole && j == baseSubdivisions) { point = lon[0][baseSubdivisions]; }
                if (firstLon && i == baseSubdivisions) { point = lon[0][j]; }

                lat.Add(point);

                vertices[index] = point;
                uv[index] = new Vector2(point.x / (float)baseSubdivisions, point.z / (float)baseSubdivisions);
                index += 1;

                if (j == 0) { northPole = true; }
                if (j == baseSubdivisions) { southPole = true; }
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

        transform.position = position;
        rotationDirection = rotation;
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
