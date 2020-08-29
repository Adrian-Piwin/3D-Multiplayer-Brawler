using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightPlus;

public class WeaponPickupScript : MonoBehaviour
{
    public GameObject weaponDisplay;
    public GameObject weaponPrefab;
    public bool spawnIn = true;

    private Animator anim;

    void Start(){
        Instantiate(weaponDisplay, new Vector3(0,0,0), Quaternion.identity, gameObject.transform);
        gameObject.transform.GetChild(0).transform.localPosition = Vector3.zero;

        anim = gameObject.GetComponent<Animator>();
        anim.SetBool("isSpawned", spawnIn);

    }

    void OnTriggerEnter (Collider col){
        if (col.gameObject.layer == 17 && col.gameObject.tag == "Player_Main"){

            AR_PlayerControllerScript controller = col.gameObject.GetComponent<AR_PlayerControllerScript>();
            if (controller.getHandState()){
                controller.pickupWeapon(weaponPrefab);
                Destroy(gameObject);
            }
        }
    } 
}
