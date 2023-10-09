using Global;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerLogic
{
    public sealed class TankPlayer : BasePlayer
    {
        private TankStats Stats
        {
            get => (TankStats)StatInfo;
            set => StatInfo = value;
        }

        public override IPlayer.PlayerCharacter Character
        {
            get => IPlayer.PlayerCharacter.Tank;
            protected set { }
        }

        // This is a cached reference to a prefab
        private static GameObject _meleeEffect;

        private float MoveSpeed
            => Stats.GetTotalStat(StatUpgrade.Stat.MoveSpeed);

        private float MeleeRange
            => Stats.GetTotalStat(StatUpgrade.Stat.MeleeRange);

        private float AttackSpeed
            => Stats.GetTotalStat(StatUpgrade.Stat.AttackSpeed);

        private float AttackDelay
            => 1.0f / AttackSpeed;

        private float KnockbackForce
            => Stats.GetTotalStat(StatUpgrade.Stat.Knockback);

        public override Animator Animator { get; protected set; }

        public override void Initialize()
        {
            Stats = new TankStats();
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

            _meleeEffect = Resources.Load<GameObject>("Prefabs/Effects/Attacks/MeleeSwing");

            Model = GameObject.FindWithTag("PlayerModel").transform;

            ModelRenderer = Model.GetComponentInChildren<SpriteRenderer>();

            HealthBar = transform.Find("WorldCanvas/HealthBar").GetComponent<Slider>();

            HealthBar.gameObject.SetActive(false);

            DamagePopUpPrefab = Resources.Load<GameObject>("Prefabs/Player/DamagePopUp");

            PlayerAnim = Model.GetComponentInChildren<Animator>();

            Initialized = true;
        }

        public override void Attack()
        {
            if (ShouldAttack())
                TryMeleeAttack();
        }

        public override void ReceiveUpgrade(StatUpgrade upgrade)
        {
            if (upgrade == null)
                return;

            if (upgrade is TankUpgrade)
            {
                // TODO: Implement shield hit upgrades etc.
            }
            else
            {
                Stats.AddUpgrade(upgrade);
            }
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

        private void TryMeleeAttack()
        {
            // Calculate whether we can attack now
            var timeNow = Time.time;
            var timeSinceLastAttack = timeNow - TimeAtLastAttack;

            // If we attacked too short a duration ago, return (do not proceed)
            if (timeSinceLastAttack < AttackDelay)
                return;

            TimeAtLastAttack = Time.time;

            // Don't turn if player is not holding the attack key
            if (!AttackHeld)
            {
                CreateMeleeEffect(FacingLeft);
            }
            // If holding attack key, turn player in the direction he is attacking
            else
            {
                // Turn player in the direction he is attacking and set flag to true
                // (when mouse position x is less than the screen's width split in half, the mouse is on the left side)
                if (Input.mousePosition.x < (float)Screen.width / 2)
                {
                    SetFacing(left: true);
                    CreateMeleeEffect(flipX: true);
                }
                else
                {
                    // Only change it if we're facing left currently; otherwise leave it as is
                    if (FacingLeft)
                        SetFacing(left: false);

                    CreateMeleeEffect(flipX: false);
                }
            }

            // Check for enemies hit
            Vector2 overlapCenter = AttackOrigin.position;

            // Move center depending on player facing
            overlapCenter += FacingLeft ? Vector2.left : Vector2.right;
            var hitCols = Physics2D.OverlapBoxAll(overlapCenter, Vector2.one * MeleeRange, 0);

            foreach (var col in hitCols)
            {
                if (!col.CompareTag("Enemy"))
                    continue;

                // Get damage from stats
                var damage = Stats.GetTotalStat(StatUpgrade.Stat.AttackDamage);

                GameManager.Instance.EnemyHitWithKnockback(col.gameObject, Mathf.RoundToInt(damage), transform.position, KnockbackForce);
            }
        }

        private bool ShouldAttack()
        {
            float radius = Const.Player.MELEEATTACK_TRIGGER_RADIUS;

            // Check whether there are any enemies in range
            var collidersHit = Physics2D.OverlapCircleAll(
                transform.position, radius, EnemyLayer.value);

            return collidersHit.Length > 0;
        }

        private void CreateMeleeEffect(bool flipX)
        {
            var fx = Instantiate(_meleeEffect, AttackOrigin.position, Quaternion.identity);
            var scale = fx.transform.localScale;
            if (flipX)
            {
                scale.x *= -1;
                fx.transform.localScale = scale;
            }
        }
    }
}