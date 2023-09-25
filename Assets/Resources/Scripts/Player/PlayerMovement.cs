using UnityEngine;

namespace PlayerLogic
{
    public class PlayerMovement : MonoBehaviour
    {
        private Vector2 _nextMovement;
        private Rigidbody2D _rb;

        private Transform _model;

        private SpriteRenderer _modelRenderer;
        private Animator _playerAnim;
        public bool FacingLeft { get; private set; }

        public bool AttackHeld
        {
            get { return Input.GetMouseButton(0); }
        }

        // Start is called before the first frame update
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();

            if (_rb == null)
            {
                Debug.LogWarning("Player rigidbody is null, you stupid idiot!!");
            }

            _model = GameObject.FindWithTag("PlayerModel").transform;


            _modelRenderer = _model.GetComponentInChildren<SpriteRenderer>();

            _playerAnim = _model.GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateMovement();
        }

        void UpdateMovement()
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
            screenMovement *= Const.Player.WALK_SPEED;

            // Lerp movement towards 0 if the player is not desiring to move (smoother stop)
            bool moving = input.magnitude > 0;

            // Player should face left if they are trying to move left BUT are not attacking
            bool facesLeft = _nextMovement.x < 0;
            if (!Player.instance.AttackHandler.Attacking && moving)
                SetFacing(facesLeft);

            // Player is backtracking if facing left but moving right or vice versa
            bool backTracking =
                (FacingLeft && _nextMovement.x > 0)
                || (!FacingLeft && _nextMovement.x < 0);

            // Reverse animation (multiply animation speed by -1) if backtracking
            if(backTracking)
            {
                _playerAnim.SetFloat("MoveSpeed", -1);
            }
            else
                _playerAnim.SetFloat("MoveSpeed", 1);

            // Ossi! Simplified expression to set animator bool equal to this bool's value
            _playerAnim.SetBool("IsMoving", moving);

            if (moving)
            {
                // If moving, lerp the next movement towards the current desired movement (smooth acceleration)
                _nextMovement = Vector2.Lerp(_nextMovement, screenMovement, Time.deltaTime * 25.0f);
            }
            else
            {
                // If not moving, lerp towards zero
                _nextMovement = Vector2.Lerp(_nextMovement, Vector2.zero, Time.deltaTime * 15.0f);
            }

            // Movement updates happen in FixedUpdate since Unity physics run in that timeframe
        }

        private void FixedUpdate()
        {
            // Cast to Vector2 required for transform.position change since _rb is rigidbody2D
            _rb.MovePosition((Vector2)transform.position + _nextMovement * Time.fixedDeltaTime);
        }

        // Set with true parameter if facing left, false for right
        public void SetFacing(bool left)
        {
            FacingLeft = left;

            // Flip player sprite if facing left
            _modelRenderer.flipX = left;

            // Flip player attack origin when facing left
            Player.instance.AttackHandler.attackOrigin.localScale = new Vector3()
            {
                x = left ? -1 : 1,
                y = 1,
                z = 1
            };
        }
    }
}