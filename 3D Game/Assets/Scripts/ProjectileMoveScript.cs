//
//NOTES:
//This script is used for DEMONSTRATION porpuses of the Projectiles. I recommend everyone to create their own code for their own projects.
//This is just a basic example.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMoveScript : MonoBehaviour {

	public GameObject muzzlePrefab;
	public GameObject hitPrefab;
	public AudioClip shotSFX;
	public AudioClip hitSFX;
	public List<GameObject> trails;

    private Vector3 startPos;
	private float speedRandomness;
	private Vector3 offset;
	private bool collided;
	private Rigidbody rb;
	private bool bounce;
	private float bounceForce;
	private float speed;
	private float accuracy;
	private float damage;
	private float impactForce;
	private float range;
	private List<Collider> collidersIgnore;

	void CreateBullet () {
        startPos = transform.position;
        rb = GetComponent <Rigidbody> ();

		for (int i = 0; i < collidersIgnore.Count; i++)
			Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), collidersIgnore[i]);

		//used to create a radius for the accuracy and have a very unique randomness
		if (accuracy != 100) {
			accuracy = 1 - (accuracy / 100);

			for (int i = 0; i < 2; i++) {
				var val = 1 * Random.Range (-accuracy, accuracy);
				var index = Random.Range (0, 2);
				if (i == 0) {
					if (index == 0)
						offset = new Vector3 (0, -val, 0);
					else
						offset = new Vector3 (0, val, 0);
				} else {
					if (index == 0)
						offset = new Vector3 (0, offset.y, -val);
					else
						offset = new Vector3 (0, offset.y, val);
				}
			}
		}
			
		if (muzzlePrefab != null) {
			var muzzleVFX = Instantiate (muzzlePrefab, transform.position, Quaternion.identity);
			muzzleVFX.transform.forward = gameObject.transform.forward + offset;
			var ps = muzzleVFX.GetComponent<ParticleSystem>();
			if (ps != null)
				Destroy (muzzleVFX, ps.main.duration);
			else {
				var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
				Destroy (muzzleVFX, psChild.main.duration);
			}
		}

		if (shotSFX != null && GetComponent<AudioSource>()) {
			GetComponent<AudioSource> ().PlayOneShot (shotSFX);
		}

		StartCoroutine(DestroyParticle(range));
	}

	public void setupBullet(bool bnc, float bncForce, float spd, float acr, float dmg, float impForce, float rng, List<Collider> ignoreCols){
		bounce = bnc;
		bounceForce = bncForce;
		speed = spd;
		accuracy = acr;
		damage = dmg;
		impactForce = impForce;
		range = rng;
		collidersIgnore = ignoreCols;

		CreateBullet();
	}

	void FixedUpdate () {
        if (speed != 0 && rb != null)
			// Shoot only on x and z
			rb.AddForce((new Vector3(gameObject.transform.forward.x, 0, gameObject.transform.forward.z))*(speed)); 
    }

	void OnCollisionEnter (Collision co) {
        if (!bounce)
        {
            if (co.gameObject.tag != "Bullet" && !collided)
            {
                collided = true;

				Debug.Log(co.transform.name);
				// Find main body 
				if (co.collider.gameObject.layer == 9 || co.collider.gameObject.layer == 10){
					Transform hitPlayer = co.transform.root.GetChild(1).GetChild(0);

					EnemyScript enemy = hitPlayer.GetComponent<EnemyScript>();
					if (enemy != null){
						enemy.takeDamage(damage);
					}
				}

                if (shotSFX != null && GetComponent<AudioSource>())
                {
                    GetComponent<AudioSource>().PlayOneShot(hitSFX);
                }

                if (trails.Count > 0)
                {
                    for (int i = 0; i < trails.Count; i++)
                    {
                        trails[i].transform.parent = null;
                        var ps = trails[i].GetComponent<ParticleSystem>();
                        if (ps != null)
                        {
                            ps.Stop();
                            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                        }
                    }
                }

                speed = 0;
                GetComponent<Rigidbody>().isKinematic = true;

                ContactPoint contact = co.contacts[0];
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
                Vector3 pos = contact.point;

                if (hitPrefab != null)
                {
                    var hitVFX = Instantiate(hitPrefab, pos, rot) as GameObject;

                    var ps = hitVFX.GetComponent<ParticleSystem>();
                    if (ps == null)
                    {
                        var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                        Destroy(hitVFX, psChild.main.duration);
                    }
                    else
                        Destroy(hitVFX, ps.main.duration);
                }

                StartCoroutine(DestroyParticle(0f));
            }
        }
        else
        {
            rb.useGravity = true;
            rb.drag = 0.5f;
            ContactPoint contact = co.contacts[0];
            rb.AddForce (Vector3.Reflect((contact.point - startPos).normalized, contact.normal) * bounceForce, ForceMode.Impulse);
            Destroy ( this );
        }
	}

	public IEnumerator DestroyParticle (float waitTime) {
		yield return new WaitForSeconds (waitTime);
		Destroy (gameObject);
	}

	private Transform FindParentWithTag(Transform t, string tag){
		while (t.parent != null)
		{
			if (t.parent.tag == tag)
				{
					return t.parent;
				}
			t = t.parent;
		}
		return null; // Could not find a parent with given tag.
	}

}
