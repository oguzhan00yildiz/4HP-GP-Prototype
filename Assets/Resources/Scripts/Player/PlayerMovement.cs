using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    const float WALK_SPEED = 5.0f;
    private Vector2 _nextMovement;

    // Start is called before the first frame update
    void Start()
    {

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
            _nextMovement = Vector2.Lerp(_nextMovement, Vector2.zero, Time.deltaTime * 25.0f);
        }
        else
        {
            // Else lerp the next movement towards the current desired movement (smooth acceleration)
            _nextMovement = Vector2.Lerp(_nextMovement, screenMovement, Time.deltaTime * 50.0f);
        }

        // Move (note: it would be better to do this in FixedUpdate so movement happens during physics checks
        transform.Translate(_nextMovement * Time.deltaTime);
    }
}
