using Global;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

namespace PlayerLogic
{
    // Base player class
    public abstract class BasePlayer : MonoBehaviour, IPlayer
    {
        // Public: accessible by any class
        public bool TryCrit(out float critDamage)
        {
            critDamage = 0;
            if (Random.Range(0, 100) < StatInfo.GetTotalStat(StatUpgrade.Stat.CritChance))
            {
                critDamage = StatInfo.GetTotalStat(StatUpgrade.Stat.CritDamage);
                return true;
            }
            return false;
        }
        public bool IsAIPlayer
            => _isAIPlayer && !IsLeader;
        public bool IsLeader
            => _isLeader;

        public PlayerStatInfo StatInfo { get; set; }
        public Vector2 Position
            => transform.position;

        // Protected: only accessible by this class and its children
        protected LayerMask EnemyLayer => LayerMask.GetMask("Enemy");
        protected float Health { get; private protected set; }
        protected float MaxHealth
            => StatInfo.GetTotalStat(StatUpgrade.Stat.MaxHealth);
        protected Slider HealthBar { get; private protected set; }
        protected float TimeAtLastAttack { get; private protected set; }
        protected CameraController CameraController { get; private protected set; }
        protected Transform AttackOrigin { get; private protected set; }
        protected Vector2 NextMovement { get; private set; }
        protected Rigidbody2D Rb { get; private protected set; }
        protected Transform Model { get; private protected set; }
        protected SpriteRenderer ModelRenderer { get; private protected set; }
        protected Animator PlayerAnim { get; private protected set; }
        protected bool FacingLeft { get; private protected set; }
        protected bool Initialized { get; private protected set; }
        protected bool AttackHeld
            => Input.GetButton("Fire1");
        protected int FramesSinceLastDamage { get; private protected set; }

        protected GameObject DamagePopUpPrefab;

        private bool _isLeader;
        private bool _isAIPlayer;

        protected BotLogic CPULogic;

        protected struct BotLogic
        {
            private bool _forcedMove;
            public static readonly float StoppingDistance = 2.5f;
            public static readonly float MaxDistanceFromPlayer = 12.0f;

            public bool ShouldForceMoveToPlayer(float distFromPlayer)
            {
                var force = distFromPlayer > MaxDistanceFromPlayer;
                if (force)
                    _forcedMove = true;
                return force;
            }

            public bool ShouldMoveTowardsPlayer(float distFromPlayer)
            {
                if (_forcedMove && !IsTooCloseToPlayer(distFromPlayer))
                    return true;

                return !IsTooCloseToPlayer(distFromPlayer);
            }

            public bool IsTooCloseToPlayer(float distFromPlayer)
            {
                bool tooClose = distFromPlayer < StoppingDistance / 2;
                if(tooClose)
                    _forcedMove = false;
                return tooClose;
            }

            public static float GetDistanceToPlayer(Vector2 myPos)
            {
                return Vector2.Distance(GameManager.LeaderPlayer.Position, myPos);
            }
        }

        // No constructor needed, since we're using Unity's MonoBehaviour

        public void SetAsLeader()
        {
            if (GameManager.LeaderPlayer != this)
                return;

            _isLeader = true;
        }

        public void SetAsAIPlayer()
        {
            _isAIPlayer = true;
        }

        protected Vector2 CalculateAI_Input()
        {
            Vector2 inputDir;

            var playerPos = GameManager.LeaderPlayer.Position;
            var myPos = Position;

            float distanceToPlayer = Vector2.Distance(playerPos, myPos);

            bool forceFollow = CPULogic.ShouldForceMoveToPlayer(distanceToPlayer);
            bool shouldMoveTowardsPlayer = CPULogic.ShouldMoveTowardsPlayer(distanceToPlayer);
            bool tooClose = CPULogic.IsTooCloseToPlayer(distanceToPlayer);

            // if should force AI to move towards the player
            if (forceFollow)
            {
                inputDir = (playerPos - myPos).normalized;
                return inputDir;
            }

            // if too close to player, move away
            if (tooClose)
            {
                forceFollow = false;
                inputDir = (myPos - playerPos).normalized;
                return inputDir;
            }

            // if the checks did not fire, we can focus on attacking enemies
            var visibleEnemies = Physics2D.OverlapCircleAll(myPos, 10.0f, EnemyLayer);
            if (visibleEnemies.Length > 0)
            {
                var closestEnemy = visibleEnemies[0];
                float closestDist = Vector2.Distance(myPos, closestEnemy.transform.position);
                Vector2 closestEnemyPos = closestEnemy.transform.position;

                foreach (var enemy in visibleEnemies)
                {
                    float dist = Vector2.Distance(myPos, enemy.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestEnemy = enemy;
                    }
                }

                inputDir = (closestEnemyPos - myPos).normalized;
                return inputDir;
            }


            return Vector2.zero;
        }

        public CameraController CreateCameraWithController()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Player/PlayerCamera");
            if (prefab == null)
            {
                Debug.LogError("Player camera prefab is null!");
                return null;
            }

            var camObj = Instantiate(prefab, new Vector3(0, 0, -10), Quaternion.identity);

            var cam = camObj.GetComponent<CameraController>();
            cam.Initialize(transform);
            return cam;
        }

        // Virtual: can be overridden by children
        public virtual void TakeDamage(int amount)
        {
            if (FramesSinceLastDamage < Const.Player.INVULNERABILITY_FRAMES)
                return;

            FramesSinceLastDamage = 0;

            Health -= amount;
            DamageEffect(amount);
        }

        protected virtual void DamageEffect(int damageAmount)
        {
            HealthBar.gameObject.SetActive(true);
            HealthBar.value = Health / MaxHealth;
            DisplayDamageNumber(transform.position, damageAmount);
            GameManager.CanvasManager.DisplayDamageOverlay();

            if (GameManager.DebugMode)
            {
                Debug.Log("Player took damage");
            }
        }

        protected virtual void UpdateHealthBar()
        {
            HealthBar.value = Health / MaxHealth;
        }

        // Ossi's method
        protected virtual void DisplayDamageNumber(Vector2 origin, int damageAmount)
        {
            float minX = Const.Effects.POPUP_MIN_X_OFFSET;
            float maxX = Const.Effects.POPUP_MAX_X_OFFSET;

            float rnd = Random.Range(minX, maxX);
            var newPos = new Vector3(rnd, 0, 0);
            var popUpObject = Instantiate(DamagePopUpPrefab, origin + (Vector2)newPos, Quaternion.identity);
            var tmpText = popUpObject.GetComponentInChildren<TextMeshPro>();
            tmpText.text = damageAmount.ToString();
            Destroy(popUpObject, .5f);
        }

        public virtual void UpdateMovement(float walkSpeed)
        {
            Vector2 desiredMovement;
            if (IsAIPlayer)
            {
                desiredMovement = CalculateAI_Input();
            }
            else
            {
                desiredMovement = new Vector2()
                {
                    x = Input.GetAxisRaw("Horizontal"),
                    y = Input.GetAxisRaw("Vertical"),
                };
            }

            // Align vectors to directions ON SCREEN
            // Also normalize them so their length is 1
            // TODO: This is completely unnecessary, since we're 2D top-down

            Vector2 screenRight = Vector2.right;
            Vector2 screenUp = Vector2.up;

            Vector2 screenMovement = (Vector2.zero
                + screenRight * desiredMovement.x
                + screenUp * desiredMovement.y).normalized;

            // Multiply movement vector by walk speed
            screenMovement *= walkSpeed;

            // Lerp movement towards 0 if the player is not desiring to move (smoother stop)
            bool moving = desiredMovement.magnitude > 0;

            // Player should face left if they are trying to move left BUT are not attacking
            bool facesLeft = NextMovement.x < 0;
            if (moving)
                SetFacing(facesLeft);

            // Player is backtracking if facing left but moving right or vice versa
            bool backTracking =
                (FacingLeft && NextMovement.x > 0)
                || (!FacingLeft && NextMovement.x < 0);

            // Reverse animation (multiply animation speed by -1) if backtracking
            if (backTracking)
            {
                PlayerAnim.SetFloat("MoveSpeed", -1);
            }
            else
                PlayerAnim.SetFloat("MoveSpeed", 1);

            // Ossi! Simplified expression to set animator bool equal to this bool's value
            PlayerAnim.SetBool("IsMoving", moving);

            if (moving)
            {
                // If moving, lerp the next movement towards the current desired movement (smooth acceleration)
                NextMovement = Vector2.Lerp(NextMovement, screenMovement, Time.deltaTime * 25.0f);
            }
            else
            {
                // If not moving, lerp towards zero
                NextMovement = Vector2.Lerp(NextMovement, Vector2.zero, Time.deltaTime * 15.0f);
            }

            // Movement updates happen in FixedUpdate since Unity physics run in that timeframe
        }

        public virtual void UpdateMovementFixed()
        {
            // Cast to Vector2 required for transform.position change since _rb is rigidbody2D
            Rb.MovePosition((Vector2)transform.position + NextMovement * Time.fixedDeltaTime);
        }

        // Set with true parameter if facing left, false for right
        public virtual void SetFacing(bool left)
        {
            FacingLeft = left;

            // Flip player sprite if facing left
            ModelRenderer.flipX = left;

            // Flip player attack origin when facing left
            AttackOrigin.localScale = new Vector3()
            {
                x = left ? -1 : 1,
                y = 1,
                z = 1
            };
        }

        // Abstract: must be implemented by children
        public abstract IPlayer.PlayerCharacter Character { get; protected set; }
        public abstract Animator Animator { get; protected set; }
        public abstract void Initialize();
        public abstract void Attack();
        public abstract void ReceiveUpgrade(StatUpgrade upgrade);
        public abstract void Update();
        public abstract void FixedUpdate();
    }
}