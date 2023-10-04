using Global;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace PlayerLogic
{
    public class PlayerAttackHandler : MonoBehaviour
    {
        #region StatInfoClasses
        public abstract class PlayerStatInfo
        {
            public List<StatUpgrade> Upgrades { get; set; }

            protected int LastKnownListLength;
            protected Dictionary<StatUpgrade.Stat, float> LastKnownStats;
            protected Dictionary<StatUpgrade.Stat, float> InitialStats;

            public float GetInitialStat(StatUpgrade.Stat stat)
            {
                return InitialStats[stat];
            }

            public float GetTotalStat(StatUpgrade.Stat stat)
            {
                // If the list of upgrades is the same length as last time, don't recalculate
                if (LastKnownListLength == Upgrades.Count)
                    return LastKnownStats[stat];

                float startingValue = GetInitialStat(stat);

                float total = startingValue;

                foreach (var upgrade in Upgrades)
                {
                    var statChanges = upgrade.StatChanges;

                    if(statChanges.Count == 0)
                        continue;

                    foreach (var change in statChanges)
                    {
                        if(change.AffectedStat != stat)
                            continue;

                        total *= change.Multiplier;
                    }
                }

                return total;
            }

            public abstract void AddUpgrade(StatUpgrade upgrade);
        }
        
        private class TankStats : PlayerStatInfo
        {
            public TankStats()
            {
                _tankUpgrades = new List<TankUpgrade>();
                Upgrades = new List<StatUpgrade>();
                InitialStats = new Dictionary<StatUpgrade.Stat, float>
                {
                    { StatUpgrade.Stat.AttackDamage, Const.Player.STATS_TANK_ATTACK_DAMAGE },
                    { StatUpgrade.Stat.AttackSpeed, Const.Player.STATS_TANK_ATTACK_SPEED },
                    { StatUpgrade.Stat.CritChance, Const.Player.STATS_TANK_CRIT_CHANCE },
                    { StatUpgrade.Stat.CritDamage, Const.Player.STATS_TANK_CRIT_DAMAGE },
                    { StatUpgrade.Stat.MaxHealth, Const.Player.STATS_TANK_HEALTH },
                    { StatUpgrade.Stat.MoveSpeed, Const.Player.STATS_TANK_SPEED },
                    { StatUpgrade.Stat.Armor, Const.Player.STATS_TANK_ARMOR }
                };
            }

            private List<TankUpgrade> _tankUpgrades;
            public override void AddUpgrade(StatUpgrade upgrade)
            {
                switch(upgrade)
                {
                    case null:
                        Debug.LogWarning("Tried to add null upgrade to tank upgrade list.");
                        return;
                    case TankUpgrade tankUpgrade:
                        _tankUpgrades.Add(tankUpgrade);
                        break;
                    default:
                        Upgrades.Add(upgrade);
                        break;
                }
            }
        }

        private class ArcherStats : PlayerStatInfo
        {
            public ArcherStats()
            {
                _archerUpgrades = new List<ArcherUpgrade>();
                Upgrades = new List<StatUpgrade>();
                InitialStats = new Dictionary<StatUpgrade.Stat, float>
                {
                    { StatUpgrade.Stat.AttackDamage, Const.Player.STATS_ARCHER_ATTACK_DAMAGE },
                    { StatUpgrade.Stat.AttackSpeed, Const.Player.STATS_ARCHER_ATTACK_SPEED },
                    { StatUpgrade.Stat.CritChance, Const.Player.STATS_ARCHER_CRIT_CHANCE },
                    { StatUpgrade.Stat.CritDamage, Const.Player.STATS_ARCHER_CRIT_DAMAGE },
                    { StatUpgrade.Stat.MaxHealth, Const.Player.STATS_ARCHER_HEALTH },
                    { StatUpgrade.Stat.MoveSpeed, Const.Player.STATS_ARCHER_SPEED },
                    { StatUpgrade.Stat.Armor, Const.Player.STATS_ARCHER_ARMOR }
                };
            }

            private List<ArcherUpgrade> _archerUpgrades;
            public override void AddUpgrade(StatUpgrade upgrade)
            {
                switch(upgrade)
                {
                    case null:
                        Debug.LogWarning("Tried to add null upgrade to archer upgrade list.");
                        return;
                    case ArcherUpgrade archerUpgrade:
                        _archerUpgrades.Add(archerUpgrade);
                        break;
                    default:
                        Upgrades.Add(upgrade);
                        break;
                }
            }
        }
        #endregion

        public Transform AttackOrigin => _attackOrigin;

        [HideInInspector]
        private Transform _attackOrigin;

        // To be used in tandem with movement script
        // so it doesn't flip the player around when trying to attack
        [HideInInspector]
        public bool Attacking { get; private set; }

        private PlayerMovement _movement;

        // This is a cached reference to a prefab
        private static GameObject _meleeEffect;

        private float _timeAtLastMelee = 0;
        private float _timeAtLastProjectile = 0;

        [SerializeField] private float _meleeDamage;
        [SerializeField] LayerMask _enemyLayer;

        private static GameObject _arrowCache;

        private PlayerStatInfo _stats;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _stats
                = GameManager.Player.Character == Player.PlayerCharacter.Archer
                    ? new ArcherStats()
                    : new TankStats();

            _attackOrigin = transform.Find("AttackOrigin");
            _movement = GameManager.Player.Movement;
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

            float attackDelay 
                = _stats.GetTotalStat(StatUpgrade.Stat.AttackSpeed);

            float meleeRange
                = _stats.GetTotalStat(StatUpgrade.Stat.MeleeRange);

            // If we attacked too short a duration ago, return (do not proceed)
            if (timeSinceLastAttack < attackDelay)
                return;

            _timeAtLastMelee = Time.time;

            // Don't turn if player is not holding the attack key
            if (!GameManager.Player.Movement.AttackHeld)
            {
                CreateMeleeEffect(_movement.FacingLeft);
            }
            // If holding attack key, turn player in the direction he is attacking
            else
            {
                // Turn player in the direction he is attacking and set flag to true
                // (when mouse position x is less than the screen's width split in half, the mouse is on the left side)
                if (Input.mousePosition.x < (float)Screen.width / 2)
                {
                    _movement.SetFacing(left: true);
                    CreateMeleeEffect(flipX: true);
                }
                else
                {
                    // Only change it if we're facing left currently; otherwise leave it as is
                    if (_movement.FacingLeft)
                        _movement.SetFacing(left: false);

                    CreateMeleeEffect(flipX: false);
                }
            }

            // Check for enemies hit
            Vector2 overlapCenter = _attackOrigin.position;

            // Move center depending on player facing
            overlapCenter += _movement.FacingLeft ? Vector2.left : Vector2.right;
            var hitCols = Physics2D.OverlapBoxAll(overlapCenter, Vector2.one * meleeRange, 0);

            foreach (var col in hitCols)
            {
                if (!col.CompareTag("Enemy"))
                    continue;

                // Get damage from stats
                var damage = _stats.GetTotalStat(StatUpgrade.Stat.AttackDamage);

                GameManager.Instance.EnemyHit(col.gameObject, Mathf.RoundToInt(damage));
            }
        }

        // Actually receive the upgrade and update the player's stats
        // according to the upgrade
        public void ReceiveUpgrade(StatUpgrade upgrade)
        {
            if (upgrade == null)
                return;

            if (upgrade is ArcherUpgrade 
                && GameManager.Player.Character == Player.PlayerCharacter.Archer)
            {
                // TODO: Implement multi-shot, burst shot, etc.
            }
            else if (upgrade is TankUpgrade
                     && GameManager.Player.Character == Player.PlayerCharacter.Tank)
            {
                // TODO: Implement shield hit upgrades etc.
            }

            /*
            var statChanges = upgrade.StatChanges;
            for (int i = 0; i < statChanges.Count; i++)
            {
                // Too small of a difference to take into account?
                if (Mathf.Approximately(statChanges[i].Difference, 0))
                    continue;

                var stat = statChanges[i].AffectedStat;

                switch (stat)
                {
                    case StatUpgrade.StatChange.Stat.AttackSpeed:
                        _meleeDelay *= statChanges[i].Multiplier;
                        _projectileFireDelay *= statChanges[i].Multiplier;

                        // TODO: Affect possible projectile attack speed
                        break;
                    case StatUpgrade.StatChange.Stat.AttackDamage:
                        _meleeDamage *= statChanges[i].Multiplier;

                        // TODO: Affect possible projectile damages
                        break;
                }
            }
            */
        }

        void CreateMeleeEffect(bool flipX)
        {
            var fx = Instantiate(_meleeEffect, _attackOrigin.position, Quaternion.identity);
            var scale = fx.transform.localScale;
            if (flipX)
            {
                scale.x *= -1;
                fx.transform.localScale = scale;
            }
        }

        public bool ShouldFireProjectiles(out Collider2D[] collidersHit)
        {
            // Check whether there are any enemies in range
            collidersHit
                = Physics2D.OverlapCircleAll(
                    transform.position, Const.Player.PROJECTILE_FIRE_TRIGGER_RADIUS, _enemyLayer.value);

            return collidersHit.Length > 0;
        }

        public bool ShouldMelee(out Collider2D[] collidersHit)
        {
            if (Player.instance.Character == Player.PlayerCharacter.Archer)
            {
                collidersHit = null;
                return false;
            }

            // Check whether there are any enemies in range
            collidersHit
                = Physics2D.OverlapCircleAll(
                    transform.position, Const.Player.MELEEATTACK_TRIGGER_RADIUS, _enemyLayer.value);

            return collidersHit.Length > 0;
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
            if (timeSinceLastAttack < _stats.GetTotalStat(StatUpgrade.Stat.AttackSpeed)) 
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

            var proj = projectileObj.GetComponent<ArrowProjectile>();

            if (proj == null)
            {
                Debug.LogError($"Projectile {projectileObj.name} doesn't have an ArrowProjectile component!", projectileObj);
                return;
            }

            proj.InitializeProjectileWithVector(origin, target.position);
        }
    }
}