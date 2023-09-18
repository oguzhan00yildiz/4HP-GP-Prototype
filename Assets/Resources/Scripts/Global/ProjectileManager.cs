using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileClass
{
    public GameObject WorldObject
    {
        get { return _obj; }
    }

    public ParticleSystem Particles;

    private GameObject _obj;
    private readonly Vector2 _origin;
    private readonly Vector2 _target;
    public enum ProjectileType
    {
        Basic
    };

    public ProjectileClass(Color color, Vector2 origin, Vector2 target, bool fireOnCreation)
    {
        //_obj = ProjectileManager.InstantiateProjectile();
        _obj.transform.position = origin;
        _origin = origin;
        _target = target;

        Particles = _obj.GetComponentInChildren<ParticleSystem>();

        // TODO: Set color

        if(!fireOnCreation)
        { return;  }
    }
}
public class ProjectileManager : MonoBehaviour
{
    const string PREFAB_PATH = "Prefabs/Projectiles/BasicProjectile";
    private static GameObject _prefabCache;

    public static void CreateProjectile(Vector2 origin, Vector2 target, Color color)
    {
        GameObject projectileObj = InstantiateProjectile();
        projectileObj.transform.position = origin;

        Projectile proj = projectileObj.GetComponent<Projectile>();

        if(proj == null)
        {
            Debug.LogError($"Projectile {projectileObj.name} doesn't have a Projectile component!", projectileObj);
            return;
        }

        proj.InitializeProjectile(origin, target, color);
    }

    static GameObject InstantiateProjectile()
    {
        if(_prefabCache == null )
        {
            _prefabCache = Resources.Load(PREFAB_PATH) as GameObject;
        }

        return Object.Instantiate(_prefabCache);
    }
}
