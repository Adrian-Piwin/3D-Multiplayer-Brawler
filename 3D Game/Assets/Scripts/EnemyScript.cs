using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Settings")]
    public float health = 50f;
    public float ragdollUpTimer = 2f;

    [Header("References")]
    public CapsuleCollider bodyCap;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void takeDamage(float amount){
        health -= amount;
        if (health <= 0f){
            die();
        }
    }

    public void die(){
        enableRagdoll(true);
    }

    private void enableRagdoll(bool dead){
        rb.constraints = RigidbodyConstraints.None;
        bodyCap.enabled = false;
        if (!dead)
        StartCoroutine(ragdollUp());
    }

    // Timer for ragdoll to get up
    IEnumerator ragdollUp(){
        yield return new WaitForSeconds(ragdollUpTimer);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        bodyCap.enabled = true;
    }


}
