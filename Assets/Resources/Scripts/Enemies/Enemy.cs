using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour, IEnemy
    {
        [Header("Enemy Stats")] 
        public string EnemyName;
        public int Health;
        [SerializeField] protected float MoveSpeed;
        [SerializeField] protected float AttackSpeed;
        [SerializeField] protected float AttackRange;
        [SerializeField] protected int Damage;

        protected float TimeAtLastAttack;

        [Space(10)] [Header("Spawn Attributes")] 
        [SerializeField] public EnemyData Data;

        [Space(10)]
        protected Slider HealthBar;

        protected Rigidbody2D Rb;
        protected Transform Target;
        protected Vector3 TargetDirection;
        protected Vector2? KnockbackVector;

        private bool _initialized;
        private static GameObject _popUpTextPrefab;

        public abstract void TryAttack();

        public void TakeDamage(int amount, Vector2? damageOrigin = null, float knockbackAmount = 0)
        {
            int originalHealth = Health;
            Health -= amount;

            StartCoroutine(UpdateHealthBar(originalHealth, Health));
            DisplayDamageNumber(transform.position, amount);

            if (!damageOrigin.HasValue)
                return;

            // Knockback
            Vector2 knockback 
                = ((Vector2)transform.position - (Vector2)damageOrigin).normalized * knockbackAmount;
            
            KnockbackVector = knockback;
        }

        // Updates the health bar to the new value with a smooth animation
        private IEnumerator UpdateHealthBar(int initialHealth, float targetHealth)
        {
            var elapsedTime = 0f;

            while (elapsedTime < 0.1f)
            {
                SetHealthBarValue(Mathf.Lerp(initialHealth, targetHealth, elapsedTime / 0.1f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            SetHealthBarValue(targetHealth);
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

        public void Initialize()
        {
            HealthBar = transform.Find("WorldCanvas/HealthBar").GetComponent<Slider>();
            HealthBar.maxValue = Health;
            HealthBar.value = Health;

            if (_popUpTextPrefab == null)
                _popUpTextPrefab = Resources.Load<GameObject>("Prefabs/Enemies/EnemyDamagePopUp");

            Target = GameObject.FindWithTag("Player").transform;
            Rb = GetComponent<Rigidbody2D>();
            if (Rb == null)
            {
                Rb = gameObject.AddComponent<Rigidbody2D>();
                Rb.bodyType = RigidbodyType2D.Kinematic;
                Rb.gravityScale = 0.0f;
                Rb.freezeRotation = true;
                Rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }

            _initialized = true;
        }

        protected virtual void Update()
        {
            if (!_initialized)
                return;

            TryAttack();

            if (Target != null)
            {
                // Not normalized since we'll use magnitude to gauge distance
                Vector3 direction = (Target.position - transform.position);
                TargetDirection = direction;

                // Move in fixed update
            }
        }

        protected virtual void FixedUpdate()
        {
            if (Rb == null)
                return;
            Vector2 moveAmount = transform.position + TargetDirection.normalized * MoveSpeed * Time.fixedDeltaTime;
            //Vector2 moveAmount = TargetDirection.normalized * MoveSpeed;

            // TODO: Fix knockback (too snappy/laggy looking rn)
            //if (KnockbackVector.HasValue)
            //{
            //    moveAmount += KnockbackVector.Value;
            //    KnockbackVector = null;
            //}

            //Rb.velocity += moveAmount * Time.fixedDeltaTime;
            Rb.MovePosition(moveAmount);
        }

        public void Die()
        {
            throw new System.NotImplementedException();
        }

        internal void SetHealthBarValue(float targetHealth)
        {
            HealthBar.value = targetHealth;
        }
    }
}