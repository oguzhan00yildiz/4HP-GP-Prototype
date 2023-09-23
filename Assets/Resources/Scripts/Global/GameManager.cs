using System.Collections;
using Assets.Resources.Scripts.Enemies;
using UnityEngine;

namespace Assets.Resources.Scripts.Global
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject("GameManager");
                        instance = obj.AddComponent<GameManager>();
                    }
                }
                return instance;
            }
        }
        #endregion

        public int killedEnemies;
        private const float HealthBarTransitionDuration = .1f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
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

        // Updates the health bar to the new value with a smooth animation
        private IEnumerator UpdateHealthBar(EnemyData enemyHit, int initialHealth, float targetHealthRatio)
        {
            ;
            var elapsedTime = 0f;

            while (elapsedTime < HealthBarTransitionDuration)
            {
                enemyHit.enemyHealthBar.value = Mathf.Lerp(initialHealth, targetHealthRatio, elapsedTime / HealthBarTransitionDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            enemyHit.enemyHealthBar.value = targetHealthRatio;
        }

        // Kills the enemy
        public void EnemyKilled(EnemyData enemyKilled)
        {
            Destroy(enemyKilled.gameObject);
            killedEnemies++;
        }
    }
}