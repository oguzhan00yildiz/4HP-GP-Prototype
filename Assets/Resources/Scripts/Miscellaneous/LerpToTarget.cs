using UnityEngine;

public class LerpToTarget : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private float _lerpSpeed;

    public void SetTarget(Transform target)
    {
        _target = target;
    }
    public void SetSpeed(float speed)
    {
        _lerpSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null)
            return;

        Vector3 curPos = transform.position;

        // set z to remain the same as current position z so camera doesn't go forward/backward in 3D space
        Vector3 targetPos = new Vector3()
        {
            x = _target.position.x,
            y = _target.position.y,
            z = curPos.z
        };

        transform.position = Vector3.Lerp(curPos, targetPos, _lerpSpeed * Time.deltaTime);
    }
}
