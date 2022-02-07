using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLaserFire : MonoBehaviour
{
    // Start is called before the first frame update
    float total;
    void Start()
    {
        total = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + transform.forward * 20f * Time.deltaTime;
        total += Time.deltaTime;
        if(total >= 5.0f)
        {
            Destroy(gameObject);
        }
    }
}
