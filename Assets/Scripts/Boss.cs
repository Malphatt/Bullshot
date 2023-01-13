using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {

    float health;
    float resistance = 1.0f;

    bool fireShit;
    public GameObject Shit;
    bool fire = false;
    float fireRate = 0.1f;
    int fireAmount = 10;
    float reloadTime = 1.0f;

    bool berserkMode;
    bool keepAttack;
    bool taunt;
    bool charge;
    bool returnToCenter;
    float berserkMinRate = 2.5f;
    float berserkMaxRate = 4.0f;
    float berserkChargeDistance = 200.0f;
    float berserkChargeSpeed = 10.0f;

    public Material normalMaterial;
    public Material chargeMaterial;


    public GameObject[] Legs;

    public GameObject ShitParticles;
    public GameObject BullHair;
    public GameObject BullHorns;

    public GameObject[] DirtParticles;
    public GameObject DirtLocation;

    public GameObject Blood;

    GameObject Player;
    Timer timer;

    public GameObject ChargeArea;

    bool cutscene;
    public bool over;

    Score score;

    void Awake() {

        Player = GameObject.Find("Player");
        timer = GameObject.Find("Timer").GetComponent<Timer>();
        score = GameObject.Find("Score").GetComponent<Score>();

        health = 750.0f;

        cutscene = true;
        fireShit = true;
        StartCoroutine(FireShit());

        berserkMode = false;
        keepAttack = false;

        taunt = false;
        charge = false;
        returnToCenter = false;

        over = false;
    }

    void FixedUpdate() {
        if (!cutscene && !over) {
            if (!berserkMode) {
                transform.LookAt(new Vector3(-Player.transform.position.x, transform.position.y, -Player.transform.position.z));
                fire = true;
            } else {
                if (!charge && !taunt && !returnToCenter) {
                    transform.LookAt(new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z));
                } else
                if (charge && !taunt) {
                    if ((transform.position.x * transform.position.x) + (transform.position.z * transform.position.z) < berserkChargeDistance * berserkChargeDistance) {
                        transform.position += transform.forward * berserkChargeSpeed;
                    }
                } else
                if (returnToCenter && !charge) {
                    if ((transform.position.x * transform.position.x) + (transform.position.z * transform.position.z) > 10.0f) {
                        transform.position -= transform.forward * berserkChargeSpeed * 0.75f;
                    } else {
                        transform.position = new Vector3(0, transform.position.y, 0);
                    }
                }
                fire = false;
            }
            if (keepAttack) {
                StartCoroutine(BerserkAttack());
            }
        }
    }

    public void CutsceneEnd() {
        cutscene = false;
    }

    public void TakeDamage(float damage) {

        health -= damage / resistance;

        for (int i = 0; i < damage / resistance; i++) {
            Instantiate(Blood, new Vector3((transform.position.x + Random.Range(-5, 5)), transform.position.y, (transform.position.z + Random.Range(-5, 5))), transform.rotation);
        }

        if (health <= 100.0f && !berserkMode) {
            health = 100.0f;
            resistance = 2.0f;
            berserkMode = true;
            keepAttack = true;
            StartCoroutine(BerserkAttack());
        } else
        if (health <= 0) {

            health = 0;
            keepAttack = false;
            StopCoroutine(BerserkAttack());

            timer.BossAlive = false;

            for (int i = 0; i < 100; i++) {
                Instantiate(ShitParticles, new Vector3((transform.position.x + Random.Range(-5, 5)), transform.position.y, (transform.position.z + Random.Range(-5, 5))), transform.rotation);
            }
            for (int i = 0; i < 1000; i++) {
                Instantiate(BullHair, new Vector3((transform.position.x + Random.Range(-5, 5)), transform.position.y, (transform.position.z + Random.Range(-5, 5))), transform.rotation);
            }

            score.AddScore(500);
            Instantiate(BullHorns, transform.position, transform.rotation);
            Instantiate(BullHorns, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    void ChangeHornColour(bool bullCharge) {

        Transform leftHorn = transform.GetChild(1).GetChild(0);
        Transform rightHorn = transform.GetChild(1).GetChild(1);

        if (bullCharge) {

            leftHorn.GetComponent<Renderer>().material = chargeMaterial;
            rightHorn.GetComponent<Renderer>().material = chargeMaterial;

            for (int i = 0; i < leftHorn.childCount; i++) {
                leftHorn.GetChild(i).GetComponent<Renderer>().material = chargeMaterial;
                rightHorn.GetChild(i).GetComponent<Renderer>().material = chargeMaterial;
            }

        } else {

            leftHorn.GetComponent<Renderer>().material = normalMaterial;
            rightHorn.GetComponent<Renderer>().material = normalMaterial;

            for (int i = 0; i < leftHorn.childCount; i++) {
                leftHorn.GetChild(i).GetComponent<Renderer>().material = normalMaterial;
                rightHorn.GetChild(i).GetComponent<Renderer>().material = normalMaterial;
            }
            
        }
        
    }

    IEnumerator FireShit() {
        while (fireShit) {
            if (!over) {
                if (fire) {
                    int i = 0;
                    while (i < fireAmount) {
                        i += 1;
                        Instantiate(Shit, transform.GetChild(0).position, transform.GetChild(0).rotation);
                        yield return new WaitForSeconds(fireRate);
                    }
                }
                yield return new WaitForSeconds(reloadTime);
            } else {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    IEnumerator BerserkAttack() {
        if (!over) {
            keepAttack = false;
            yield return new WaitForSeconds(Random.Range(berserkMinRate, berserkMaxRate));
            taunt = true;
            Legs[1].transform.GetChild(0).GetComponent<Animation>().Play("Bull Taunt");
            GameObject chargeArea = Instantiate(ChargeArea, transform.position, transform.rotation);
            StartCoroutine(BlinkWarning(chargeArea));
            StartCoroutine(SpawnDirt());
            ChangeHornColour(true);
            yield return new WaitForSeconds(2.0f);
            charge = true;
            taunt = false;
            chargeArea.GetComponent<ChargeArea>().Damage();
            yield return new WaitForSeconds(1.5f);
            returnToCenter = true;
            charge = false;
            Destroy(chargeArea);
            yield return new WaitForSeconds(0.75f);
            returnToCenter = false;
            ChangeHornColour(false);
            keepAttack = true;
        }
    }
    IEnumerator BlinkWarning(GameObject chargeArea) {
        chargeArea.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        chargeArea.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        chargeArea.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        chargeArea.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        chargeArea.SetActive(true);
    }
    IEnumerator SpawnDirt() {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 5; i++) {
            SpawnDirtParticle();
        }
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < 5; i++) {
            SpawnDirtParticle();
        }
    }
    void SpawnDirtParticle() {
        GameObject Dirt = Instantiate(DirtParticles[Random.Range(0, DirtParticles.Length)], (DirtLocation.transform.position + (transform.right * Random.Range(-5, 5)) + (transform.forward * Random.Range(-2, 2))), DirtLocation.transform.rotation);
        for (int i = 0; i < Dirt.transform.childCount; i++) {
            Dirt.transform.GetChild(i).GetComponent<Rigidbody>().AddForce((Dirt.transform.forward + new Vector3(0, 0.1f, 0)) * 1000.0f);
        }
    }
}
