using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieSpawnController : MonoBehaviour
{
    public int initialZombiesPerWave = 5;

    public int currentZombiesPerWave;

    public float spawnDelay = 0.5f;

    public int currentWave = 0;

    public float waveCooldown = 10.0f;

    public bool inCooldown;

    public float cooldownCounter = 0;

    public List<Enemy> currentZombiesAlive = new List<Enemy>();

    public GameObject zombiePrefab;

    public Text cooldownCounterUI;

    public Text currentWaveUI;


    private void Start()
    {
        currentZombiesPerWave = initialZombiesPerWave;

        StartNextWave();
    }


    private void StartNextWave()
    {
        currentZombiesAlive.Clear();

        currentWave++;

        currentWaveUI.text = "Wave: " + currentWave.ToString();

        StartCoroutine(SpawnWave());
    }


    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentZombiesPerWave; i++)
        {
            // Generate a random offset within a specified range
            Vector3 spawnOffset = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            );

            Vector3 spawnPosition = transform.position + spawnOffset;

            // Instantiate the Zombie
            var zombie = Instantiate(
                zombiePrefab,
                spawnPosition,
                Quaternion.identity
            );

            // Get Enemy Script
            Enemy enemyScript = zombie.GetComponent<Enemy>();

            // Track this zombie
            currentZombiesAlive.Add(enemyScript);

            // Delay before spawning the next zombie
            yield return new WaitForSeconds(spawnDelay);
        }
    }


    private void Update()
    {
        // Get all dead zombies
        List<Enemy> zombiesToRemove = new List<Enemy>();

        foreach (Enemy zombie in currentZombiesAlive)
        {
            if (zombie.isDead)
            {
                zombiesToRemove.Add(zombie);
            }
        }

        // Actually remove all dead zombies
        foreach (Enemy zombie in zombiesToRemove)
        {
            currentZombiesAlive.Remove(zombie);
        }

        zombiesToRemove.Clear();


        // Start cooldown if all zombies are dead
        if (currentZombiesAlive.Count == 0 && inCooldown == false)
        {
            StartCoroutine(WaveCooldown());
        }


        // Run the cooldown counter
        if (inCooldown)
        {
            cooldownCounter += Time.deltaTime;
        }
        else
        {
            cooldownCounter = waveCooldown;
        }


        // Update cooldown UI
        if (cooldownCounterUI != null)
        {
            cooldownCounterUI.text = cooldownCounter.ToString("F1");
        }
    }


    private IEnumerator WaveCooldown()
    {
        inCooldown = true;

        cooldownCounter = 0;

        yield return new WaitForSeconds(waveCooldown);

        inCooldown = false;

        // Increase zombies for the next wave
        // 5 -> 10 -> 20 -> 40 -> ...
        currentZombiesPerWave *= 2;

        StartNextWave();
    }
}