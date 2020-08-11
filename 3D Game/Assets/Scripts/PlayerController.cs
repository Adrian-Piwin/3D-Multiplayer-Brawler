using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    private bool isGrounded = true;
    private bool allowedJump = true;

    Vector3 targetRotation;

    public float rotationSpeed = 10;
    public float speed = 6f;
    public float jumpforce = 5;
    public float jumpDelay = 0.5f;
    public Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update(){
        jumpInput();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float horizontalRaw = Input.GetAxisRaw("Horizontal");
        float verticalRaw = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(horizontal, 0, vertical);
        Vector3 inputRaw = new Vector3(horizontalRaw, 0, verticalRaw);
        
        movementInput(horizontal, vertical);
        rotateInput(input, inputRaw);

        // Disable and enable animation on movement
        if (inputRaw.sqrMagnitude != 0){
            anim.SetBool("isStopped", false);
        }else if (inputRaw.sqrMagnitude == 0){
            anim.SetBool("isStopped", true);
        }

        // Freeze legs when jumping
        if (!isGrounded){
            anim.enabled = false;
        }else if (isGrounded)
        {
            anim.enabled = true;
        }
    }

    // Movement
    private void movementInput(float horizontal, float vertical){
        rb.velocity = new Vector3 (horizontal * speed, rb.velocity.y, vertical * speed);

        if(rb.velocity.magnitude > speed){
            rb.velocity = rb.velocity.normalized * speed;
        }
    }

    // Jump
    private void jumpInput(){
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && allowedJump){
            rb.AddForce(new Vector3(0, jumpforce*100, 0), ForceMode.Impulse);
            isGrounded = false;
            allowedJump = false;
        }
    }

    IEnumerator allowJump(){
        yield return new WaitForSeconds(jumpDelay);
        allowedJump = true;
    }

    // Rotation
    private void rotateInput(Vector3 input, Vector3 inputRaw){
        
        if (input.sqrMagnitude > 1f){
            input.Normalize();
        }

        if (inputRaw != Vector3.zero){
            targetRotation = Quaternion.LookRotation(input).eulerAngles;
        }

        rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation.x, Mathf.Round(targetRotation.y / 45) * 45, targetRotation.z), Time.deltaTime * rotationSpeed);
    }

    // Ground collision
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Ground")){
            isGrounded = true;
            if (!allowedJump){
                StartCoroutine(allowJump());
            }
        }
    }
}
