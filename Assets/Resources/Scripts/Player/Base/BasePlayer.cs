using UnityEngine;

namespace PlayerLogic
{
    // Base player class
    public abstract class BasePlayer : MonoBehaviour, IPlayer
    {
        // Public: accessible by any class
        public PlayerStatInfo StatInfo { get; set; }

        // Protected: only accessible by this class and its children
        protected LayerMask EnemyLayer => LayerMask.GetMask("Enemy");
        protected float Health { get; private protected set; }
        protected float MoveSpd { get; private protected set; }
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

        // No constructor needed, since we're using Unity's MonoBehaviour

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
            Health -= amount;
        }

        public virtual void UpdateMovement(float walkSpeed)
        {
            // store input in vector
            Vector2 input = new Vector2()
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical"),
            };

            // Align vectors to directions ON SCREEN
            // Also normalize them so their length is 1
            Vector2 screenRight = Vector2.right;
            Vector2 screenUp = Vector2.up;

            Vector2 screenMovement = (Vector2.zero
                + screenRight * input.x
                + screenUp * input.y).normalized;

            // Multiply movement vector by walk speed
            screenMovement *= walkSpeed;

            // Lerp movement towards 0 if the player is not desiring to move (smoother stop)
            bool moving = input.magnitude > 0;

            // Player should face left if they are trying to move left BUT are not attacking
            bool facesLeft = NextMovement.x < 0;
            if (!AttackHeld && moving)
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
        public abstract void DamageEffect();
        public abstract void ReceiveUpgrade(StatUpgrade upgrade);
        public abstract void Update();
        public abstract void FixedUpdate();
    }
}