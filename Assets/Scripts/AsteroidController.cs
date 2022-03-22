using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public List<Asteroid> asteroids;
    [SerializeField] private GameObject m_AsteroidBasePrefab;
    int maxAsteroids = 20;

    float bounds = 100;

    int baseRadius = 4;
    float maxRadiusVariation;
    int baseSubdivisions = 5;
    float rotationSpeed = 12.0f;

    void Start()
    {
        asteroids = new List<Asteroid>();
        maxRadiusVariation = baseRadius / 2;

        for (int k = 0; k < maxAsteroids; k += 1)
        {
            createAsteroid(Random.insideUnitSphere * bounds);
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;

        foreach (Asteroid asteroid in asteroids)
        {
            asteroid.gameObject.transform.Rotate(asteroid.rotationDirection * rotationSpeed * dt);
        }
    }

    /// <summary>
    /// Creates a single asteroid at the specified location, adds it to the list of asteroids, 
    /// and gives it a rotation direction.
    /// </summary>
    /// <returns>The asteroid that was created.</returns>
    GameObject createAsteroid(Vector3 position)
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

        GameObject asteroid = GameObject.Instantiate(m_AsteroidBasePrefab, Vector3.zero, Quaternion.identity);
        Mesh mesh = asteroid.GetComponent<MeshFilter>().mesh;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
        asteroid.transform.position = position;
        Vector3 rotationDirection = Random.onUnitSphere;

        asteroids.Add(new Asteroid(asteroid, rotationDirection));
        return asteroid;
    }
}

public class Asteroid
{
    public GameObject gameObject;
    public Vector3 rotationDirection;
    public bool hasPowerup;

    public Asteroid(GameObject gameObject, Vector3 rotationDirection)
    {
        this.gameObject = gameObject;
        this.rotationDirection = rotationDirection;
    }
}
