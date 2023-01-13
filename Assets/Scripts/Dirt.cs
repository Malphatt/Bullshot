using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dirt : MonoBehaviour {

    void Awake() {
        Destroy(gameObject, 1.0f);
    }
}
