using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    float bounds = 100;

    int baseRadius = 4;
    float maxRadiusVariation;
    int baseSubdivisions = 5;

    public Vector3 rotationDirection;
    float rotationSpeed = 12.0f;

    public bool hasPowerup;

    void Start()
    {
        maxRadiusVariation = baseRadius / 2;
        hasPowerup = false;

        createAsteroid(Random.insideUnitSphere * bounds, Random.onUnitSphere);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        transform.Rotate(rotationDirection * rotationSpeed * dt);
    }

    /// <summary>
    /// Creates a single asteroid at the specified location with the specified rotation direction.
    /// </summary>
    /// <param name="position">The position at which to spawn this asteroid.</param>
    /// <param name="rotation">The direction in which to rotate this asteroid.</param>
    void createAsteroid(Vector3 position, Vector3 rotation)
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
}
