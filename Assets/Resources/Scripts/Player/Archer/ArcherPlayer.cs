using UnityEngine;
using UnityEngine.UI;

namespace PlayerLogic
{
    public sealed class ArcherPlayer : BasePlayer
    {
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

        public void TryFireProjectiles(Collider2D[] enemies)
        {
            // create a fire rate
            var timeNow = Time.time;
            var timeSinceLastAttack = timeNow - TimeAtLastAttack;

            if (enemies.Length == 0)
            {
                return;
            }

            // If we attacked too short a duration ago, return (do not proceed)
            if (timeSinceLastAttack < AttackDelay)
                return;

            TimeAtLastAttack = Time.time;

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