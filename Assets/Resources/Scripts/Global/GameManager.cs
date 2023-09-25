using System.Collections;
using Assets.Resources.Scripts.Enemies;
using UnityEngine;

namespace Assets.Resources.Scripts.Global
{
    public class GameManager : MonoBehaviour
    {
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

        public int killedEnemies;
        private const float HealthBarTransitionDuration = .1f;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
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
            killedEnemies++;
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
    }
}