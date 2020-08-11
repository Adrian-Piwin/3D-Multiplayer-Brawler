using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject bodyObj;
    public Rigidbody body;

    public float speed = 6f;

    // Update is called once per frame
    void Update()
    {
        /*// Face mouse
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
    
        if(Physics.Raycast(ray,out hit,100))
        {
            bodyObj.transform.LookAt(new Vector3(hit.point.x,transform.position.y,hit.point.z));
        }*/
    }

    void FixedUpdate() {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        body.velocity = new Vector3 (horizontal * speed, body.velocity.y, vertical * speed);
    }
}
