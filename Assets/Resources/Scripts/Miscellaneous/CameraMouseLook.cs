using UnityEngine;

public class CameraMouseLook : MonoBehaviour
{
    public bool EnableMouseLook;
    [SerializeField] private Transform _center;
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _lerpFactor;
    private Vector2 _mouseScreenPos;
    private Vector2 _camWorldPos;
    public Vector2 MouseScreenPosition { get; private set; }
    public CameraShake Shaker { get; private set; }

    private void Start()
    {
        Shaker = GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!EnableMouseLook)
            return;

        // Find mouse position and normalize with screen dimensions
        _mouseScreenPos = new Vector2
        {
            x = (Input.mousePosition.x - Screen.width / 2) / Screen.width,
            y = (Input.mousePosition.y - Screen.height / 2) / Screen.height,
        };

        // Find target position by center + offset clamped
        Vector2 targetPos = (Vector2)_center.position
            + Vector2.right * Mathf.Clamp(_mouseScreenPos.x * _maxDistance, -_maxDistance, _maxDistance)
            + Vector2.up * Mathf.Clamp(_mouseScreenPos.y * _maxDistance, -_maxDistance, _maxDistance);

        MouseScreenPosition = targetPos.normalized;

        // Convert to 3D position and find original Z offset
        Vector3 targetPos3d = (Vector3)targetPos;
        targetPos3d.z = transform.position.z;

        // Set position
        transform.position = Vector3.Lerp(transform.position, targetPos3d, _lerpFactor * Time.deltaTime);
    }
}
