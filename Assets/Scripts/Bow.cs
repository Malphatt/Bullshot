using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bow : MonoBehaviour {

    public float Damage = 20.0f;
    public float Cooldown = 0.75f;

    public GameObject arrowAmmo;
    GameObject Arrow;
    bool loaded = false;
    bool fromPlayer;

    Camera cam;
    Ray targetRay;
    RaycastHit targetHit;

    GameObject Player;

    void Awake() {

        Player = GameObject.Find("Player");

        if (Vector3.Distance(transform.position, Player.transform.position) < 5.0f) {
            fromPlayer = true;
        } else {
            fromPlayer = false;
        }

        cam = Player.GetComponent<Player>().cam;
        
    }

    void Update() {
        if (loaded) {
            if (!Arrow.GetComponent<Arrow>().Shot){
                Arrow.transform.position = transform.position;

                float forwardDistFromArrow;
                float angle = 0.0f;

                targetRay = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(targetRay, out targetHit, 500.0f)) {
                    if (fromPlayer) {
                        forwardDistFromArrow = Vector3.Distance(transform.position, new Vector3(targetHit.point.x, transform.position.y, targetHit.point.z)) - 2.5f;
                        angle = Mathf.Atan((3.0f / forwardDistFromArrow)) * Mathf.Rad2Deg;

                        if (angle > 30.0f) {
                            angle = angle / 2;
                        }
                    }
                }
                if (!fromPlayer) {
                    forwardDistFromArrow = Vector3.Distance(transform.position, new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z)) - 2.5f;
                    angle = Mathf.Atan((3.0f / forwardDistFromArrow)) * Mathf.Rad2Deg;
                }

                Arrow.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - angle, transform.rotation.eulerAngles.z);
            }
        }
    }

    public void Draw() {
        Arrow = Instantiate(arrowAmmo, transform.position, transform.rotation);
        loaded = true;
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animation>().Play("Bow Draw Upper");
        transform.GetChild(0).GetChild(9).gameObject.GetComponent<Animation>().Play("Bow Draw Lower");
        Arrow.GetComponent<Arrow>().Draw();
        Invoke("Release", 0.5f);
    }

    void Release() {
        loaded = false;
        gameObject.GetComponent<AudioSource>().Play();
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animation>().Play("Bow Shoot Upper");
        transform.GetChild(0).GetChild(9).gameObject.GetComponent<Animation>().Play("Bow Shoot Lower");
        Arrow.GetComponent<Arrow>().Shoot();
    }

    void OnDestroy() {
        Destroy(Arrow);
    }
}
