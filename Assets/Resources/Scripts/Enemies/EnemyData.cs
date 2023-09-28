using UnityEngine;
using UnityEngine.UI;

namespace Enemies
{
    public class EnemyData : MonoBehaviour
    {
        [Header("Enemy Name")]
        public string enemyName;
        [Space(10)]

        [Header("Enemy Attributes")]
        public int enemyHealth;
        public int enemyCost;
        public int unlockingWave;
        public int lastSpawningWave;
        public float maxPercentageInWave;
        [Space(10)]


        [Header("References")]
        public GameObject enemyPrefab;
        public Slider enemyHealthBar;

        void Start()
        {
            enemyHealthBar.maxValue = enemyHealth;
            enemyHealthBar.value = enemyHealth;
        }
    }
}