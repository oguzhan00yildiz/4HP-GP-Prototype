using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDebugger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 50; i++)
        {
            Vector2 target = Random.insideUnitCircle * 1000;
            ProjectileManager.CreateProjectile(Vector2.zero, target, Color.green);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
