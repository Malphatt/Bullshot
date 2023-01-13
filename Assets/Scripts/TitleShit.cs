using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleShit : MonoBehaviour {

    void Awake() {
        Destroy(gameObject, Random.Range(0.5f, 1.5f));
    }
}
