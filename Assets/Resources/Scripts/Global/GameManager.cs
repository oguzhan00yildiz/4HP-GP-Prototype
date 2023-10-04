using Enemies;
using PlayerLogic;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Global
{
    public class GameManager : MonoBehaviour
    {
        // Public static references
        // Use these to access game instance references.
        public static CanvasManager Canvas
            => _instance._canvasManager;

        public static UpgradeManager Upgrades
            => _instance._upgradeManager;

        public static ProjectileManager Projectiles
            => _instance._projectileManager;

        public static Player Player
        => _instance._player;

        public static Player.PlayerCharacter PlayerType
            => _instance._playerType;

        private CanvasManager _canvasManager;
        private UpgradeManager _upgradeManager;
        private ProjectileManager _projectileManager;
        private Player _player;
        private Player.PlayerCharacter _playerType;

        private int _killedEnemies;
        private const float HealthBarTransitionDuration = .1f;
        private bool _playerReady;

        private static GameObject _popUpTextPrefab;

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

            Initialize();
        }

        private void Initialize()
        {
            _popUpTextPrefab = Resources.Load<GameObject>("Prefabs/Effects/PopUpText");
            _canvasManager = FindObjectOfType<CanvasManager>();
            _upgradeManager = gameObject.AddComponent<UpgradeManager>();
            _projectileManager = gameObject.AddComponent<ProjectileManager>();
            _player = FindObjectOfType<Player>();

        }

        public void EnemyHit(EnemyData data, int damage)
        {
            DisplayDamageNumber(data.transform.position, damage);

            var initialHealth = data.enemyHealth;
            var remainingHealth = data.enemyHealth -= damage;

            StartCoroutine(UpdateEnemyHealthBar(data, initialHealth, remainingHealth));

            if (remainingHealth <= 0)
            {
                KillEnemy(data.gameObject);
            }
        }

        // Deals damage to the enemy hit
        public void EnemyHit(GameObject enemyGameObject, int damage)
        {
            DisplayDamageNumber(enemyGameObject.transform.position, damage);

            EnemyData data = enemyGameObject.GetComponent<EnemyData>();

            var initialHealth = data.enemyHealth;
            var remainingHealth = data.enemyHealth -= damage;

            StartCoroutine(UpdateEnemyHealthBar(data, initialHealth, remainingHealth));

            if (remainingHealth <= 0)
            { KillEnemy(enemyGameObject); }
        }

        private void KillEnemy(EnemyData data)
        {
            Destroy(data.gameObject);
            _killedEnemies++;
            var progressPercentage = (float)_killedEnemies
                                     / (SpawnerManager.Instance.TotalNumSpawned != 0
                                        ? SpawnerManager.Instance.TotalNumSpawned
                                        : 1);

            Canvas.UpdateProgress(progressPercentage);

            if (_killedEnemies < SpawnerManager.Instance.TotalNumSpawned)
            {
                return;
            }

            ClearKilledEnemyCounter();
            Canvas.ShowWaveCompletionScreen();
            // Start waiting for player to be ready to start next wave
            StartCoroutine(PlayerContinueWaiter());
        }

        // Kills the enemy
        private void KillEnemy(GameObject enemyKilled)
        {
            Destroy(enemyKilled);
            _killedEnemies++;
            var progressPercentage = (float)_killedEnemies / (SpawnerManager.Instance.TotalNumSpawned != 0
                ? SpawnerManager.Instance.TotalNumSpawned
                : 1);
            Canvas.UpdateProgress(progressPercentage);

            if (_killedEnemies < SpawnerManager.Instance.TotalNumSpawned)
            {
                return;
            }

            ClearKilledEnemyCounter();
            Canvas.ShowWaveCompletionScreen();
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
        private static IEnumerator UpdateEnemyHealthBar(EnemyData enemy, int initialHealth, float targetHealth)
        {
            var elapsedTime = 0f;

            while (elapsedTime < HealthBarTransitionDuration)
            {
                enemy.enemyHealthBar.value = Mathf.Lerp(initialHealth, targetHealth, elapsedTime / HealthBarTransitionDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            enemy.enemyHealthBar.value = targetHealth;
        }
        private void DisplayDamageNumber(Vector2 origin, int damageAmount)
        {
            float minX = Const.Effects.POPUP_MIN_X_OFFSET;
            float maxX = Const.Effects.POPUP_MAX_X_OFFSET;

            float rnd = Random.Range(minX, maxX);
            var newPos = new Vector3(rnd, 0, 0);
            var popUpObject = Instantiate(_popUpTextPrefab, origin + (Vector2)newPos, Quaternion.identity);
            var tmpText = popUpObject.GetComponentInChildren<TextMeshPro>();
            tmpText.text = damageAmount.ToString();
            Destroy(popUpObject, .5f);
        }

        public void ClearKilledEnemyCounter()
        {
            _killedEnemies = 0;
        }
    }
}