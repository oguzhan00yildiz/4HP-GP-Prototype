using Global;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private static GameObject _prefabCache;
    private static GameObject _arrowCache;

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

    public static void CreateArrow(Vector2 origin, Transform target)
    {
        GameObject projectileObj = InstantiateArrow();
        projectileObj.transform.position = origin;

        ArrowProjectile proj = projectileObj.GetComponent<ArrowProjectile>();

        if (proj == null && GameManager.DebugMode)
        {
            Debug.LogError($"Projectile {projectileObj.name} doesn't have an ArrowProjectile component!", projectileObj);
            return;
        }

        proj.InitializeProjectileWithTransform(origin, target);
    }

    public static void CreateArrow(Vector2 origin, Vector2 target)
    {
        GameObject projectileObj = InstantiateArrow();
        projectileObj.transform.position = new Vector3(origin.x, origin.y, 0);

        ArrowProjectile proj = projectileObj.GetComponent<ArrowProjectile>();

        if (proj == null && GameManager.DebugMode)
        {
            Debug.LogError($"Projectile {projectileObj.name} doesn't have an ArrowProjectile component!", projectileObj);
            return;
        }

        proj.InitializeProjectileWithVector(origin, target);
    }

    // Generate method that works the same as the above CreateProjectile but accepts a Vector3 target instead of a Transform
    public static void CreateProjectile(Vector2 origin, Vector2 target, Color color)
    {
        GameObject projectileObj = InstantiateProjectile();
        projectileObj.transform.position = origin;

        Projectile proj = projectileObj.GetComponent<Projectile>();

        if (proj == null && GameManager.DebugMode)
        {
            Debug.LogError($"Projectile {projectileObj.name} doesn't have a Projectile component!", projectileObj);
            return;
        }

        proj.InitializeProjectileWithVector(origin, target, color);
    }

    static GameObject InstantiateProjectile()
    {
        if (_prefabCache == null)
        {
            _prefabCache = Resources.Load("Prefabs/Projectiles/BasicProjectile") as GameObject;
        }

        return Instantiate(_prefabCache);
    }

    static GameObject InstantiateArrow()
    {
        if (_arrowCache == null)
        {
            _arrowCache = Resources.Load("Prefabs/Projectiles/BasicArrow") as GameObject;
        }

        return Instantiate(_arrowCache);
    }
}
