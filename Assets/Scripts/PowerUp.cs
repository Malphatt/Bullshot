using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    int type;

    public Material[] materials;

    void Awake() {
        
        type = new System.Random().Next(0, 4);

        //Change Particle Colour to match PowerUp
        GameObject PowerUpParticles = transform.GetChild(0).GetChild(1).gameObject;
        
        for(int i = 0; i < PowerUpParticles.transform.childCount; i++) {
            PowerUpParticles.transform.GetChild(i).GetChild(0).GetComponent<MeshRenderer>().material = materials[type];
        }

    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if (other.gameObject.GetComponent<Player>().CallPowerUp(type)) {
                gameObject.GetComponent<Animation>().Play("Power Up Collect");
                transform.GetChild(1).GetChild(new System.Random().Next(0, 4)).gameObject.GetComponent<AudioSource>().Play();
                Destroy(this.gameObject, 0.5f);
            }
        }
    }
}
