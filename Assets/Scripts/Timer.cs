using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    float timerLength = 60.000f;
    float timeLeft = 0.000f;
    bool startTimer = false;

    public GameObject Task;
    public Spawner spawner;
    public Score score;

    bool bossTime;
    public bool BossAlive;
    public Camera cam;

    void Start() {
        GetComponent<Text>().text = timeLeft.ToString("F3");
        bossTime = false;
    }

    public void StartTimer() {
        startTimer = true;
        timeLeft = timerLength;
        GetComponent<Text>().text = timerLength.ToString("F3");
        Task.GetComponent<Text>().text = "SURVIVE!";
        Task.GetComponent<Text>().color = Color.white;
    }

    void Update() {
        if (startTimer) {
            if (timeLeft > 0) {
                timeLeft -= Time.deltaTime;
                GetComponent<Text>().text = timeLeft.ToString("F3");
            } else {
                timeLeft = 0.000f;
                GetComponent<Text>().text = timeLeft.ToString("F3");
                startTimer = false;

                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies) {
                    enemy.GetComponent<Enemy>().Disable();
                }


                cam.GetComponent<CameraController>().StartCutscene();
            }
        }
        if (bossTime) {
            Task.GetComponent<Text>().text = "KILL THE BOSS!";
            Task.GetComponent<Text>().color = Color.red;
            
            if (BossAlive) {
                if (timeLeft > 0) {
                    timeLeft -= Time.deltaTime;
                    GetComponent<Text>().text = timeLeft.ToString("F3");
                } else {
                    timeLeft = 0.000f;
                    GetComponent<Text>().text = timeLeft.ToString("F3");
                    bossTime = false;
                    score.ShowScore(1);
                }
            } else {
                bossTime = false;
                spawner.BossFightEnd();
                Task.GetComponent<Text>().text = "YOU WIN!";
                Task.GetComponent<Text>().color = Color.green;
                score.ShowScore(2);
            }
        }
    }
    public void PlayerDeath() {
        startTimer = false;
        bossTime = false;
        score.ShowScore(0);
    }
    public float[] GetTime() {
        float[] returnTime = new float[2];
        returnTime[0] = timeLeft;
        if (BossAlive)
            returnTime[1] = 1;
        else
            returnTime[1] = 0;
        return returnTime;
    }
    public void BossFightStart() {
        GameObject.Find("Spawner").GetComponent<Spawner>().BossFightStart();
    }
    public void CutsceneEnd() {
        bossTime = true;
        timeLeft = timerLength;
    }
}
