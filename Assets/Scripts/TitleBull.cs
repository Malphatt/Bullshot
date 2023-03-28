using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBull : MonoBehaviour {
    
    public bool title;
    public GameObject Shit;

    void Awake() {
        title = true;
        StartCoroutine(FireShit());
    }

    IEnumerator FireShit() {
        while (title) {
            yield return new WaitForSeconds(0.0f);
            GameObject shit = Instantiate(Shit, transform.GetChild(0).position, transform.GetChild(0).rotation);
            shit.GetComponent<Rigidbody>().AddForce((-transform.forward + (transform.right * Random.Range(-1.125f, 1.125f))) * 500);
        }
    }
}
