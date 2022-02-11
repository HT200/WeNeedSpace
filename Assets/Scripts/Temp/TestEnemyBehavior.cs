using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyBehavior : MonoBehaviour
{
    int m_direction;

    // Start is called before the first frame update
    void Start()
    {
        m_direction = Random.Range(0, 4);
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_direction)
        {
            case 0:
                transform.position += new Vector3(1, 1, -1) * Time.deltaTime;
                break;
            case 1:
                transform.position += new Vector3(1, -1, -1) * Time.deltaTime;
                break;
            case 2:
                transform.position += new Vector3(-1, 1, -1) * Time.deltaTime;
                break;
            case 3:
                transform.position += new Vector3(-1, -1, -1) * Time.deltaTime;
                break;
            default:
                transform.position += new Vector3(0, 0, -1) * Time.deltaTime;
                break;
        }
        
    }
}
