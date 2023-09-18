using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile
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

    public Projectile(Color color, Vector2 origin, Vector2 target, bool fireOnCreation)
    {
        _obj = ProjectileManager.InstantiateProjectile();
        _obj.transform.position = origin;
        _origin = origin;
        _target = target;

        Particles = _obj.GetComponentInChildren<ParticleSystem>();

        // TODO: Set color

        if(!fireOnCreation)
        { return;  }

        Fire();
    }

    public void Fire()
    {
        AnimateProjectile();
    }

    async void AnimateProjectile()
    {
        float distance = Vector2.Distance(_origin, _target);

        while(distance > 0.01f)
        {

        }
    }
}
public class ProjectileManager
{
    const string PREFAB_PATH = "Prefabs/Projectiles/BasicProjectile";
    private static GameObject _prefabCache;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static GameObject InstantiateProjectile()
    {
        if(_prefabCache == null )
        {
            _prefabCache = Resources.Load(PREFAB_PATH) as GameObject;
        }

        return Object.Instantiate(_prefabCache);
    }

    IEnumerator HandleProjectile(Projectile projectile)
    {
        yield break;
    }
}
