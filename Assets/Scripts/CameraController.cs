using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject player;

    public bool cutscene;

    Vector3 offset;
    Quaternion rotation;

    void Start() {
        offset = transform.position - player.transform.position;
        rotation = transform.rotation;
        cutscene = false;
    }

    void LateUpdate() {
        if (!cutscene) {
            transform.position = player.transform.position + offset;
            transform.rotation = rotation;
        }
    }
    public void StartCutscene() {
        StartCoroutine(Cutscene());
    }
    IEnumerator Cutscene() {
        cutscene = true;
        gameObject.GetComponent<Animation>().Play("Cutscene");
        yield return new WaitForSeconds(2.0f);
        GameObject.Find("Timer").GetComponent<Timer>().BossFightStart();
        yield return new WaitForSeconds(3.0f);
        gameObject.GetComponent<Animation>().Stop("Cutscene");
        GameObject.Find("Spawner").GetComponent<Spawner>().SpawnedBoss.GetComponent<Boss>().CutsceneEnd();
        GameObject.Find("Timer").GetComponent<Timer>().CutsceneEnd();
        cutscene = false;
    }
}
