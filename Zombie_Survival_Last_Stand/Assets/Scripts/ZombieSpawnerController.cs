using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZombieSpawnerController : MonoBehaviour
{
    public int initialZombiesPerWave = 5;
    public int currentZombiesPerWave;

    public float spawnDelay = 0.5f; //Delay between spawning each zombie in a wave

    public int currentWave = 0;
    public float waveCoolDown = 10.0f; //Time between waves

    public bool inCoolDown;
    public float cooldownCounter = 0;   //Only for testing and UI

    public List<EnemyScript> currentZombiesAlive;

    public GameObject zombiePrefab;

    public TextMeshProUGUI waveOverUI;
    public TextMeshProUGUI cooldownTimerUI;
    public TextMeshProUGUI currentWaveNumberUI;

    public List<Transform> spawnPoints;
    

    private void Start()
    {
        currentZombiesPerWave = initialZombiesPerWave;

        StartNextWave();
    }

    private void StartNextWave()
    {
        currentZombiesAlive.Clear();

        currentWave++;
        currentWaveNumberUI.text = "Wave : " + currentWave.ToString();

        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for(int i=0; i < currentZombiesPerWave; i++)
        {
            // Pick a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            //Generate a random offset within a specified range
            Vector3 spawnOffset = new Vector3(Random.Range(-2f, 2f),0f,Random.Range(-2f, 2f));
            Vector3 spawnPosition = spawnPoint.position + spawnOffset;

            //Instantiate the zombie
            var zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

            //Get Enemy Script
            EnemyScript enemyScript = zombie.GetComponent<EnemyScript>();                                  

            //Track this zombie
            currentZombiesAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }


    private void Update()
    {
        //Get all dead zombies
        List<EnemyScript> zombiesToRemove = new List<EnemyScript>();
        foreach(EnemyScript zombie in currentZombiesAlive)
        {
            if (zombie.isDead)
            {
                zombiesToRemove.Add(zombie);
            }
        }

        //Actually remove all dead zombies
        foreach(EnemyScript zombie in zombiesToRemove)
        {
            currentZombiesAlive.Remove(zombie);
        }

        zombiesToRemove.Clear();

        //Start Cooldown if all zombies are dead
        if(currentZombiesAlive.Count == 0 && inCoolDown == false)
        {
            //Start cooldown for next wave
            StartCoroutine(WaveCoolDown());
        }


        //Run the cooldown counter
        if (inCoolDown)
        {
            cooldownCounter = cooldownCounter - Time.deltaTime;
        }
        else
        {
            cooldownCounter = waveCoolDown;
        }

        cooldownTimerUI.text = cooldownCounter.ToString("F0");
    }


    private IEnumerator WaveCoolDown()
    {
        inCoolDown = true;
        waveOverUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(waveCoolDown);

        inCoolDown = false;
        waveOverUI.gameObject.SetActive(false);

        currentZombiesPerWave = currentZombiesPerWave * 2;

        StartNextWave();
    }


}
