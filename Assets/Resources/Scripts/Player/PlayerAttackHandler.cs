using Global;
using UnityEngine;

namespace PlayerLogic
{
    public class PlayerAttackHandler : MonoBehaviour
    {
        [HideInInspector]
        public Transform attackOrigin;

        // To be used in tandem with movement script
        // so it doesn't flip the player around when trying to attack
        [HideInInspector]
        public bool Attacking { get; private set; }

        // This is a cached reference to a prefab
        private static GameObject _meleeEffect;

        [SerializeField]
        private float _meleeDelay = 0.15f;
        private float _timeAtLastMelee = 0;
        private float _timeAtLastProjectile = 0;

        [SerializeField] private Vector3 _meleeHitBox;

        private bool _shouldAttack => EnemiesInRangeCheck();

        //this is setting popupdamage effect settings
        [SerializeField] private float _meleeDamage;
        [SerializeField] private float _projectileFireRate;
        [SerializeField] LayerMask enemyLayer;
        [SerializeField] private float meleeDetectionRadius;
        [HideInInspector]
        public static PlayerAttackHandler instance;

        public PlayerAttackHandler(float projectileFireRate, LayerMask enemyLayer)
        {
            _projectileFireRate = projectileFireRate;
            this.enemyLayer = enemyLayer;
        }

        private void OnEnable()
        {
            attackOrigin = transform.Find("AttackOrigin");
            instance = this;
        }
        private void Start()
        {
            _meleeEffect = Resources.Load<GameObject>("Prefabs/Effects/Attacks/MeleeSwing");
        }

        private void Update()
        {
            if (_shouldAttack)
            {
                TryMeleeAttack();
                //Attacking = true;
            }
            else
                //Attacking = false;

            if (Input.GetKeyDown(KeyCode.K))
            {
                Player.instance.TakeDamage(0);
            }
        }

        void TryMeleeAttack()
        {
            // Calculate whether we can attack now
            var timeNow = Time.time;
            var timeSinceLastAttack = timeNow - _timeAtLastMelee;

            // If we attacked too short a duration ago, return (do not proceed)
            if (timeSinceLastAttack < _meleeDelay)
                return;

            _timeAtLastMelee = Time.time;

            // Don't turn if player is not holding the attack key
            if (!Player.instance.Movement.AttackHeld)
            {
                if (Player.instance.Movement.FacingLeft)
                {
                    CreateMeleeEffect(true);
                }
                else
                {
                    CreateMeleeEffect(false);
                }
            }
            // If holding attack key, turn player in the direction he is attacking
            else
            {
                // Turn player in the direction he is attacking and set flag to true
                // (when mouse position x is less than the screen's width split in half, the mouse is on the left side)
                if (Input.mousePosition.x < (float)Screen.width / 2)
                {
                    Player.instance.Movement.SetFacing(left: true);
                    CreateMeleeEffect(flipX: true);
                }
                else
                {
                    // Only change it if we're facing left currently; otherwise leave it as is
                    if (Player.instance.Movement.FacingLeft)
                        Player.instance.Movement.SetFacing(left: false);

                    CreateMeleeEffect(flipX: false);
                }
            }

            // Check for enemies hit
            Vector2 overlapCenter = attackOrigin.position;

            // Move center depending on player facing
            overlapCenter += Player.instance.Movement.FacingLeft ? Vector2.left : Vector2.right;
            var hitCols = Physics2D.OverlapBoxAll(overlapCenter, _meleeHitBox, 0);

            foreach (var col in hitCols)
            {
                if (!col.CompareTag("Enemy"))
                    continue;

                // TODO: Actually change the damage given depending on attack type
                GameManager.Instance.EnemyHit(col.gameObject, Mathf.RoundToInt(_meleeDamage));
            }
        }

        // Actually receive the upgrade and update the player's stats
        // according to the upgrade
        public void ReceiveUpgrade(SkillUpgrade upgrade)
        {
            if (upgrade == null)
                return;

            var effects = upgrade.StatChanges;
            for (int i = 0; i < effects.Count; i++)
            {
                // Too small of a difference to take into account?
                if (Mathf.Approximately(effects[i].Difference, 0))
                    continue;

                var fxType = effects[i].AffectedStat;

                switch (fxType)
                {
                    case SkillUpgrade.StatChange.Stat.AttackSpeed:
                        _meleeDelay *= effects[i].Multiplier;

                        // TODO: Affect possible projectile attack speed
                        break;
                    case SkillUpgrade.StatChange.Stat.AttackDamage:
                        _meleeDamage *= effects[i].Multiplier;

                        // TODO: Affect possible projectile damages
                        break;
                }
            }
        }

        void CreateMeleeEffect(bool flipX)
        {
            var fx = Instantiate(_meleeEffect, attackOrigin.position, Quaternion.identity);
            var scale = fx.transform.localScale;
            if (flipX)
            {
                scale.x *= -1;
                fx.transform.localScale = scale;
            }
        }

        public bool EnemiesInRangeCheck()
        {
            // Check whether there are any enemies in range
            var hitCols = Physics2D.OverlapCircleAll(transform.position, meleeDetectionRadius, enemyLayer.value);
            return hitCols.Length > 0;
        }
        public void TryFireProjectiles()
        {
            //create a fire rate
            var timeNow = Time.time;
            var timeSinceLastAttack = timeNow - _timeAtLastProjectile;

            // If we attacked too short a duration ago, return (do not proceed)
            if (timeSinceLastAttack > _projectileFireRate)
            {
                _timeAtLastProjectile = Time.time;
                var enemies = Physics2D.OverlapCircleAll(transform.position, meleeDetectionRadius, enemyLayer);

                var closestEnemyDistance = Mathf.Infinity;
                Transform target = null;
                foreach (var enemy in enemies)
                {
                    Vector2 bulletDirection = enemy.transform.position - transform.position;
                    var bulletDistance = bulletDirection.magnitude;
                    if (bulletDistance < closestEnemyDistance)
                    {
                        closestEnemyDistance = bulletDistance;
                        target = enemy.transform;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (target == null)
                {
                    return;
                }


                ProjectileManager.CreateProjectile(transform.position, target, Color.cyan);
            }
        }







    }
}