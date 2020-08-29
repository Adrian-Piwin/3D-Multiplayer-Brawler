using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AR_PlayerControllerScript : MonoBehaviour
{
    [Header("Movement Settings")]
    public bool isRagdoll = false;
    public float ragdollUpTimer = 2f;
    public float rotationSpeed = 10;
    public float speed = 6f;
    public float jumpforce = 5;
    public float jumpDelay = 0.5f;

    [Header("References")]
    public Transform groundTransform;
    public Transform hand;

    private Rigidbody body;
    private Animator animator;
    private bool isGrounded = true;
    private bool isHandEmpty = true;
    private GameObject currentWeapon;

    void Start(){
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update(){
        if (!isRagdoll) jumpInput();

        if (Input.GetKeyDown(KeyCode.E) && !isHandEmpty){
            dropWeapon();
        }

        if (Input.GetKeyDown(KeyCode.R)){
            isRagdoll = !isRagdoll;
            enableRagdoll(false, ragdollUpTimer);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isRagdoll){
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            float horizontalRaw = Input.GetAxisRaw("Horizontal");
            float verticalRaw = Input.GetAxisRaw("Vertical");

            Vector3 input = new Vector3(horizontal, 0, vertical);
            Vector3 inputRaw = new Vector3(horizontalRaw, 0, verticalRaw);

            // If moving set animation
            if (horizontal != 0 || vertical != 0)
                animator.SetBool("isWalking", true);
            else
                animator.SetBool("isWalking", false);
            
            movementInput(horizontal, vertical);
            rotateInput(input, inputRaw);

            if (inputRaw.sqrMagnitude == 0){
                //aimAtMouse();
            }
        }

        isOnGround();
    }

    // Movement
    private void movementInput(float horizontal, float vertical){
        body.velocity = new Vector3 (horizontal * speed, body.velocity.y, vertical * speed);

        if(body.velocity.magnitude > speed){
            body.velocity = body.velocity.normalized * speed;
        }
    }

    // Jump
    private void jumpInput(){
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded){
            // Jump animation
            if (animator.GetBool("isWalking"))
                animator.Play("Walking_Jump");
            else
                animator.Play("Idle_Jump");

            body.AddForce(new Vector3(0, jumpforce*100, 0), ForceMode.Impulse);
            isGrounded = false;
        }
    }

    // Rotation
    private void rotateInput(Vector3 input, Vector3 inputRaw){
        if (input.sqrMagnitude > 1f){
            input.Normalize();
        }

        if (inputRaw != Vector3.zero){
            Vector3 targetRotation = Quaternion.LookRotation(inputRaw).eulerAngles;
            body.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation.x, Mathf.Round(targetRotation.y / 45) * 45, targetRotation.z), Time.deltaTime * rotationSpeed);
        }
        
    }

    // Rotate player to mouse
    public void aimAtMouse(){
        //Face mouse
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
    
        if(Physics.Raycast(ray,out hit,100))
        {
            body.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (hit.point - transform.position), rotationSpeed * Time.deltaTime);
        }
    }

    // Player is touching ground
    private void isOnGround() {
        isGrounded = Physics.Raycast(groundTransform.position, Vector3.down, 0.3f);
    }

    // Put Weapon in players hand
    public void pickupWeapon(GameObject weapon){
        if (isHandEmpty){
            isHandEmpty = false;
            animator.SetBool("isHoldingGun", true);
            currentWeapon = Instantiate(weapon, hand);
        }
    }

    // Drop weapon
    public void dropWeapon(){
        currentWeapon.transform.parent = GameObject.Find("Environment").transform;
        Rigidbody rb = currentWeapon.AddComponent<Rigidbody>();
        rb.mass = 1;
        rb.AddForce(transform.forward * 120000f);
        isHandEmpty = true;
        animator.SetBool("isHoldingGun", false);
    }

    // Set player to ragdoll
    private void enableRagdoll(bool dead, float timer){
        // enable ragdoll
        if (!dead)
            StartCoroutine(ragdollUp(timer));
    }

    // Timer for ragdoll to get up
    IEnumerator ragdollUp(float upTime){
        yield return new WaitForSeconds(upTime);
        // disable ragdoll
    }

    public bool getHandState(){
        return isHandEmpty;
    }

    public bool getRagdollState(){
        return isRagdoll;
    }
}
