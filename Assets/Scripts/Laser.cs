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
    }


    void Update()
    {
        lineRenderer.SetPosition(0, from);  
        lineRenderer.SetPosition(1, to);

        var o = gameObject;
        var position = o.transform.position;
        from = position;
        to = o.transform.forward * 3f + position;

        transform.position += transform.forward * speed * Time.deltaTime;
        total += Time.deltaTime;
        if (total >= 5.0f)
        {
            Destroy(gameObject);
        }
    }
}

