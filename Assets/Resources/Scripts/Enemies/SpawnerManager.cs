using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Resources.Scripts.Enemies
{
    public class SpawnerManager : MonoBehaviour
    {
        [HideInInspector] public int currentWave;

        [SerializeField] private List<EnemyData> _enemies = new();

        
        private List<GameObject> _enemiesToSpawn = new();
        private Transform _player;
        private float _nextSpawnTime;
        private int _waveValue;

        private const float SpawnRadius = 15;
        private const float EnemiesPerSecond = 2;
        private const int MaxEnemiesPerRound = 50;

        void Start()
        {
            #region Missing Player Check

            _player = FindObjectOfType<Player>().transform;

            if (_player == null)
            {
                Debug.LogError("No player found.");
                enabled = false;
                return;
            }

            #endregion

            // Generates the first wave
            CalculateWaveBudget(); 
        }

        void Update()
        {
            // Checking if we have more enemies to spawn
            if (Time.time >= _nextSpawnTime && _enemiesToSpawn.Count != 0) 
            {
                SpawnEnemy();
                _nextSpawnTime = Time.time + (1f / EnemiesPerSecond);
            }

            // When enemies have been spawned AND have been killed, then we go to the next wave and start the cycle
            if (_enemiesToSpawn.Count == 0 && FindObjectsOfType<EnemyMovement>().Length == 0)
            {
                StartNextWave();
            }
        }

        // This method decides the "budget" for the wave
        private void CalculateWaveBudget() 
        {
            _waveValue = currentWave * 10;
            GenerateEnemies();
        }

        // Creates the list of enemies that then are going to be spawned according to current wave and budget
        private void GenerateEnemies()
        {
            // Creates a list of all the unlocked enemies given the current wave
            var availableEnemies = _enemies.Where(enemy => enemy.unlockingWave <= currentWave && currentWave <= enemy.lastSpawningWave).ToList();
            
            List<GameObject> generatedEnemies = new();

            // Chooses enemies at random from the available ones, until the budget is spent or enemies cap reached
            while (_waveValue > 0 && _enemiesToSpawn.Count < MaxEnemiesPerRound)
            {
                if (availableEnemies.Count == 0)
                    break;

                var randEnemyId = Random.Range(0, availableEnemies.Count);
                var randEnemyCost = availableEnemies[randEnemyId].enemyCost;

                if (_waveValue - randEnemyCost < 0)
                    availableEnemies.Remove(availableEnemies[randEnemyId]);
                else
                {
                    generatedEnemies.Add(availableEnemies[randEnemyId].enemyPrefab);
                    _waveValue -= randEnemyCost;
                }
            }

            _enemiesToSpawn.Clear();
            _enemiesToSpawn = generatedEnemies;
        }

        // This method spawns the whole list, by spawning the first enemy and then deleting it from the list
        private void SpawnEnemy() 
        {
            // Calculate random angle in radians
            var angle = Random.Range(0f, 2f * Mathf.PI);

            // Calculate spawn position in a circle around the player
            var spawnX = _player.transform.position.x + SpawnRadius * Mathf.Cos(angle); 
            var spawnY = _player.transform.position.y + SpawnRadius * Mathf.Sin(angle);

            var spawnPosition = new Vector3(spawnX, spawnY, 0f);

            if (_enemiesToSpawn.Count <= 0) return;

            // Spawning the list of enemies that has been generated
            Instantiate(_enemiesToSpawn[0], spawnPosition, Quaternion.identity);
            _enemiesToSpawn[0].GetComponent<EnemyData>().enemyHealthBar.maxValue = _enemiesToSpawn[0].GetComponent<EnemyData>().enemyHealth;
            _enemiesToSpawn[0].GetComponent<EnemyData>().enemyHealthBar.value = _enemiesToSpawn[0].GetComponent<EnemyData>().enemyHealth;
            _enemiesToSpawn.RemoveAt(0);
        }

        private void StartNextWave()
        {
            currentWave++;
            CalculateWaveBudget();
        }
    }
}