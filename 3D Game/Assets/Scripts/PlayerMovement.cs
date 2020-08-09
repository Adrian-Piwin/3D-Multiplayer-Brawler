using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 6f;

    // Update is called once per frame
    void Update()
    {
        // Movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f){
            controller.Move(direction * speed * Time.deltaTime);
        }

        // Face mouse
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
    
        if(Physics.Raycast(ray,out hit,100))
        {
            transform.LookAt(new Vector3(hit.point.x,transform.position.y,hit.point.z));
        }
    }
}
