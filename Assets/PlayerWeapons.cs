using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug key
        if (Input.GetKeyDown(KeyCode.R))
        {
            Color randCol = new Color()
            {
                r = Random.value,
                g = Random.value,
                b = Random.value,
                a = 1,
            };

            // Create projectile from position to 0,0,0 with a random color
            ProjectileManager.CreateProjectile(transform.position, Vector2.zero, randCol);
        }
    }
}
