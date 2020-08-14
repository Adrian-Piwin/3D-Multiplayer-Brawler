using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;

    [Header("References")]
    public Transform groundCheck;
    public CharacterController controller;
    public LayerMask groundMask;
    public Transform hand;
    public Animator anim;
    public RagdollToggle ragdollToggleScript;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isRagdoll = false;
    private bool isHandHolding = false;

    // Update is called once per frame
    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0){
            velocity.y = -2f;
        }

        // Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal * -1 + transform.forward * vertical * -1;

        controller.Move(move * speed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded){
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Hand
        if (Input.GetKeyDown(KeyCode.E)){
            isHandHolding = !isHandHolding;
            graspHand(isHandHolding);
        }

        // Ragdoll
        if (Input.GetKeyDown(KeyCode.R)){
            ragdollToggleScript.toggleRagdoll(isRagdoll);
            isRagdoll = !isRagdoll;
        }

        // Animation
        if (horizontal == 1){
            anim.SetBool("isWalking", true);
        }else if (horizontal == -1){
            anim.SetBool("isWalking", true);
        }else if (vertical == 1){
            anim.SetBool("isWalking", true);
        }else if (vertical == -1){
            anim.SetBool("isWalking", true);
        }else{
            anim.SetBool("isWalking", false);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        
        /* Face mouse
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
    
        if(Physics.Raycast(ray,out hit,100))
        {
            bodyObj.transform.LookAt(new Vector3(hit.point.x,transform.position.y,hit.point.z));
        }*/


    }

    private void graspHand(bool grasp){
        if (grasp){
            hand.GetChild(0).rotation = Quaternion.Slerp(hand.GetChild(0).rotation, Quaternion.Euler(-3.48f, 80.22f, -76.68f),  Time.deltaTime * speed);
            hand.GetChild(0).GetChild(0).rotation = Quaternion.Slerp(hand.GetChild(0).GetChild(0).rotation, Quaternion.Euler(0.02f, 13.61f, -85.11f),  Time.deltaTime * speed);
            hand.GetChild(1).rotation = Quaternion.Slerp(hand.GetChild(1).rotation, Quaternion.Euler(21.89f, 77.756f, -85.48f),  Time.deltaTime * speed);
            hand.GetChild(1).GetChild(0).rotation = Quaternion.Slerp(hand.GetChild(1).GetChild(0).rotation, Quaternion.Euler(57.87f, -97.84f, -95.84f),  Time.deltaTime * speed);
            hand.GetChild(2).rotation = Quaternion.Slerp(hand.GetChild(2).rotation, Quaternion.Euler(69.53f, -58.81f, -179.10f),  Time.deltaTime * speed);
            hand.GetChild(2).GetChild(0).rotation = Quaternion.Slerp(hand.GetChild(2).GetChild(0).rotation, Quaternion.Euler(-48.51f, 58.79f, -87.263f),  Time.deltaTime * speed);
        }else{
            hand.GetChild(0).rotation = Quaternion.Slerp(hand.GetChild(0).rotation, Quaternion.Euler(9.4f, 73.5f, -27.4f),  Time.deltaTime * speed);
            hand.GetChild(0).GetChild(0).rotation = Quaternion.Slerp(hand.GetChild(0).GetChild(0).rotation, Quaternion.Euler(-0.179f, 14f, -1.7f),  Time.deltaTime * speed);
            hand.GetChild(1).rotation = Quaternion.Slerp(hand.GetChild(1).rotation, Quaternion.Euler(11.6f, 106.2f, -28.7f),  Time.deltaTime * speed);
            hand.GetChild(1).GetChild(0).rotation = Quaternion.Slerp(hand.GetChild(1).GetChild(0).rotation, Quaternion.Euler(1.9f, 12.1f, -2.5f),  Time.deltaTime * speed);
            hand.GetChild(2).rotation = Quaternion.Slerp( hand.GetChild(2).rotation, Quaternion.Euler(44.3f, 122f, 17.8f),  Time.deltaTime * speed);
            hand.GetChild(2).GetChild(0).rotation = Quaternion.Slerp(hand.GetChild(2).GetChild(0).rotation, Quaternion.Euler(-1.6f, 5.1f, -7.7f),  Time.deltaTime * speed);
        }
    }

}
