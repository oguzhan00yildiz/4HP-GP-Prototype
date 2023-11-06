using Global;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemies
{
    public class SpawnerManager : MonoBehaviour
    {
        [HideInInspector] public int currentWave = 1;

        private List<Enemy> _enemies = new();
        private Dictionary<ulong, bool> _enemiesAlive = new();

        private List<GameObject> _spawnPool = new();
        private Transform _player;
        private float _nextSpawnTime;
        private int _waveBudget;
        private int _totalEnemySpawned;
        public int TotalNumSpawned => _totalEnemySpawned;

        private const float SpawnRadius = 15;
        private const float EnemiesPerSecond = 2;
        private const int MaxEnemiesPerRound = 50;
        private ulong _enemyIdBase = 0;

        private bool _initialized;

        public bool IsEnemyAlive(ulong id)
            =>  _enemiesAlive.TryGetValue(id, out bool isAlive) && isAlive;

        public void SetEnemyDead(ulong id)
            => _enemiesAlive.Remove(id);

        public void Initialize()
        {
            // Loads all the enemies from the resources folder
            _enemies = new List<Enemy>(Resources.LoadAll<Enemy>("Prefabs/Enemies"));

            // Generates the first wave
            CalculateWaveBudget();

            // Finds the player
            _player = GameObject.FindWithTag("Player").transform;

            // All good
            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized)
                return;

            // Checking if we have more enemies to spawn
            if (!(Time.time >= _nextSpawnTime) || _spawnPool.Count == 0)
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
            GameManager.CanvasManager.ResetProgressBar();
            _spawnPool.Clear();

            // LINQ expression to creates a list of all the unlocked enemies given the current wave
            var availableEnemies
                = _enemies.Where(
                    enemy => enemy.Data.unlockingWave <= currentWave
                             && currentWave <= enemy.Data.lastSpawningWave).ToList();

            List<Enemy> generatedEnemies = new();
            var remainingBudget = _waveBudget;

            // Chooses enemies at random from the available ones, until there are no more enemies available or enemies cap reached
            while (availableEnemies.Count != 0 && _spawnPool.Count < MaxEnemiesPerRound)
            {
                var randEnemyId = Random.Range(0, availableEnemies.Count);
                var randEnemyCost = availableEnemies[randEnemyId].Data.enemyCost;

                if (remainingBudget - randEnemyCost > 0)
                {
                    // Finds the number of the specific enemy that have already been added
                    // to the list and calculates the percentage compared to the whole budget for the wave

                    // Read FindAll as "find all the enemies in the list that have the same name as the one we are checking"
                    // or with SQL terms,
                    //      "select * (ALL)
                    //      from availableEnemies
                    //      where EnemyName = availableEnemies[randEnemyId].EnemyName"
                    var specificEnemyPercentage
                        = (float)generatedEnemies.
                            FindAll(obj => obj.EnemyName == availableEnemies[randEnemyId].EnemyName).Count / _waveBudget * 100;

                    // Check if there are already too many of that enemy in the wave,
                    // if not, it adds it to the list, if yes, it deletes it from the available enemies
                    if (specificEnemyPercentage < availableEnemies[randEnemyId].Data.maxPercentageInWave)
                    {
                        generatedEnemies.Add(availableEnemies[randEnemyId]);
                        remainingBudget -= randEnemyCost;
                    }
                    else
                        availableEnemies.Remove(availableEnemies[randEnemyId]);
                }
                else
                    availableEnemies.Remove(availableEnemies[randEnemyId]);
            }

            // The old way stored a prefab in each enemydata, this new way
            // "makes a prefab" from the enemy which is a monobehaviour ~= is a gameobject
            _spawnPool = new List<GameObject>();
            foreach (Enemy enemy in generatedEnemies)
                _spawnPool.Add(enemy.gameObject);

            _totalEnemySpawned = generatedEnemies.Count;
        }

        // This method returns a random spawn point so for SpawnEnemy() to use
        private Vector3 PickRandomSpawnPoint()
        {
            if (_player == null)
            {
                Debug.LogError("Player not found!");
                return Vector3.zero;
            }

            // Calculate random angle in radians
            var angle = Random.Range(0f, 2f * Mathf.PI);

            // Calculate spawn position in a circle around the player
            var spawnX = _player.transform.position.x + SpawnRadius * Mathf.Cos(angle);
            var spawnY = _player.transform.position.y + SpawnRadius * Mathf.Sin(angle);

            return new Vector3(spawnX, spawnY, 0f);
        }

        private ulong GetNewEnemyId()
        {
            _enemyIdBase++;
            return _enemyIdBase;
        }

        // This method spawns the whole list, by spawning the first enemy and then deleting it from the list
        private void SpawnEnemy()
        {
            if (_spawnPool.Count <= 0)
                return;

            // Spawning the list of enemies that has been generated
            var spawnedEnemy = Instantiate(_spawnPool[0], PickRandomSpawnPoint(), Quaternion.identity);

            ulong newId = GetNewEnemyId();
            spawnedEnemy.GetComponent<Enemy>().InitializeSetId(newId);

            _enemiesAlive.Add(newId, true);

            _spawnPool.RemoveAt(0);
        }

        // Restarts the spawning cycle
        public void StartNextWave()
        {
            currentWave++;
            CalculateWaveBudget();
        }
    }
}