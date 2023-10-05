using Global;
using UnityEngine;
using UnityEngine.UI;

namespace Enemies
{
    public interface IEnemy
    {
        void TryAttack();
        void TakeDamage(int amount);
        void Die();
        void Initialize();
    }

    public abstract class Enemy : MonoBehaviour, IEnemy
    {
        [Header("Enemy Stats")]
        [SerializeField] protected int Health;
        [SerializeField] protected float MoveSpeed;
        [SerializeField] protected float AttackSpeed;
        [SerializeField] protected float AttackRange;
        [SerializeField] protected int Damage;

        [Space(10)]

        [Header("Spawning information")]
        [SerializeField] protected int Cost;
        [SerializeField] protected int UnlockingWave;
        [SerializeField] protected int LastSpawningWave;
        [SerializeField] protected float MaxPercentageInWave;
        [SerializeField] protected float MinPercentageInWave;

        public abstract void TryAttack();

        public void TakeDamage(int amount)
        {
            
        }

        public void Die()
        {
            throw new System.NotImplementedException();
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }
    }


    public class EnemyData : MonoBehaviour
    {
        [Header("Enemy Name")]
        public string enemyName;

        [Space(10)] [Header("Enemy Attributes")]
        public int enemyHealth;
        public float meleeRange;
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

        void Update()
        {
            if (Vector2.Distance(transform.position, GameManager.Player.Position) <= meleeRange)
            {
                GameManager.Instance.PlayerHit(1);
            }
        }
    }
}