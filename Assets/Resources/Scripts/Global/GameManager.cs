using System.Collections;
using Assets.Resources.Scripts.Enemies;
using UnityEngine;

namespace Assets.Resources.Scripts.Global
{
    public class GameManager : MonoBehaviour
    {
        private int _killedEnemies;
        private const float HealthBarTransitionDuration = .1f;
        private bool _playerReady;

        #region Singleton
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<GameManager>();

                if (_instance != null) return _instance;
                var obj = new GameObject("GameManager");
                _instance = obj.AddComponent<GameManager>();

                return _instance;
            }
        }
        #endregion

        private void OnEnable()
        {
            if (_instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        // Deals damage to the enemy hit
        public void EnemyHit(EnemyData enemyHit, int damage)
        {
            var initialHealth = enemyHit.enemyHealth;
            var remainingHealth = enemyHit.enemyHealth -= damage;

            StartCoroutine(UpdateHealthBar(enemyHit, initialHealth, remainingHealth));

            if (remainingHealth <= 0) { EnemyKilled(enemyHit); }
        }

        // Kills the enemy
        public void EnemyKilled(EnemyData enemyKilled)
        {
            Destroy(enemyKilled.gameObject);
            _killedEnemies++;
            var progressPercentage = (float)_killedEnemies / (SpawnerManager.Instance.TotalNumSpawned != 0
                ? SpawnerManager.Instance.TotalNumSpawned
                : 1);
            CanvasManager.Instance.UpdateProgress(progressPercentage);

            if (_killedEnemies < SpawnerManager.Instance.TotalNumSpawned)
            {
                return;
            }

            ClearKilledEnemyCounter();
            CanvasManager.Instance.OnWaveCompleted();
            // Start waiting for player to be ready to start next wave
            StartCoroutine(PlayerContinueWaiter());
        }

        private IEnumerator PlayerContinueWaiter()
        {
            while (!_playerReady)
            {
                yield return new WaitForSeconds(0.25f);
            }

            SpawnerManager.Instance.StartNextWave();
            _playerReady = false;
        }

        public void PlayerSetReady()
        {
            _playerReady = true;
        }

        // Updates the health bar to the new value with a smooth animation
        private static IEnumerator UpdateHealthBar(EnemyData enemyHit, int initialHealth, float targetHealthRatio)
        {
            var elapsedTime = 0f;

            while (elapsedTime < HealthBarTransitionDuration)
            {
                enemyHit.enemyHealthBar.value = Mathf.Lerp(initialHealth, targetHealthRatio, elapsedTime / HealthBarTransitionDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            enemyHit.enemyHealthBar.value = targetHealthRatio;
        }

        public void ClearKilledEnemyCounter()
        {
            _killedEnemies = 0;
        }
    }
}