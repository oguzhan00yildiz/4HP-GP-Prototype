using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [HideInInspector] public int currentWave;

    [SerializeField] private List<Enemy> enemies = new List<Enemy>();
    [SerializeField] private float enemiesPerSecond;

    private List<GameObject> enemiesToSpawn = new List<GameObject>();
    private float spawnRadius = 15;
    private Transform player;
    private float nextSpawnTime;
    private int waveValue;
    private int maxEnemiesPerRound= 50;

    void Start()
    {
        #region Missing Player Check
        player = FindObjectOfType<Player>().transform;

        if (player == null)
        {
            Debug.LogError("No player found.");
            enabled = false;
            return;
        }
        #endregion

        CalculateWaveBudget(); // Generates the first wave
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && enemiesToSpawn.Count != 0) // Checking if we have more enemies to spawn
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + (1f / enemiesPerSecond);
        }

        if(enemiesToSpawn.Count == 0 && FindObjectsOfType<EnemyMovement>().Length == 0) // When enemies have been spawned AND have been killed, then we go to the next wave and start the cycle
        {
            StartNextWave();
        }
    }

    void CalculateWaveBudget() // This method decides the "budget" for the wave
    {
        waveValue = currentWave * 10;
        GenerateEnemies();
    }

    void GenerateEnemies() // Creates the list of enemies that then are going to be spawned according to current wave and budget
    {
        List<Enemy> availableEnemies = new List<Enemy>(); // Creates a list of all the spawnable enemies given the current wave

        for (int i = 0; i < enemies.Count; i++) 
        {
            if (enemies[i].unlockingWave <= currentWave &&
                currentWave <= enemies[i].lastSpawningWave)
            {
                availableEnemies.Add(enemies[i]);
            }
        }

        List<GameObject> generatedEnemies = new List<GameObject>(); // Chooses enemies at random from the available ones, until the budget is spent or enemies cap reached

        while (waveValue > 0 && enemiesToSpawn.Count < maxEnemiesPerRound)
        {
            int randEnemyID = Random.Range(0, availableEnemies.Count);
            int randEnemyCost = availableEnemies[randEnemyID].enemyCost;

            if(waveValue - randEnemyCost >= 0)
            {
                generatedEnemies.Add(availableEnemies[randEnemyID].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if(waveValue <= 0) 
            {
                break;
            }
        }

        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

    void SpawnEnemy() // This method spawns the whole list, by spawning the first enemy and then deleting it from the list
    {
        float angle = Random.Range(0f, 2f * Mathf.PI); // Calculate random angle in radians

        float spawnX = player.transform.position.x + spawnRadius * Mathf.Cos(angle); // Calculate spawn position in a circle around the player
        float spawnY = player.transform.position.y + spawnRadius * Mathf.Sin(angle); //

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        if(enemiesToSpawn.Count > 0) 
        {
            Instantiate(enemiesToSpawn[0], spawnPosition, Quaternion.identity);
            enemiesToSpawn.RemoveAt(0);
        }
    }

    void StartNextWave()
    {
        currentWave++;
        CalculateWaveBudget();
    }
}

[System.Serializable]
public class Enemy
{
    public string enemyName;
    
    [Space(10)]

    public GameObject enemyPrefab;
    public int enemyCost;

    public int unlockingWave;
    public int lastSpawningWave;
} 