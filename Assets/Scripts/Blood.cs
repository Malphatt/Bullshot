using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour {
    void Awake() {
        Destroy(gameObject, Random.Range(3.0f, 5.0f));
    }
}
