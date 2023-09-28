using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    const string PREFAB_PATH = "Prefabs/Projectiles/BasicProjectile";
    private static GameObject _prefabCache;

    /// <summary>
    /// Create a projectile with a transform as target (will follow the transform's position changes)
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <param name="color"></param>
    public static void CreateProjectile(Vector2 origin, Transform target, Color color)
    {
        GameObject projectileObj = InstantiateProjectile();
        projectileObj.transform.position = origin;

        Projectile proj = projectileObj.GetComponent<Projectile>();

        if (proj == null)
        {
            Debug.LogError($"Projectile {projectileObj.name} doesn't have a Projectile component!", projectileObj);
            return;
        }

        proj.InitializeProjectileWithTransform(origin, target, color);
    }

    // Generate method that works the same as the above CreateProjectile but accepts a Vector3 target instead of a Transform
    public static void CreateProjectile(Vector2 origin, Vector2 target, Color color)
    {
        GameObject projectileObj = InstantiateProjectile();
        projectileObj.transform.position = origin;

        Projectile proj = projectileObj.GetComponent<Projectile>();

        if (proj == null)
        {
            Debug.LogError($"Projectile {projectileObj.name} doesn't have a Projectile component!", projectileObj);
            return;
        }

        proj.InitializeProjectileWithVector(origin, target, color);
    }

    static GameObject InstantiateProjectile()
    {
        if(_prefabCache == null )
        {
            _prefabCache = Resources.Load(PREFAB_PATH) as GameObject;
        }

        return Instantiate(_prefabCache);
    }
}
