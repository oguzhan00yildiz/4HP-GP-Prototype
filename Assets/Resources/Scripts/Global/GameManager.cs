using Enemies;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Global
{
    public class GameManager : MonoBehaviour
    {
        private int _killedEnemies;
        private const float HealthBarTransitionDuration = .1f;
        private bool _playerReady;

        [SerializeField] private GameObject _popUpTextPrefab;
        [SerializeField] private float _minX, _maxX;

        #region Singleton
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                _instance = FindObjectOfType<GameManager>();

                if (_instance != null)
                    return _instance;
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
        public void EnemyHit(GameObject enemyHit, int damage)
        {
            DisplayDamageNumber(enemyHit.transform.position, damage);

            var initialHealth = enemyHit.GetComponent<EnemyData>().enemyHealth;
            var remainingHealth = enemyHit.GetComponent<EnemyData>().enemyHealth -= damage;

            StartCoroutine(UpdateHealthBar(enemyHit.GetComponent<EnemyData>(), initialHealth, remainingHealth));

            if (remainingHealth <= 0)
            { EnemyKilled(enemyHit); }
        }

        // Kills the enemy
        public void EnemyKilled(GameObject enemyKilled)
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

        // Call this to set the internal flag to true to start the next wave
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
        private void DisplayDamageNumber(Vector2 origin, int damageAmount)
        {
            float rnd = Random.Range(_minX, _maxX);
            var newPos = new Vector3(rnd, 0, 0);
            var popUpObject = Instantiate(_popUpTextPrefab, origin + (Vector2)newPos, Quaternion.identity);
            popUpObject.transform.GetComponentInChildren<TextMeshPro>().text = damageAmount.ToString();
            Destroy(popUpObject.gameObject, .5f);
        }

        public void ClearKilledEnemyCounter()
        {
            _killedEnemies = 0;
        }
    }
}