using Enemies;
using PlayerLogic;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Global
{
    [RequireComponent(typeof(SpawnerManager), typeof(UpgradeManager), typeof(ProjectileManager))]
    public class GameManager : MonoBehaviour
    {
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

        // Public static references
        // Use these to access game instance references.
        #region Static References
        public static bool DebugMode
            => _instance._enableDebug;
        public static CanvasManager CanvasManager
            => _instance._canvasManager;

        public static UpgradeManager Upgrades
            => _instance._upgradeManager;

        public static ProjectileManager Projectiles
            => _instance._projectileManager;

        public static BasePlayer Player
            => _instance._player;

        public static SpawnerManager EnemyManager
            => _instance._enemyManager;
        #endregion

        private CanvasManager _canvasManager;
        private SpawnerManager _enemyManager;
        private UpgradeManager _upgradeManager;
        private ProjectileManager _projectileManager;
        private BasePlayer _player;
        

        [SerializeField] private IPlayer.PlayerCharacter _playerType;
        [SerializeField] private Texture2D cursorTexture;

        private int _numKilledEnemies;
        private bool _playerReady;

        private static GameObject _popUpTextPrefab;

        [Header("Debug")]
        [SerializeField] private bool _enableDebug;
        [SerializeField] private bool _godMode;
        [SerializeField] private bool _nerfPlayer;

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

        private void Start()
        {
            
        }

        public void StartGame()
        {
            if (_player != null)
            {
                Debug.LogWarning("Player already exists, game probably running. Trying to start game, what's up??");
                return;
            }

            StartGameAsync();
        }

        public void ReturnToMainMenu()
        {
            Deinitialize();
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        private async void StartGameAsync()
        {
            // Asynchronous loading of the game scene
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");

            asyncLoad.allowSceneActivation = true;

            // NOTE: Since our game is so small, we don't really
            // need to wait for the loading to finish.
            // But this is how you would do it if you needed to.

            // While loading we let the game continue
            while (!asyncLoad.isDone)
            {
                Debug.Log("Loading progress: " + asyncLoad.progress);
                await Task.Delay(100);
            }

            // Set the game scene as the active scene.
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScene"));

            // Finally, initialize the game
            Initialize();
        }

        public void Deinitialize()
        {
            Destroy(_player.gameObject);
            Destroy(_canvasManager.gameObject);
            Destroy(_enemyManager.gameObject);
            Destroy(_upgradeManager.gameObject);
            Destroy(_projectileManager.gameObject);
        }

        public void Initialize()
        {
            _player = CreatePlayer(_playerType, true);

            _popUpTextPrefab = Resources.Load<GameObject>("Prefabs/Effects/DamagePopUpParent");

            if ((_canvasManager = FindObjectOfType<CanvasManager>()) == null)
            {
                _canvasManager = CreateGameCanvas();
            }

            _upgradeManager = gameObject.GetComponent<UpgradeManager>();
            _upgradeManager.Initialize();

            _enemyManager = gameObject.GetComponent<SpawnerManager>();
            _enemyManager.Initialize();

            _projectileManager = gameObject.GetComponent<ProjectileManager>();

            SetCursor();
        }

        private static BasePlayer CreatePlayer(IPlayer.PlayerCharacter type, bool initialize)
        {
            switch (type)
            {
                case IPlayer.PlayerCharacter.Archer:
                    return SpawnArcher(initialize);
                case IPlayer.PlayerCharacter.Tank:
                    return SpawnTank(initialize);
                default:
                    throw new System.Exception("Invalid player type");
            }
        }

        private static ArcherPlayer SpawnArcher(bool initialize)
        {
            var playerPrefab = Resources.Load<GameObject>("Prefabs/Player/ArcherPlayer");
            var playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            var player = playerObj.GetComponent<ArcherPlayer>();

            if (initialize)
            {
                player.Initialize();
            }
            return player;
        }

        private static TankPlayer SpawnTank(bool initialize)
        {
            var playerPrefab = Resources.Load<GameObject>("Prefabs/Player/TankPlayer");
            var playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            var player = playerObj.GetComponent<TankPlayer>();
            if (initialize)
            {
                player.Initialize();
            }
            return player;
        }

        private static CanvasManager CreateGameCanvas()
        {
            var canvasPrefab = Resources.Load<GameObject>("Prefabs/UI/GameCanvas");
            var canvasObj = Instantiate(canvasPrefab, Vector3.zero, Quaternion.identity);
            var canvas = canvasObj.GetComponentInChildren<CanvasManager>();
            return canvas;
        }

        public void PlayerHit(int damage)
        {
            if (_godMode)
            {
                return;
            }

            _player.TakeDamage(damage);
        }

        public void EnemyHitWithKnockback(GameObject enemyObject, int damage, Vector2 source, float knockback)
        {
            var enemy = enemyObject.GetComponent<Enemy>();
            if (enemy == null)
                return;

            EnemyHitWithKnockback(enemy, damage, source, knockback);
        }

        public void EnemyHitWithKnockback(Enemy enemy, int damage, Vector2 source, float knockback)
        {
            if (DebugMode && _nerfPlayer)
            {
                return;
            }

            enemy.TakeDamage(damage, source, knockback);

            if (enemy.Health <= 0)
            {
                KillEnemy(enemy);
            }
        }

        public void EnemyHit(Enemy enemy, int damage)
        {
            if (_nerfPlayer)
            {
                return;
            }

            enemy.TakeDamage(damage);

            if (enemy.Health <= 0)
            {
                KillEnemy(enemy);
            }
        }

        // Deals damage to the enemy hit
        public void EnemyHit(GameObject enemyGameObject, int damage)
        {
            Enemy enemy = enemyGameObject.GetComponent<Enemy>();

            EnemyHit(enemy, damage);
        }

        private void KillEnemy(Enemy enemy)
        {
            if (!EnemyManager.IsEnemyAlive(enemy.Id))
                return;

            EnemyManager.SetEnemyDead(enemy.Id);

            Destroy(enemy.gameObject);
            _numKilledEnemies++;
            var progressPercentage = (float)_numKilledEnemies
                                     / (_enemyManager.TotalNumSpawned != 0
                                        ? _enemyManager.TotalNumSpawned
                                        : 1);

            CanvasManager.UpdateProgress(progressPercentage);

            if (_numKilledEnemies < _enemyManager.TotalNumSpawned)
            {
                return;
            }

            ClearKilledEnemyCounter();
            CanvasManager.ShowWaveCompletionScreen();
            // Start waiting for player to be ready to start next wave
            StartCoroutine(PlayerContinueWaiter());
        }

        private IEnumerator PlayerContinueWaiter()
        {
            while (!_playerReady)
            {
                yield return new WaitForSeconds(0.25f);
            }

            _enemyManager.StartNextWave();
            _playerReady = false;
        }

        // Call this to set the internal flag to true to start the next wave
        public void PlayerSetReady()
        {
            _playerReady = true;
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
            _numKilledEnemies = 0;
        }

        private void SetCursor()
        {
            if (cursorTexture == null)
            {
                Debug.LogError("Cursor texture not set!");
                return;
            }
            var cursorHotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
            Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.Auto);
        }
    }
}