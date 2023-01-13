using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour {

    float score;
    public GameObject EndScore;
    public GameObject ScoreText;

    void Start() {
        EndScore.SetActive(false);
        score = 0;
        GetComponent<Text>().text = "Score: " + score;
    }

    public void AddScore(float score) {
        this.score += score;
        GetComponent<Text>().text = "Score: " + this.score;
    }

    public float GetScore() {
        return score;
    }

    public void ShowScore(int endType) {
        string text = "";
        if (endType == 0) { // player died

            float time = 60.0f - GameObject.Find("Timer").GetComponent<Timer>().GetTime()[0];
            if (GameObject.Find("Timer").GetComponent<Timer>().GetTime()[1] == 1) {
                time = (60.0f - GameObject.Find("Timer").GetComponent<Timer>().GetTime()[0]) + 60.0f;
            }

            text = "You died! Your score was: " + score + "! and you survived for " + time.ToString("F3") + " seconds!";
        } else
        if (endType == 1) { // time ran out
            text = "Time ran out! Your score was: " + score + "!";
        } else
        if (endType == 2) { // boss died
            text = "You killed the boss in " + (60.0f - GameObject.Find("Timer").GetComponent<Timer>().GetTime()[0]).ToString("F3") + " seconds! Your score was: " + score + "!";
        }
        StartCoroutine(TitleScreen(text));
    }
    IEnumerator TitleScreen(string scoreText) {
        GameObject.Find("Player").GetComponent<Player>().Disable();
        if (GameObject.Find("Spawner").GetComponent<Spawner>().SpawnedBoss != null) {
            GameObject.Find("Spawner").GetComponent<Spawner>().SpawnedBoss.GetComponent<Boss>().over = true;
        }
        yield return new WaitForSeconds(5.0f);
        ScoreText.GetComponent<Text>().text = scoreText;
        EndScore.SetActive(true);
    }
    public void GoToTitleScreen() {
        SceneManager.LoadScene("Title Screen");
    }
}
