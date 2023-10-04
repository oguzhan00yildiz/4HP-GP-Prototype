using Global;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemies
{
    public class SpawnerManager : MonoBehaviour
    {
        [HideInInspector] public int currentWave = 1;

        [SerializeField] private List<EnemyData> _enemies = new();

        private List<GameObject> _enemiesToSpawn = new();
        private Transform _player;
        private float _nextSpawnTime;
        private int _waveBudget;
        private int _totalEnemySpawned;
        public int TotalNumSpawned => _totalEnemySpawned;

        private const float SpawnRadius = 15;
        private const float EnemiesPerSecond = 2;
        private const int MaxEnemiesPerRound = 50;

        private bool _initialized;

        public void Initialize()
        {
            // Generates the first wave
            CalculateWaveBudget();

            _player = GameObject.FindWithTag("Player").transform;

            _initialized = true;
        }

        void Update()
        {
            if (!_initialized)
                return;

            // Checking if we have more enemies to spawn
            if (!(Time.time >= _nextSpawnTime) || _enemiesToSpawn.Count == 0)
                return;
            SpawnEnemy();
            _nextSpawnTime = Time.time + (1f / EnemiesPerSecond);
        }

        // This method decides the "budget" for the wave
        private void CalculateWaveBudget()
        {
            _waveBudget = currentWave * 10;
            GenerateEnemies();
        }

        // Creates the list of enemies that then are going to be spawned according to current wave and budget
        private void GenerateEnemies()
        {
            GameManager.Canvas.ResetProgressBar();
            _enemiesToSpawn.Clear();

            // LINQ expression to creates a list of all the unlocked enemies given the current wave
            var availableEnemies = _enemies.Where(enemy => enemy.unlockingWave <= currentWave && currentWave <= enemy.lastSpawningWave).ToList();

            List<GameObject> generatedEnemies = new();
            var remainingBudget = _waveBudget;

            // Chooses enemies at random from the available ones, until there are no more enemies available or enemies cap reached
            while (availableEnemies.Count != 0 && _enemiesToSpawn.Count < MaxEnemiesPerRound)
            {
                var randEnemyId = Random.Range(0, availableEnemies.Count);
                var randEnemyCost = availableEnemies[randEnemyId].enemyCost;

                if (remainingBudget - randEnemyCost > 0)
                {
                    // Finds the number of the specific enemy that have already been added to the list and calculates the percentage compared to the whole budget for the wave
                    var specificEnemyPercentage = ((float)generatedEnemies.FindAll(obj => availableEnemies[randEnemyId].enemyPrefab).Count) / _waveBudget * 100;

                    // Check if there are already too many of that enemy in the wave, if not, it adds it to the list, if yes, it deletes it from the available enemies
                    if (specificEnemyPercentage < availableEnemies[randEnemyId].maxPercentageInWave)
                    {
                        generatedEnemies.Add(availableEnemies[randEnemyId].enemyPrefab);
                        remainingBudget -= randEnemyCost;
                    }
                    else
                        availableEnemies.Remove(availableEnemies[randEnemyId]);
                }
                else
                    availableEnemies.Remove(availableEnemies[randEnemyId]);
            }

            _enemiesToSpawn = generatedEnemies;
            _totalEnemySpawned = generatedEnemies.Count;
        }

        // This method returns a random spawn point so for SpawnEnemy() to use
        private Vector3 PickRandomSpawnPoint()
        {
            // Calculate random angle in radians
            var angle = Random.Range(0f, 2f * Mathf.PI);

            // Calculate spawn position in a circle around the player
            var spawnX = _player.transform.position.x + SpawnRadius * Mathf.Cos(angle);
            var spawnY = _player.transform.position.y + SpawnRadius * Mathf.Sin(angle);

            return new Vector3(spawnX, spawnY, 0f);
        }

        // This method spawns the whole list, by spawning the first enemy and then deleting it from the list
        private void SpawnEnemy()
        {
            if (_enemiesToSpawn.Count <= 0)
                return;

            // Spawning the list of enemies that has been generated
            Instantiate(_enemiesToSpawn[0], PickRandomSpawnPoint(), Quaternion.identity);
            _enemiesToSpawn.RemoveAt(0);
        }

        // Restarts the spawning cycle
        public void StartNextWave()
        {
            currentWave++;
            CalculateWaveBudget();
        }
    }
}