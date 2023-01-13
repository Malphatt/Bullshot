using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    float speed = 350.0f;
    float meleeRange = 10.0f;
    float rangedDistance = 50.0f;
    public float health = 100.0f;

    Rigidbody rb;

    Ray enemyLooking;
    RaycastHit hit;
    Vector3 lastHit;
    Vector3 offset;

    GameObject player;

    public GameObject[] Weapons;

    bool melee;

    GameObject Weapon;
    GameObject meleeHitbox;
    bool inRange = false;
    bool attack = true;

    public GameObject Blood;
    public GameObject[] DirtParticles;

    bool alive;
    bool moving = false;
    bool disabled;

    void Awake() {
        
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        alive = true;
        disabled = false;

        offset = player.transform.position - transform.position;
        lastHit = transform.position;

        meleeHitbox = transform.GetChild(1).gameObject;

        // randomize mellie or ranged
        melee = new System.Random().Next(0,2) == 0;

        if (melee) {
            health = 100.0f;
            Weapon = Instantiate(Weapons[0], transform.position, transform.rotation);
        } else {
            health = 50.0f;
            Weapon = Instantiate(Weapons[1], transform.position, transform.rotation);
        }

        StartCoroutine(DirtTrail());

    }

    void FixedUpdate() {
        if (player.GetComponent<Player>().alive) {
            if (alive && !disabled) {
                enemyLooking = new Ray(transform.position, player.transform.position - transform.position);

                // Raycast to point at player position
                if (Physics.Raycast(enemyLooking, out hit, 500.0f)) {

                    transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
                    // Debug.DrawLine(transform.position, new Vector3(hit.point.x, transform.position.y, hit.point.z), Color.red);
                    
                    // Debug.Log(hit.collider.tag);
                }

                Vector3 distVector = player.transform.position - transform.position;

                if (melee) {
                    
                    if (distVector.x*distVector.x + distVector.z*distVector.z > meleeRange*meleeRange) {
                        inRange = false;
                        moving = true;

                        rb.AddForce(transform.forward * speed);
                        distVector = player.transform.position - transform.position;
                    } else {
                        inRange = true;
                        moving = false;
                    }
                } else {
                    if (distVector.x*distVector.x + distVector.z*distVector.z > rangedDistance*rangedDistance) {
                        inRange = false;
                        moving = true;

                        rb.AddForce(transform.forward * speed);
                        distVector = player.transform.position - transform.position;
                    } else {
                        inRange = true;
                        moving = false;
                    }
                }
                
                if (inRange && attack) {
                    if (melee) {

                        StartCoroutine(Cooldown(Weapon.GetComponent<Sword>().Cooldown));
                        Weapon.GetComponent<Sword>().Swing();

                        Collider[] hitColliders = Physics.OverlapBox(meleeHitbox.transform.position, meleeHitbox.transform.localScale/2, meleeHitbox.transform.rotation);

                        foreach(Collider col in hitColliders) {
                            if (col.tag == "Player") {
                                col.GetComponent<Player>().TakeDamage(Weapon.GetComponent<Sword>().Damage);
                            }
                        }

                    } else {
                        StartCoroutine(Cooldown(Weapon.GetComponent<Bow>().Cooldown));

                        Weapon.GetComponent<Bow>().Draw();
                    }
                }

                Weapon.transform.position = transform.position;
                Weapon.transform.rotation = transform.rotation;

            }
        }
    }

    public void Disable() {
        disabled = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void OnDestroy() {
        GameObject.Find("Spawner").GetComponent<Spawner>().SpawnPowerUp(transform.position.x, transform.position.z);
    }

    public void TakeDamage(float damage) {
        health -= damage;
        if (health <= 0) {
            transform.GetChild(0).GetChild(1).GetChild(new System.Random().Next(0,2)).gameObject.GetComponent<AudioSource>().Play();
            for (int i = 0; i < damage; i++) {
                Instantiate(Blood, new Vector3((transform.position.x + Random.Range(-1, 1)), transform.position.y + 2, (transform.position.z + Random.Range(-1, 1))), transform.rotation);
            }
            alive = false;
            Destroy(Weapon, 0.5f);
            Destroy(gameObject, 0.5f);
        } else {
            for (int i = 0; i < damage / 4; i++) {
                Instantiate(Blood, new Vector3((transform.position.x + Random.Range(-1, 1)), transform.position.y + 2, (transform.position.z + Random.Range(-1, 1))), transform.rotation);
            }
            transform.GetChild(0).GetChild(0).GetChild(new System.Random().Next(0,3)).gameObject.GetComponent<AudioSource>().Play();
        }
    }

    IEnumerator Cooldown(float time) {
        attack = false;
        yield return new WaitForSeconds(time);
        attack = true;
    }
    IEnumerator DirtTrail() {
        while (alive && player.GetComponent<Player>().alive) {
            if (moving) {
                Vector3 dirtPlacement = new Vector3((transform.position - transform.right * UnityEngine.Random.Range(-2, 2)).x, transform.position.y - 5, (transform.position - transform.right * UnityEngine.Random.Range(-2, 2)).z);
                Instantiate(DirtParticles[Random.Range(0, DirtParticles.Length)], dirtPlacement, transform.rotation);
                yield return new WaitForSeconds(0.1f);
            } else {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
