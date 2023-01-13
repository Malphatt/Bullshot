using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    public float Damage;
    public float Cooldown;

    void Awake() {
        Damage = 40.0f;
        Cooldown = 0.5f;
    }

    public void Swing() {
        transform.GetChild(1).GetChild(new System.Random().Next(0, 3)).gameObject.GetComponent<AudioSource>().Play();
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animation>().Play("Sword Swing");
    }
}
