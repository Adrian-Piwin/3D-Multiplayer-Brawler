using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollDeath : MonoBehaviour
{

    public Animator animator = null;
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;


    // Start is called before the first frame update
    void Start()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        ToggleRagdoll(false);
    }

    private void ToggleRagdoll(bool state){
        animator.enabled = !state;

        foreach(Rigidbody rb in ragdollBodies){
            rb.isKinematic = !state;
        }

        foreach(Collider collder in ragdollColliders){
            collder.enabled = state;
        }
    }
}
