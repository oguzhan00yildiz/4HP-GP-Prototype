using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menus
{
    public class LogoAnimation : MonoBehaviour
    {
        public float maxScale;
        Vector3 origScale;
        public float animSpeed;

        // Start is called before the first frame update
        void Start()
        {
            origScale = transform.localScale;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 zoom = new Vector3(origScale.x + Mathf.PingPong(Time.time * animSpeed, maxScale) / 2,
                origScale.y + Mathf.PingPong(Time.time * animSpeed, maxScale), transform.localScale.z);
            transform.localScale = zoom;
        }
    }
}
