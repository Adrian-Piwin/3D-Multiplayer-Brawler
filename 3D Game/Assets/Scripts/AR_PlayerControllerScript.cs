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
    public Rigidbody hipsRb;
    public Transform hipsTf;
    public Transform hand;
    public CapsuleCollider bodyCap;

    private bool isGrounded = true;
    private bool isHandEmpty = true;
    private GameObject currentWeapon;

    void Update(){
        if (!isRagdoll) jumpInput();

        if (Input.GetKeyDown(KeyCode.E)){
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
        hipsRb.velocity = new Vector3 (horizontal * speed, hipsRb.velocity.y, vertical * speed);

        if(hipsRb.velocity.magnitude > speed){
            hipsRb.velocity = hipsRb.velocity.normalized * speed;
        }
    }

    // Jump
    private void jumpInput(){
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded){
            hipsRb.AddForce(new Vector3(0, jumpforce*100, 0), ForceMode.Impulse);
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
            hipsRb.rotation = Quaternion.Slerp(hipsTf.rotation, Quaternion.Euler(targetRotation.x, Mathf.Round(targetRotation.y / 45) * 45, targetRotation.z), Time.deltaTime * rotationSpeed);
        }
        
    }

    // Rotate player to mouse
    public void aimAtMouse(){
        //Face mouse
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
    
        if(Physics.Raycast(ray,out hit,100))
        {
            hipsRb.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (hit.point - transform.position), rotationSpeed * Time.deltaTime);
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
            currentWeapon = Instantiate(weapon, hand);
        }
    }

    // Drop weapon
    public void dropWeapon(){
        currentWeapon.transform.parent = GameObject.Find("Environment").transform;
        Rigidbody rb = currentWeapon.AddComponent<Rigidbody>();
        rb.mass = 12;
        rb.AddForce(transform.forward * 120000f);
        isHandEmpty = true;
    }

    // Set player to ragdoll
    private void enableRagdoll(bool dead, float timer){
        hipsRb.constraints = RigidbodyConstraints.None;
        bodyCap.enabled = false;
        if (!dead)
            StartCoroutine(ragdollUp(timer));
    }

    // Timer for ragdoll to get up
    IEnumerator ragdollUp(float upTime){
        yield return new WaitForSeconds(upTime);
        hipsRb.constraints = RigidbodyConstraints.FreezeRotation;
        bodyCap.enabled = true;
        isRagdoll = false;
    }

    public Transform getHipsTf(){
        return hipsTf;
    }

    public Rigidbody getHipsRb(){
        return hipsRb;
    }

    public bool getHandState(){
        return isHandEmpty;
    }

    public bool getRagdollState(){
        return isRagdoll;
    }
}
