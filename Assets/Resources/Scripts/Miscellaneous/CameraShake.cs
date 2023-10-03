using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform _trans;

    // Start is called before the first frame update
    void Start()
    {
        _trans = transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Shake(float magnitude, float duration)
    {
        StartCoroutine(ShakeCoroutine(magnitude, duration));
    }

    IEnumerator ShakeCoroutine(float magn, float dur)
    {
        // Stop mouselook, let this script handle the position
        CameraMouseLook mouseLook = GetComponent<CameraMouseLook>();
        mouseLook.EnableMouseLook = false;

        Vector3 originalPos = _trans.position;
        for (int x = 0; x < 25; x++)
        {
            // Find random target in XY and set Z to original value
            Vector3 target = originalPos + (Vector3)Random.insideUnitCircle * magn;
            target.z = originalPos.z;

            // Lerp towards target a total of 25 times, or stop if reached already
            for (int i = 0; i < 25; i++)
            {
                float distanceToTarget = Vector2.Distance(_trans.position, target);
                if (distanceToTarget < 0.25f)
                    break;

                _trans.position = Vector3.Lerp(_trans.position, target, Time.deltaTime * 25);
                dur -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (dur <= 0)
                break;
        }

        // Enable mouselook again
        mouseLook.EnableMouseLook = true;
    }
}
