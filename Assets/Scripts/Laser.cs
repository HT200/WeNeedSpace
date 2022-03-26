using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private int segments = 1;  //just drawing a line segment, so only need two endpoints

    public float width;

    public Color color;

    LineRenderer lineRenderer;

    public Vector3 to;

    public Vector3 from;

    public float speed;

    float total;

    public Vector3 vel;

    void Start()
    {
        total = 0.0f;

        color = Color.green;

        width = .2f;
  
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = (segments + 1);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.startColor = color;
        lineRenderer.endColor = lineRenderer.startColor;
        lineRenderer.useWorldSpace = true;

        from = Vector3.zero;
        to = Vector3.zero;

        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);

        vel = transform.forward * speed;
    }


    void Update()
    {
        lineRenderer.SetPosition(0, from);  
        lineRenderer.SetPosition(1, to);

        //This is one of the newer additions, it makes it so the black hole can simply alter the vel vector and the laser shot will rotate it self with that automatically
        transform.forward = vel.normalized;
        //This is so that its not moving in its original direction should it break out of the black hole's grip
        //Image firing a bullet around the black hole, with the old code, it wouldve been physically pulled towards the hole, but instead of firing out in a different direction
        //it wouldve stayed going on its transform.forward vector, which would've been unchanged



        //This looks a little wonky but makes whats happening very explicitly clear
        var o = gameObject;
        var position = o.transform.position;
        from = position;
        to = o.transform.forward * 3f + position;



        transform.position += vel * Time.deltaTime;
        total += Time.deltaTime;
        if (total >= 5.0f)
        {
            Destroy(gameObject);
        }
    }
}

