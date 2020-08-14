using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupScript : MonoBehaviour
{
    public GameObject weaponDisplay;
    public GameObject weaponPrefab;

    void Start(){
        Instantiate(weaponDisplay, new Vector3(0,0,0), Quaternion.identity, gameObject.transform);
        gameObject.transform.GetChild(0).transform.localPosition = Vector3.zero;
    }

    void OnTriggerEnter (Collider col){
        if (col.gameObject.layer == 9 || col.gameObject.layer == 10){

            GameObject root = col.gameObject;
            while (root.tag != "Player"){
                root = root.transform.parent.gameObject;
            }

            PlayerController controller = root.gameObject.GetComponent<PlayerController>();
            if (!(controller.getHandState())){
                controller.pickupWeapon(weaponPrefab);
                Destroy(gameObject);
            }
        }
    } 
}
