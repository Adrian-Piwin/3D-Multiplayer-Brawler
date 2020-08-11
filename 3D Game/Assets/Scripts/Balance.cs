using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balance : MonoBehaviour
{
    public GameObject body;

    // Update is called once per frame
    void Update()
    {
        transform.position = body.transform.position;
    }
}
