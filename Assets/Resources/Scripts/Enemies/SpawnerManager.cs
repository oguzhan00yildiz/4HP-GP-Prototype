using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Player;

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

            _player = FindObjectOfType<Player.Player>().transform;

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

            // Chooses enemies at random from the available ones, until there are no more enemies available or enemies cap reached
            while (availableEnemies.Count != 0 && _enemiesToSpawn.Count < MaxEnemiesPerRound)
            {
                var randEnemyId = Random.Range(0, availableEnemies.Count);
                var randEnemyCost = availableEnemies[randEnemyId].enemyCost;

                if (_waveValue - randEnemyCost > 0)
                {
                    generatedEnemies.Add(availableEnemies[randEnemyId].enemyPrefab);
                    _waveValue -= randEnemyCost;
                }
                else
                 availableEnemies.Remove(availableEnemies[randEnemyId]);
            }

            _enemiesToSpawn.Clear();
            _enemiesToSpawn = generatedEnemies;
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
            if (_enemiesToSpawn.Count <= 0) return;

            // Spawning the list of enemies that has been generated
            Instantiate(_enemiesToSpawn[0], PickRandomSpawnPoint(), Quaternion.identity);
            _enemiesToSpawn.RemoveAt(0);
        }

        // Restarts the spawning cycle
        private void StartNextWave()
        {
            currentWave++;
            CalculateWaveBudget();
        }
    }
}