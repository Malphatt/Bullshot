using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeArea : MonoBehaviour {

    float damage = 200.0f;

    public void Damage() {
        Collider[] colliders = Physics.OverlapBox(transform.GetChild(0).position, transform.GetChild(0).localScale, transform.rotation);
        foreach (Collider collider in colliders) {
            if (collider.tag == "Player") {
                collider.GetComponent<Player>().TakeDamage(damage);
            }
        }
    }
}
