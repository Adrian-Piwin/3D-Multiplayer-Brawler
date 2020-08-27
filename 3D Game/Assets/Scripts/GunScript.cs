using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [Header("Settings")]
    public bool autoFire;
    public float damage = 10f;
    public float range = 10f;
    public float fireRate = 15f;
    public float impactForce = 30f;
    public float recoil = 10f;
    public bool bounce = false;
    public float bounceForce = 10;
    public int ammo = 10;
    public float speed;
	[Tooltip("From 0% to 100%")]
	public float accuracy;

    [Header("References")] 
    public GameObject bullet;
    public Transform firePoint;
    private Rigidbody playerRb;
    private Transform playerTf;
    private AR_PlayerControllerScript playerController;
    private List<Collider> ignoreColliders;
    private float nextTimeToFire = 0f;

    void Start(){
        GameObject root = gameObject;
        while (root.tag != "Player_Main"){
            root = root.transform.parent.gameObject;
        }

        playerController = root.GetComponent<AR_PlayerControllerScript>();
        playerTf = playerController.getHipsTf();
        playerRb = playerController.getHipsRb();

        int num = 5;
        Transform parentT = gameObject.transform;
        ignoreColliders = new List<Collider>();
        for (int i = 0; i < num; i++){
            ignoreColliders.Add(parentT.GetComponent<Collider>());
            parentT = parentT.parent;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire && !autoFire){
            if (!playerController.getHandState() && ammo > 0){
                nextTimeToFire = Time.time + 1f/fireRate;
                shoot();
                
                ammo -= 1;
            }
            
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && autoFire){
            if (!playerController.getHandState() && ammo > 0){
                nextTimeToFire = Time.time + 1f/fireRate;
                shoot();

                ammo -= 1;
            }
        }
    }

    // Shoot gun
    void shoot(){
        playerRb.AddForce(playerTf.forward*-(recoil)*10000);
        GameObject bulletObj = Instantiate(bullet, firePoint.position, firePoint.rotation);
        bulletObj.GetComponent<ProjectileMoveScript>().setupBullet(bounce, bounceForce, speed, accuracy, damage, impactForce, range, ignoreColliders);
    }
}
