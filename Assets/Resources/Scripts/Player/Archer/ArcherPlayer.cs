using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerLogic
{
    public sealed class ArcherPlayer : BasePlayer
    {
        private struct ShotTimer
        {
            public float LastShotTime;

            public bool EnoughWaiting(float fireDelay)
                => Time.time - LastShotTime > fireDelay;
        }

        private ShotTimer _burstShotTimer;
        private ShotTimer _multiShotTimer;
        private ShotTimer _baseShotTimer;

        private ArcherStats Stats
        {
            get => (ArcherStats)StatInfo;
            set => StatInfo = value;
        }

        private float MoveSpeed
            => Stats.GetTotalStat(StatUpgrade.Stat.MoveSpeed);

        private float MeleeRange
            => Stats.GetTotalStat(StatUpgrade.Stat.MeleeRange);

        private float AttackSpeed
            => Stats.GetTotalStat(StatUpgrade.Stat.AttackSpeed);

        private float MultiShotDelay
            => Stats.TryGetMultiShotStats(out var upgrade) 
                ? upgrade.Value.FireDelay 
                : 0;

        private float MultiShotDisperseAngle
            => Stats.TryGetMultiShotStats(out var upgrade) 
                ? upgrade.Value.DisperseAngle 
                : 0;

        private int MultiShotArrows
        => Stats.TryGetMultiShotStats(out var upgrade) 
                ? upgrade.Value.ArrowCount 
                : 0;

        private float BurstShotDelay
        => Stats.TryGetBurstShotStats(out var upgrade) 
                ? upgrade.Value.FireDelay 
                : 0;

        private float BurstShotDisperseAngle
        => Stats.TryGetBurstShotStats(out var upgrade) 
                ? upgrade.Value.BurstDelay 
                : 0;

        private int BurstShotArrows
        => Stats.TryGetBurstShotStats(out var upgrade) 
                ? upgrade.Value.ArrowCount 
                : 0;

        private float AttackDelay
            => 1.0f / AttackSpeed;

        private static GameObject _arrowCache;

        public override IPlayer.PlayerCharacter Character
        {
            get => IPlayer.PlayerCharacter.Archer;
            protected set { }
        }

        public override Animator Animator { get; protected set; }

        public override void ReceiveUpgrade(StatUpgrade upgrade)
        {
            if (upgrade == null)
                return;

            if (upgrade is ArcherUpgrade)
            {
                // TODO: Implement shield hit upgrades etc.
            }
            else
            {
                Stats.AddUpgrade(upgrade);
            }
        }

        public override void Attack()
        {
            if (ShouldFireProjectiles(out var enemies))
                TryFireProjectiles(enemies);
        }

        public override void Initialize()
        {
            Stats = new ArcherStats();
            Health = Stats.GetInitialStat(StatUpgrade.Stat.MaxHealth);

            // Initialize shot timers (track whether can shoot again)
            _baseShotTimer = new();
            _multiShotTimer = new();

            if ((CameraController = FindObjectOfType<CameraController>()) == null)
            {
                CameraController = CreateCameraWithController();
            }

            if ((AttackOrigin = transform.Find("AttackOrigin")) == null)
            {
                AttackOrigin = new GameObject("AttackOrigin").transform;
                AttackOrigin.transform.parent = transform;
                AttackOrigin.transform.localPosition = Vector3.zero;
            }

            if ((Rb = GetComponent<Rigidbody2D>()) == null)
            {
                Debug.LogWarning("Player rigidbody is null, you stupid idiot!!");
                Rb = gameObject.AddComponent<Rigidbody2D>();
                Rb.gravityScale = 0;
            }

            Model = GameObject.FindWithTag("PlayerModel").transform;

            ModelRenderer = Model.GetComponentInChildren<SpriteRenderer>();

            PlayerAnim = Model.GetComponentInChildren<Animator>();

            HealthBar = transform.Find("WorldCanvas/HealthBar").GetComponent<Slider>();

            HealthBar.gameObject.SetActive(false);

            DamagePopUpPrefab = Resources.Load<GameObject>("Prefabs/Player/DamagePopUp");

            Initialized = true;
        }

        public override void Update()
        {
            if (!Initialized)
                return;

            // Auto-fire
            Attack();

            UpdateMovement(MoveSpeed);

            FramesSinceLastDamage++;
        }

        public override void FixedUpdate()
        {
            if (!Initialized)
                return;

            UpdateMovementFixed();
        }

        public bool ShouldFireProjectiles(out Collider2D[] collidersHit)
        {
            // Check whether there are any enemies in range
            collidersHit
                = Physics2D.OverlapCircleAll(
                    transform.position, Const.Player.PROJECTILE_FIRE_TRIGGER_RADIUS, EnemyLayer.value);

            return collidersHit.Length > 0;
        }

        private void FireMultiShot(Vector3 enemyPosition, int numSpreadArrows, int angleOffset)
        {
            float angleOffsetRad = angleOffset * Mathf.Deg2Rad;

            Vector3 direction = enemyPosition - transform.position;

            float centralAngleRad = Mathf.Atan2(direction.y, direction.x);

            int centralIndex = numSpreadArrows / 2;

            for (var i = 0; i < numSpreadArrows; i++)
            {
                float angle = centralAngleRad;
                if (i < centralIndex)
                {
                    angle -= angleOffsetRad;
                }
                else if (i > centralIndex)
                {
                    angle += angleOffsetRad;
                }

                Vector3 targetDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
                Vector3 targetPosition = transform.position + (targetDirection.normalized * direction.magnitude);

                FireArrow(transform.position, targetPosition);
            }

            _multiShotTimer.LastShotTime = Time.time;
        }

        private void FireBurstShot(Vector3 enemyPosition, int numArrows, float delay)
        {
            StartCoroutine(BurstShotCoroutine(enemyPosition, numArrows, delay));
            _burstShotTimer.LastShotTime = Time.time;
        }

        private IEnumerator BurstShotCoroutine(Vector3 enemyPosition, int numArrows, float delay)
        {
            for (int i = 0; i < numArrows; i++)
            {
                FireArrow(transform.position, enemyPosition);
                yield return new WaitForSeconds(delay);
            }
        }

        public Collider2D GetClosestEnemy(Collider2D[] enemyColliders)
        {
            var closestEnemyDistance = Mathf.Infinity;
            Collider2D target = null;
            foreach (var enemy in enemyColliders)
            {
                Vector2 targetDirection = enemy.transform.position - transform.position;
                var targetDistance = targetDirection.magnitude;

                if (targetDistance > closestEnemyDistance)
                    continue;

                closestEnemyDistance = targetDistance;
                target = enemy.GetComponentInChildren<Collider2D>();
            }

            if (target == null)
            {
                return null;
            }

            return target;
        }

        public void TryFireProjectiles(Collider2D[] enemies)
        {
            if (enemies.Length == 0)
            {
                return;
            }

            Collider2D target = GetClosestEnemy(enemies);
            if (target == null)
            {
                return;
            }

            // Fire regular arrow if enough time has passed
            if (_baseShotTimer.EnoughWaiting(AttackDelay))
            {
                FireArrow(transform.position, enemies[0].transform.position);
            }

            // Fire multi-shot if enough time has passed
            if (_multiShotTimer.EnoughWaiting(AttackDelay * 5))
            {
                FireMultiShot(target.transform.position, 3, 15);
            }

            // Fire burst shot if enough time has passed
            if (_burstShotTimer.EnoughWaiting(AttackDelay * 7))
            {
                FireBurstShot(target.transform.position, 5, 0.1f);
            }
        }

        static GameObject InstantiateArrow()
        {
            if (_arrowCache == null)
            {
                _arrowCache = Resources.Load("Prefabs/Projectiles/BasicArrow") as GameObject;
            }

            return Instantiate(_arrowCache);
        }

        public void FireArrow(Vector2 origin, Vector2 target)
        {
            GameObject projectileObj = InstantiateArrow();
            projectileObj.transform.position = transform.position;

            var proj = projectileObj.GetComponent<ArrowProjectile>();

            if (proj == null)
            {
                Debug.LogError($"Projectile {projectileObj.name} doesn't have an ArrowProjectile component!", projectileObj);
                return;
            }

            proj.InitializeProjectileWithVector(origin, target);
            _baseShotTimer.LastShotTime = Time.time;
        }   

        public void FireArrowHoming(Vector2 origin, Transform target)
        {
            GameObject projectileObj = InstantiateArrow();
            projectileObj.transform.position = transform.position;

            var proj = projectileObj.GetComponent<ArrowProjectile>();

            if (proj == null)
            {
                Debug.LogError($"Projectile {projectileObj.name} doesn't have an ArrowProjectile component!", projectileObj);
                return;
            }

            _baseShotTimer.LastShotTime = Time.time;
            proj.InitializeProjectileWithTransform(origin, target);
        }
    }
}