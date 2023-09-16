using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    const float WALK_SPEED = 5.0f;
    private Vector2 _nextMovement;
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        if(_rb == null)
        {
            Debug.LogWarning("Player rigidbody is null, you stupid idiot!!");
        }
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

        // Multiply movement vector by walk speed and normalize to frame time
        screenMovement *= WALK_SPEED;

        // Lerp movement towards 0 if the player is not desiring to move (smoother stop)
        if(input.magnitude <= 0)
        {
            _nextMovement = Vector2.Lerp(_nextMovement, Vector2.zero, Time.deltaTime * 15.0f);
        }
        else
        {
            // Else lerp the next movement towards the current desired movement (smooth acceleration)
            _nextMovement = Vector2.Lerp(_nextMovement, screenMovement, Time.deltaTime * 25.0f);
        }

        // Movement updates happen in FixedUpdate since Unity physics run in that timeframe
    }

    private void FixedUpdate()
    {
        // Cast to Vector2 required for transform.position change
        _rb.MovePosition((Vector2)transform.position + _nextMovement * Time.fixedDeltaTime);
    }
}
