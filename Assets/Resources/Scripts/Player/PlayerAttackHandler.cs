using Global;
using UnityEngine;

namespace PlayerLogic
{
    public class PlayerAttackHandler : MonoBehaviour
    {
        [HideInInspector]
        public Transform AttackOrigin;

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

        //this is setting popupdamage effect settings
        [SerializeField] private float _meleeDamage;
        [SerializeField] private float _projectileFireDelay;
        [SerializeField] LayerMask enemyLayer;
        [SerializeField] private float meleeDetectionRadius;
        [SerializeField] private float projectileDetectionRadius;
        [HideInInspector]
        public static PlayerAttackHandler instance;

        private static GameObject _arrowCache;

        private void OnEnable()
        {
            AttackOrigin = transform.Find("AttackOrigin");
            instance = this;
        }
        private void Start()
        {
            _meleeEffect = Resources.Load<GameObject>("Prefabs/Effects/Attacks/MeleeSwing");
        }

        private void Update()
        {
            TryPerformAttacks();
        }

        private void TryPerformAttacks()
        {
            if (ShouldFireProjectiles(out Collider2D[] enemyColliders))
            {
                TryFireProjectiles(enemyColliders);
            }

            if (ShouldMelee(out _))
            {
                TryMeleeAttack();
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
                CreateMeleeEffect(Player.instance.Movement.FacingLeft);
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
            Vector2 overlapCenter = AttackOrigin.position;

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

            if (upgrade is ArcherUpgrade)
            {
                // TODO: Implement multishot, burst shot, etc.
            }
            else if (upgrade is TankUpgrade)
            {
                // TODO: Implement shield hit upgrades etc.
            }

            var statChanges = upgrade.StatChanges;
            for (int i = 0; i < statChanges.Count; i++)
            {
                // Too small of a difference to take into account?
                if (Mathf.Approximately(statChanges[i].Difference, 0))
                    continue;

                var stat = statChanges[i].AffectedStat;

                switch (stat)
                {
                    case SkillUpgrade.StatChange.Stat.AttackSpeed:
                        _meleeDelay *= statChanges[i].Multiplier;
                        _projectileFireDelay *= statChanges[i].Multiplier;

                        // TODO: Affect possible projectile attack speed
                        break;
                    case SkillUpgrade.StatChange.Stat.AttackDamage:
                        _meleeDamage *= statChanges[i].Multiplier;

                        // TODO: Affect possible projectile damages
                        break;
                }
            }
        }

        void CreateMeleeEffect(bool flipX)
        {
            var fx = Instantiate(_meleeEffect, AttackOrigin.position, Quaternion.identity);
            var scale = fx.transform.localScale;
            if (flipX)
            {
                scale.x *= -1;
                fx.transform.localScale = scale;
            }
        }

        public bool ShouldFireProjectiles(out Collider2D[] enemyColliders)
        {
            // Check whether there are any enemies in range
            enemyColliders
                = Physics2D.OverlapCircleAll(
                    transform.position, projectileDetectionRadius, enemyLayer.value);

            return enemyColliders.Length > 0;
        }

        public bool ShouldMelee(out Collider2D[] enemyColliders)
        {
            if (Player.instance.Character == Player.PlayerCharacter.Archer)
            {
                enemyColliders = null;
                return false;
            }

            // Check whether there are any enemies in range
            enemyColliders
                = Physics2D.OverlapCircleAll(
                    transform.position, meleeDetectionRadius, enemyLayer.value);

            return enemyColliders.Length > 0;
        }
        public void TryFireProjectiles(Collider2D[] enemies)
        {
            // create a fire rate
            var timeNow = Time.time;
            var timeSinceLastAttack = timeNow - _timeAtLastProjectile;

            if (enemies.Length == 0)
            {
                return;
            }

            // If we attacked too short a duration ago, return (do not proceed)
            if (timeSinceLastAttack < _projectileFireDelay) 
                return;

            _timeAtLastProjectile = Time.time;

            var closestEnemyDistance = Mathf.Infinity;
            Transform target = null;
            foreach (var enemy in enemies)
            {
                Vector2 targetDirection = enemy.transform.position - transform.position;
                var targetDistance = targetDirection.magnitude;

                if (targetDistance > closestEnemyDistance)
                    continue;

                closestEnemyDistance = targetDistance;
                target = enemy.transform;
            }
            if (target == null)
            {
                return;
            }

            FireArrow(transform.position, target);
        }

        static GameObject InstantiateArrow()
        {
            if (_arrowCache == null)
            {
                _arrowCache = Resources.Load("Prefabs/Projectiles/BasicArrow") as GameObject;
            }

            return Instantiate(_arrowCache);
        }

        public void FireArrow(Vector2 origin, Transform target)
        {
            GameObject projectileObj = InstantiateArrow();
            projectileObj.transform.position = transform.position;

            ArrowProjectile proj = projectileObj.GetComponent<ArrowProjectile>();

            if (proj == null)
            {
                Debug.LogError($"Projectile {projectileObj.name} doesn't have an ArrowProjectile component!", projectileObj);
                return;
            }

            proj.InitializeProjectileWithVector(origin, target.position);
        }
    }
}