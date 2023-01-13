using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    public bool alive;

    //UI Elements
    public HealthBar healthBar;
    public UIPowerUpIndicator powerUpIndicator;

    public Timer timer;
    public Score score;

    //Abilities
    public bool invincible = false;
    float invincibleTime = 5.0f;
    bool invincibilityTimer = true;
    float invincibilityCooldown = 7.5f;
    public Material[] invincibilityMaterial;

    bool knockbackTimer = true;
    float knockbackCooldown = 5.0f;
    float knockbackRange = 100.0f;
    public GameObject KnockbackVisual;

    public GameObject Blood;

    // Player Stats
    float speed = 500.0f;
    float maxHealth = 100.0f;
    float health;
    public float damageBoost = 1.0f;
    float resistance = 5.0f;

    float speedTime = 5.0f;
    float damageBoostTime = 3.0f;
    float resistanceTime = 5.0f;

    bool usingPowerUp = false;
    public Material[] PowerUpIndicatorMaterials;

    public GameObject[] DirtParticles;

    bool God = false;

    Rigidbody rb;

    Ray playerLooking;
    RaycastHit hit;
    Vector3 lastHit;

    public Camera cam;

    public GameObject[] Weapons;

    bool melee;

    public GameObject Weapon;
    GameObject meleeHitbox;
    bool attack = true;

    bool disabled;

    void Start() {

        alive = true;
        disabled = false;
        gameObject.SetActive(true);
        GetComponent<Collider>().enabled = true;

        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        lastHit = transform.position;
        meleeHitbox = transform.GetChild(0).gameObject;
        
        melee = true;

        WeaponChange();
        StartCoroutine(DirtTrail());
    }

    void FixedUpdate() {

        if (alive && !cam.GetComponent<CameraController>().cutscene && !disabled) {

            // Get Mouse Position
            playerLooking = cam.ScreenPointToRay(Input.mousePosition);

            // Raycast to point at mouse position
            if (Physics.Raycast(playerLooking, out hit, 500.0f)) {

                Debug.DrawLine(transform.position, new Vector3(hit.point.x, transform.position.y, hit.point.z), Color.green);
                
                lastHit = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                transform.LookAt(lastHit);
            }
            else {
                transform.LookAt(lastHit);
            }
            
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            rb.AddForce(new Vector3(horizontal, 0, vertical) * speed);

            Weapon.transform.position = transform.position;
            Weapon.transform.rotation = transform.rotation;

            if (Vector3.Distance(transform.position, new Vector3(0, 6, 0)) > 250) {
                transform.position = new Vector3(0, 6, -20);
            }
        }
    }

    void LateUpdate() {
        if (alive && !cam.GetComponent<CameraController>().cutscene && !disabled) {
            if (Input.GetMouseButtonDown(0)) {

                if (attack) {
                    
                    if (melee) {
                        StartCoroutine(Cooldown(Weapon.GetComponent<Sword>().Cooldown));
                    
                        Weapon.GetComponent<Sword>().Swing();

                        Collider[] hitColliders = Physics.OverlapBox(meleeHitbox.transform.position, meleeHitbox.transform.localScale, meleeHitbox.transform.rotation);

                        foreach(Collider col in hitColliders) {
                            if (col.tag == "Enemy") {
                                score.AddScore(Weapon.GetComponent<Sword>().Damage * damageBoost);
                                col.GetComponent<Enemy>().TakeDamage(Weapon.GetComponent<Sword>().Damage * damageBoost);
                            } else
                            if (col.tag == "Boss") {
                                score.AddScore(Weapon.GetComponent<Sword>().Damage * damageBoost);
                                col.GetComponent<Boss>().TakeDamage(Weapon.GetComponent<Sword>().Damage * damageBoost);
                            }
                        }
                    } else {
                        StartCoroutine(Cooldown(Weapon.GetComponent<Bow>().Cooldown));

                        Weapon.GetComponent<Bow>().Draw();
                    }
                }
            }
        }
    }

    public void Disable() {
        disabled = true;
        GetComponent<Collider>().enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    void OnActivateInvincibility() {
        if (invincibilityTimer) {
            StartCoroutine(InvincibilityTimer());
        }
    }

    void OnActivateAOEKnockback() {
        if (knockbackTimer) {
            StartCoroutine(AOEKnockbackTimer());

            Collider[] AOEColliders = Physics.OverlapSphere(transform.position, knockbackRange);

            foreach(Collider col in AOEColliders) {
                if (col.tag == "Enemy") {
                    col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(4000.0f, transform.position, knockbackRange);
                    col.GetComponent<Enemy>().TakeDamage(10.0f);
                }
            }
            StartCoroutine(AOEKnockbackAnimator(Instantiate(KnockbackVisual, new Vector3(transform.position.x, transform.position.y - 6, transform.position.z), transform.rotation)));
        }
    }

    void OnWeaponChange() {
        melee = !melee;
        Destroy(Weapon);
        WeaponChange();
    }

    void WeaponChange() {
        if (melee) {
            Weapon = Instantiate(Weapons[0], transform.position, transform.rotation);
        } else {
            Weapon = Instantiate(Weapons[1], transform.position, transform.rotation);
        }
    }

    void OnGod() {
        God = !God;
    }

    public bool CallPowerUp(int powerUp) {

        if (powerUp != 3) {

            if (usingPowerUp) {
                return false;
            }

            speed = 500.0f;
            damageBoost = 1.0f;
            resistance = 5.0f;
            
        } 

        switch (powerUp) {
            case 0:
                StartCoroutine(SpeedTimer());
                return true;
            case 1:
                StartCoroutine(DamageBoostTimer());
                return true;
            case 2:
                StartCoroutine(ResistanceTimer());
                return true;
            case 3:
                health += 50;
                if (health > maxHealth) {
                    health = maxHealth;
                }
                healthBar.SetHealth(health);
                return true;
        }
        return false;
    }

    public void TakeDamage(float damage) {
        if (!God) {
            health -= damage / resistance;
            for (int i = 0; i < (damage / resistance); i++) {
                Instantiate(Blood, new Vector3((transform.position.x + UnityEngine.Random.Range(-1, 1)), transform.position.y + 2, (transform.position.z + UnityEngine.Random.Range(-1, 1))), transform.rotation);
            }
            if (health <= 0) {
                if (alive) {
                    timer.PlayerDeath();
                }
                alive = false;
                health = 0;
                Death();
            } else {
                transform.GetChild(3).GetChild(new System.Random().Next(0,2)).gameObject.GetComponent<AudioSource>().Play();
            }
        }
        healthBar.SetHealth(health);
    }

    void Death() {
        gameObject.SetActive(false);
        for (int i = 0; i < 100; i++) {
            Instantiate(Blood, new Vector3((transform.position.x + UnityEngine.Random.Range(-1, 1)), transform.position.y + 2, (transform.position.z + UnityEngine.Random.Range(-1, 1))), transform.rotation);
        }
        Destroy(Weapon);
    }

    IEnumerator InvincibilityTimer() {
        invincible = true;
        invincibilityTimer = false;
        GetComponent<Renderer>().material = invincibilityMaterial[1];
        //lock y axis
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSecondsRealtime(invincibleTime);
        invincible = false;
        GetComponent<Renderer>().material = invincibilityMaterial[0];
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 
        yield return new WaitForSecondsRealtime(invincibilityCooldown);
        invincibilityTimer = true;
    }
    IEnumerator AOEKnockbackTimer() {
        knockbackTimer = false;
        yield return new WaitForSecondsRealtime(knockbackCooldown);
        knockbackTimer = true;
    }
    IEnumerator AOEKnockbackAnimator(GameObject AOEKnockbackVisual) {

        AOEKnockbackVisual.transform.GetChild(0).gameObject.GetComponent<Animation>().Play();

        float alpha = 1.0f;
        while (alpha > 0) {
            for (int i = 0; i < AOEKnockbackVisual.transform.GetChild(0).childCount; i++) {
                AOEKnockbackVisual.transform.GetChild(0).GetChild(i).gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1, alpha);
            }
            alpha -= 0.025f;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(AOEKnockbackVisual);
    }
    
    IEnumerator SpeedTimer() {
        usingPowerUp = true;
        transform.GetChild(1).GetComponent<Renderer>().material = PowerUpIndicatorMaterials[0];
        transform.GetChild(1).gameObject.SetActive(true);
        powerUpIndicator.SetIndicatorActive(0);
        speed = speed * 2;
        yield return new WaitForSecondsRealtime(speedTime);
        speed = 500.0f;
        transform.GetChild(1).gameObject.SetActive(false);
        powerUpIndicator.SetIndicatorInactive(0);
        usingPowerUp = false;
    }
    IEnumerator DamageBoostTimer() {
        usingPowerUp = true;
        transform.GetChild(1).GetComponent<Renderer>().material = PowerUpIndicatorMaterials[1];
        transform.GetChild(1).gameObject.SetActive(true);
        powerUpIndicator.SetIndicatorActive(1);
        damageBoost = damageBoost * 2;
        yield return new WaitForSecondsRealtime(damageBoostTime);
        damageBoost = 1.0f;
        transform.GetChild(1).gameObject.SetActive(false);
        powerUpIndicator.SetIndicatorInactive(1);
        usingPowerUp = false;
    }
    IEnumerator ResistanceTimer() {
        usingPowerUp = true;
        transform.GetChild(1).GetComponent<Renderer>().material = PowerUpIndicatorMaterials[2];
        transform.GetChild(1).gameObject.SetActive(true);
        powerUpIndicator.SetIndicatorActive(2);
        resistance = resistance * 2;
        yield return new WaitForSecondsRealtime(resistanceTime);
        resistance = 5.0f;
        transform.GetChild(1).gameObject.SetActive(false);
        powerUpIndicator.SetIndicatorInactive(2);
        usingPowerUp = false;
    }

    IEnumerator Cooldown(float cooldown) {
        attack = false;
        yield return new WaitForSecondsRealtime(cooldown);
        attack = true;
    }

    IEnumerator DirtTrail() {
        while (alive) {
            Vector3 dirtPlacement = new Vector3((transform.position - transform.right * UnityEngine.Random.Range(-2, 2)).x, transform.position.y - 5, (transform.position - transform.right * UnityEngine.Random.Range(-2, 2)).z);
            Instantiate(DirtParticles[UnityEngine.Random.Range(0, DirtParticles.Length)], dirtPlacement, transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }
    }
}