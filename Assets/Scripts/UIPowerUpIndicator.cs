using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPowerUpIndicator : MonoBehaviour {

    public GameObject Speed;
    public Sprite[] SpeedSprites;
    bool speedActive = false;

    public GameObject SpeedTimer;

    public GameObject DamageBoost;
    public Sprite[] DamageBoostSprites;
    bool damageBoostActive = false;

    public GameObject DamageBoostTimer;
    
    public GameObject Resistance;
    public Sprite[] ResistanceSprites;
    bool resistanceActive = false;

    public GameObject ResistanceTimer;

    float speedTimer = 5.0f;
    float damageBoostTimer = 3.0f;
    float resistanceTimer = 5.0f;

    float timeLeft;

    bool StartSpeedTimer = false;
    bool StartDamageBoostTimer = false;
    bool StartResistanceTimer = false;

    public void SetIndicatorActive(int powerUp) {
        
        if (powerUp == 0) {
                speedActive = true;
                StartSpeedTimer = true;
                timeLeft = speedTimer;
        } else if (powerUp == 1) {
                damageBoostActive = true;
                StartDamageBoostTimer = true;
                timeLeft = damageBoostTimer;
        } else if (powerUp == 2) {
                resistanceActive = true;
                StartResistanceTimer = true;
                timeLeft = resistanceTimer;
        }

        UpdateUI();
    }

    public void SetIndicatorInactive(int powerUp) {
        
        if (powerUp == 0) {
                speedActive = false;
        } else if (powerUp == 1) {
                damageBoostActive = false;
        } else if (powerUp == 2) {
                resistanceActive = false;
        }
    
        UpdateUI();
    }

    void UpdateUI() {

        SpeedTimer.SetActive(speedActive);
        if (speedActive) {
            Speed.GetComponent<Image>().sprite = SpeedSprites[1];
        } else {
            Speed.GetComponent<Image>().sprite = SpeedSprites[0];
        }

        DamageBoostTimer.SetActive(damageBoostActive);
        if (damageBoostActive) {
            DamageBoost.GetComponent<Image>().sprite = DamageBoostSprites[1];
        } else {
            DamageBoost.GetComponent<Image>().sprite = DamageBoostSprites[0];
        }

        ResistanceTimer.SetActive(resistanceActive);
        if (resistanceActive) {
            Resistance.GetComponent<Image>().sprite = ResistanceSprites[1];
        } else {
            Resistance.GetComponent<Image>().sprite = ResistanceSprites[0];
        }
    }

    void Update() {
        if (StartSpeedTimer) {
            if (timeLeft >= 0.0f) {
                timeLeft -= Time.deltaTime;
                SpeedTimer.GetComponent<Text>().text = timeLeft.ToString("F1");
            } else {
                StartSpeedTimer = false;
            }
        }
        if (StartDamageBoostTimer) {
            if (timeLeft >= 0.0f) {
                timeLeft -= Time.deltaTime;
                DamageBoostTimer.GetComponent<Text>().text = timeLeft.ToString("F1");
            } else {
                StartDamageBoostTimer = false;
            }
        }
        if (StartResistanceTimer) {
            if (timeLeft >= 0.0f) {
                timeLeft -= Time.deltaTime;
                ResistanceTimer.GetComponent<Text>().text = timeLeft.ToString("F1");
            } else {
                StartResistanceTimer = false;
            }
        }
    }
}
