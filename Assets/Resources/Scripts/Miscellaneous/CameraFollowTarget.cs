using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;

        Vector3 curPos = transform.position;

        // set z to remain the same as current position z so camera doesn't go forward/backward in 3D space
        Vector3 targetPos = new Vector3()
        {
            x = target.position.x,
            y = target.position.y,
            z = curPos.z
        };

        transform.position = Vector3.Lerp(curPos, targetPos, Time.deltaTime * 15.0f);
    }
}
