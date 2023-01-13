using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShitStain : MonoBehaviour {

    float alpha;
    Color colour;
    
    void Awake() {
        float radius = Random.Range(2, 5);
        transform.localScale = new Vector3(radius, 0.1f, radius);
        colour = GetComponent<Renderer>().material.color;
        alpha = GetComponent<Renderer>().material.color.a;
        StartCoroutine(Fade());
    }

    IEnumerator Fade() {
        while (alpha > 0) {
            alpha -= 0.01f;
            colour.a = alpha;
            GetComponent<Renderer>().material.color = colour;
            yield return new WaitForSeconds(0.25f);
        }
        Destroy(gameObject);
    }
}
