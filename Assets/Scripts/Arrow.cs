using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    float speed = 10000.0f;
    float damage = 25.0f;

    Score score;

    GameObject Player;
    bool fromPlayer = false;
    Rigidbody rb;
    public bool Shot = false;
    bool hit = false;

    void Awake() {
        
        Player = GameObject.Find("Player");
        score = GameObject.Find("Score").GetComponent<Score>();
        rb = gameObject.GetComponent<Rigidbody>();

        if (Vector3.Distance(transform.position, Player.transform.position) < 1.0f) {
            fromPlayer = true;
        }
    }

    public void Draw() {
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animation>().Play("Arrow Draw");

    }

    public void Shoot() {
        transform.GetChild(0).gameObject.GetComponent<Collider>().enabled = true;
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animation>().Play("Arrow Fly");
        rb.AddForce(transform.forward * speed);
        Shot = true;
        StartCoroutine(DestroyIfNotHit());
    }

    void OnCollisionEnter(Collision collision) {

    //Arrows Were gonna hit the player of enemy and stick into them but kept randomly spinning on collision and after so removed it
        // if (collision.collider.tag == "Player" || collision.collider.tag == "Enemy") {

        //     //Freeze arrow in place
        //     transform.GetChild(0).gameObject.GetComponent<Collider>().enabled = false;
        //     rb.constraints = RigidbodyConstraints.FreezeAll;

        //     entityHit = collision.collider.gameObject;
        //     hitEntity = true;
            
        //     rb.velocity = Vector3.zero;
        //     rb.angularVelocity = Vector3.zero;

        //     transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animation>().Play("Arrow Hit");

        //     if (collision.collider.tag == "Player") {
        //         transform.SetParent(entityHit.transform.GetChild(2));
        //     } else
        //     if (collision.collider.tag == "Enemy") {
        //         transform.SetParent(entityHit.transform.GetChild(2));
        //     }
        float bonusDamage = damage;
        if (fromPlayer) {
            bonusDamage = damage * Player.GetComponent<Player>().damageBoost;
        }


        if (collision.collider.tag == "Player") {
            if (!Player.GetComponent<Player>().invincible) {
                collision.collider.gameObject.GetComponent<Player>().TakeDamage(damage);
                Destroy(gameObject);
            }
        } else
        if (collision.collider.tag == "Enemy") {
            if (fromPlayer) {
                score.AddScore(bonusDamage);
            }
            collision.collider.gameObject.GetComponent<Enemy>().TakeDamage(bonusDamage);
            Destroy(gameObject);
        } else
        if (collision.collider.tag == "Boss") {
            if (fromPlayer) {
                score.AddScore(bonusDamage);
            }
            collision.collider.gameObject.GetComponent<Boss>().TakeDamage(bonusDamage);
            Destroy(gameObject);
        } else
         if (collision.collider.name == "Wall") {
            hit = true;
            transform.GetChild(0).gameObject.GetComponent<Collider>().enabled = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animation>().Play("Arrow Hit");
        }
    }
    IEnumerator DestroyIfNotHit() {
        yield return new WaitForSeconds(3.0f);
        if (!hit) {
            Destroy(gameObject);
        }
    }
}
