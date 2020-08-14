using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public bool isRagdoll = false;
    public float ragdollUpTimer = 2f;
    public float rotationSpeed = 10;
    public float speed = 6f;
    public float jumpforce = 5;
    public float jumpDelay = 0.5f;

    [Header("Hand Settings")]
    public int handAngleMin;
    public int handAngleMax;

    [Header("References")]
    public int playerNumber;
    public Animator anim;
    public CapsuleCollider bodyCap;
    public HingeJoint[] armHingeJoints;
    public Texture2D shootCursor;
    public Rigidbody handRb;
    public Transform hand;

    private Rigidbody hipsRb;
    private Vector3 targetRotation;
    private bool isGrounded = true;
    private bool allowedJump = true;
    private bool isHandEmpty = true;
    private GameObject currentWeapon;

    void Start(){
        hipsRb = GetComponent<Rigidbody>();
    }

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

            // Disable and enable animation on movement
            if (inputRaw.sqrMagnitude != 0){
                anim.SetBool("isStopped", false);
            }else if (inputRaw.sqrMagnitude == 0){
                anim.SetBool("isStopped", true);
                aimAtMouse();
            }

            // Freeze legs when jumping
            if (!isGrounded){
                anim.enabled = false;
            }else if (isGrounded)
            {
                anim.enabled = true;
            }
            
        }
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && allowedJump){
            hipsRb.AddForce(new Vector3(0, jumpforce*100, 0), ForceMode.Impulse);
            isGrounded = false;
            allowedJump = false;
        }
    }

    // Jump delay
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
            targetRotation = Quaternion.LookRotation(inputRaw).eulerAngles;
            hipsRb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation.x, Mathf.Round(targetRotation.y / 45) * 45, targetRotation.z), Time.deltaTime * rotationSpeed);
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

    // Ground collision && resistance when pushing player
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Ground")){
            isGrounded = true;
            if (!allowedJump){
                StartCoroutine(allowJump());
            }
        }

    }

    public void playerDied(){
        enableRagdoll(true, 0f);
    }

    // Put Weapon in players hand
    public void pickupWeapon(GameObject weapon){
        if (isHandEmpty){
            isHandEmpty = false;
            currentWeapon = Instantiate(weapon, hand);

            graspHand(true);
            handMovement(true);
        }
    }

    // Drop weapon
    public void dropWeapon(){
        currentWeapon.transform.parent = GameObject.Find("Environment").transform;
        Rigidbody rb = currentWeapon.AddComponent<Rigidbody>();
        rb.mass = 12;
        rb.AddForce(transform.forward * 120000f);
        isHandEmpty = true;

        graspHand(false);
        handMovement(false);
    }

    // Hand movement
    private void handMovement(bool up){
        IEnumerator coroutine;
        if (up){

            JointLimits limits = armHingeJoints[0].limits;

            limits.min = 75;
            limits.max = 76;
            coroutine = changeLimits(armHingeJoints[0], limits);
            StartCoroutine(coroutine);
            limits.min = 89;
            limits.max = 90;
            coroutine = changeLimits(armHingeJoints[1], limits);
            StartCoroutine(coroutine);

            limits.min = 0;
            limits.max = 0;
            coroutine = changeLimits(armHingeJoints[2], limits);
            StartCoroutine(coroutine);
            coroutine = changeLimits(armHingeJoints[3], limits);
            StartCoroutine(coroutine);
        }else{
            JointLimits limits = armHingeJoints[0].limits;

            limits.min = -40;
            limits.max = 60;
            armHingeJoints[0].limits = limits;
            limits.min = -70;
            limits.max = 90;
            armHingeJoints[1].limits = limits;

            limits.min = -90;
            limits.max = 90;
            armHingeJoints[2].limits = limits;
            limits.min = 30;
            limits.max = 120;
            armHingeJoints[3].limits = limits;
        }
    }

    IEnumerator changeLimits(HingeJoint joint, JointLimits newLimit){
        float speed = 0.001f;
        float step = 1f;

        JointLimits newLimitDiff = joint.limits;
        while (joint.limits.min != newLimit.min || joint.limits.max != newLimit.max){
            yield return new WaitForSeconds(speed);

            if (joint.limits.min != newLimit.min){
                if (joint.limits.min < newLimit.min)
                    newLimitDiff.min += step;
                else newLimitDiff.min -= step;

                joint.limits = newLimitDiff;
            }

            if (joint.limits.max != newLimit.max){
                if (joint.limits.max < newLimit.max)
                    newLimitDiff.max += step;
                else newLimitDiff.max -= step;

                joint.limits = newLimitDiff;
            }

        }
    }

    // Set player to ragdoll
    private void enableRagdoll(bool dead, float timer){
        hipsRb.constraints = RigidbodyConstraints.None;
        bodyCap.enabled = false;
        anim.enabled = false;
        if (!dead)
            StartCoroutine(ragdollUp(timer));
    }

    // Timer for ragdoll to get up
    IEnumerator ragdollUp(float upTime){
        yield return new WaitForSeconds(upTime);
        hipsRb.constraints = RigidbodyConstraints.FreezeRotation;
        bodyCap.enabled = true;
        anim.enabled = true;
        isRagdoll = false;
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

    public bool getHandState(){
        return isHandEmpty;
    }

    public bool getRagdollState(){
        return isRagdoll;
    }
}
