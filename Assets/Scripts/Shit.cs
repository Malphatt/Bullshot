using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shit : MonoBehaviour {

    float launchSpeed = 5000.0f;
    Rigidbody rb;

    public GameObject ShitStain;
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * launchSpeed);
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Player") {
            collision.collider.gameObject.GetComponent<Player>().TakeDamage(5.0f);
            Destroy(gameObject);
        } else
        if (collision.collider.tag == "Boss" || collision.collider.tag == "Shit") {
        } else
        if (collision.collider.name == "Floor") {
            Instantiate(ShitStain, new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), Quaternion.identity);
            Destroy(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
