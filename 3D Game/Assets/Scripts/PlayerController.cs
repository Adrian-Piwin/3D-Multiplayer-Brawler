using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    private bool isGrounded = true;

    Vector3 targetRotation;

    public float rotationSpeed = 10;
    public float moveSpeed = 100;
    public float Jumpforce = 5;
    public Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded){
            rb.AddForce(new Vector3(0, Jumpforce*100, 0), ForceMode.Impulse);
            isGrounded = false;
        }
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

        if (input.sqrMagnitude > 1f){
            input.Normalize();
        }

        if (inputRaw != Vector3.zero){
            targetRotation = Quaternion.LookRotation(input).eulerAngles;
        }

        rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation.x, Mathf.Round(targetRotation.y / 45) * 45, targetRotation.z), Time.deltaTime * rotationSpeed);

        if (inputRaw.sqrMagnitude != 0){
            anim.enabled = true;
        }else if (inputRaw.sqrMagnitude == 0){
            anim.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Ground")){
            isGrounded = true;
        }
    }
}
