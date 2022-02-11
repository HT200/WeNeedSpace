using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempController : MonoBehaviour
{
    public float moveSpeed = 4.0f;

    // Temporary movement controls to test bullet impact location
    void Update()
    {
        float fHorizontal = Input.GetAxis("Horizontal");
        float fVertical = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(fHorizontal * Time.deltaTime * moveSpeed, 0, fVertical * Time.deltaTime * moveSpeed), Space.Self);
    }
}
