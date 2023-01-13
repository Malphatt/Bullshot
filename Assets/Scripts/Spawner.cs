using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject PowerUp;

    float EnemySpawnRate;
    public GameObject Enemy;
    public GameObject[] EnemySpawnLocations;

    public Timer timer;

    bool bossFight = false;
    public GameObject Boss;
    public GameObject SpawnedBoss;

    public GameObject Player;

    void Start() {

        EnemySpawnRate = 5.5f;

        Invoke("StartTimer", 1.0f);
        Invoke("SpawnEnemy", 0);
    }

    void StartTimer() {
        timer.StartTimer();
    }

    public void SpawnPowerUp(float xPosition, float zPosition) {
        if (Random.Range(0, 99) < 10) {
            Instantiate(PowerUp, new Vector3(xPosition, 6, zPosition), transform.rotation);
        }
    }

    void SpawnEnemy() {
            
        if (EnemySpawnRate > 0.5f) {
            EnemySpawnRate -= 0.5f;
        }

        GameObject EnemySpawnLocation = EnemySpawnLocations[new System.Random().Next(0, 8)];

        float xPosition = EnemySpawnLocation.transform.position.x;
        float zPosition = EnemySpawnLocation.transform.position.z;

        if(!bossFight && Player.GetComponent<Player>().alive) {
            Instantiate(Enemy, new Vector3(xPosition, 6, zPosition), transform.rotation);
            Invoke("SpawnEnemy", EnemySpawnRate);
        }
    }

    public void BossFightStart() {
        bossFight = true;
        CancelInvoke("SpawnEnemy");

        //remove all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) {
            Destroy(enemy);
        }

        // //remove all powerups
        // GameObject[] powerups = GameObject.FindGameObjectsWithTag("Power Up");
        // foreach (GameObject powerup in powerups) {
        //     Destroy(powerup);
        // }

        //remove all weapons
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
        foreach (GameObject weapon in weapons) {
            if (weapon != Player.GetComponent<Player>().Weapon) {
                Destroy(weapon);
            }
        }

        Player.transform.position = new Vector3(0, 6, -200);

        SpawnedBoss = Instantiate(Boss, new Vector3(0, 8.5f, 0), transform.rotation);
        timer.BossAlive = true;

    }

    public void BossFightEnd() {
        GameObject[] powerups = GameObject.FindGameObjectsWithTag("Power Up");
        foreach (GameObject powerup in powerups) {
            Destroy(powerup);
        }
    }
}
