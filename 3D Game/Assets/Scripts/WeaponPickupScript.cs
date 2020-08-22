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
        if (col.gameObject.layer == 9 || col.gameObject.layer == 10){

            GameObject root = col.gameObject;
            while (root.tag != "Player_Main"){
                root = root.transform.parent.gameObject;
            }

            PlayerController controller = root.gameObject.GetComponent<PlayerController>();
            if (controller.getHandState()){
                controller.pickupWeapon(weaponPrefab);
                Destroy(gameObject);
            }
        }
    } 
}
