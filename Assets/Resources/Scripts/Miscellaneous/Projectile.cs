using Enemies;
using Global;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    protected float _speed;
    [SerializeField]
    protected int _damage;

    protected Vector2 _origin, _target;
    protected Transform _targetTransform;

    protected bool _hitEnemy = false;

    public virtual void InitializeProjectileWithTransform(Vector2 origin, Transform target, Color? color = null)
    {
        InitTransform(origin, target, color);
        StartAnimating();
    }

    public virtual void InitializeProjectileWithVector(Vector2 origin, Vector2 target, Color? color = null)
    {
        InitVector(origin, target, color);
        StartAnimating();
    }

    /// <summary>
    /// Initialize projectile with a transform as target (will follow the transform's position changes)
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <param name="color"></param>
    /// 
    protected virtual void InitTransform(Vector2 origin, Transform target, Color? color = null)
    {
        _origin = origin;
        _targetTransform = target;

        if (color != null)
        {
            Color wantedColor = (Color)color;
            // Set projectile sprite color
            SpriteRenderer projectileSpriteRend = GetComponentInChildren<SpriteRenderer>();
            projectileSpriteRend.color = wantedColor;

            // Set "flight" particle color if there's a particle system (why is it so complicated??)
            var sys = transform.Find("FlightParticles").GetComponent<ParticleSystem>();
            var main = sys.main;
            ParticleSystem.MinMaxGradient gradient = new()
            {
                color = new Color()
                {
                    r = wantedColor.r,
                    g = wantedColor.g,
                    b = wantedColor.b,
                    a = 0.75f
                }
            };
            main.startColor = gradient;
        }
    }

    protected virtual void StartAnimating()
    {
        StartCoroutine(AnimateProjectileToTarget());
    }

    /// <summary>
    /// Initialize projectile with a Vector2 as target (will only fly until reaches target)
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <param name="color"></param>
    protected virtual void InitVector(Vector2 origin, Vector2 target, Color? color = null)
    {
        _origin = origin;
        _target = target;

        if (color != null)
        {
            Color wantedColor = (Color)color;
            // Set projectile sprite color
            SpriteRenderer projectileSpriteRend = GetComponentInChildren<SpriteRenderer>();
            projectileSpriteRend.color = wantedColor;

            // Set "flight" particle color if there's a particle system (why is it so complicated??)
            var sys = transform.Find("FlightParticles").GetComponent<ParticleSystem>();
            var main = sys.main;
            ParticleSystem.MinMaxGradient gradient = new()
            {
                color = new Color()
                {
                    r = wantedColor.r,
                    g = wantedColor.g,
                    b = wantedColor.b,
                    a = 0.75f
                }
            };
            main.startColor = gradient;
        }
    }

    protected Collider2D _hitCollision;
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _hitCollision = collision;
            _hitEnemy = true;
        }
    }

    // Used to keep track of last known direction of projectile
    // in case the target transform is destroyed and is null
    protected Vector2 lastKnownDirection = Vector2.zero;

    /// <summary>
    /// Animates projectile towards its target, either a transform or position
    /// </summary>
    protected virtual IEnumerator AnimateProjectileToTarget()
    {
        float distance = Mathf.Infinity;
        int maxLifetimeFrames = 5000;
        int lifeFrames = 0;

        // Find "end explosion" particles
        var explosionParticles = transform.Find("ExplosionParticles").GetComponent<ParticleSystem>();

        // Stop any particles from being created during flight
        explosionParticles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Set parent to null (unparent particles from projectile, remember to destroy)
        explosionParticles.transform.parent = null;

        Vector2 direction = Vector2.zero;

        while (distance > 0.4f)
        {
            if (_targetTransform != null)
            {
                _target = _targetTransform.position;
            }
            else
            {
                _target += lastKnownDirection.normalized;
                direction = _target - _origin;
                lastKnownDirection = direction.normalized;

                // Calculate distance to the target position
                float distanceToVec2Target = Vector2.Distance(transform.position, _target);
                if (distanceToVec2Target < 0.2f || lifeFrames > maxLifetimeFrames)
                    break;
            }

            // Update distance
            distance = Vector2.Distance(transform.position, _target);

            // Get direction from point A to point B
            direction = _target - _origin;
            lastKnownDirection = direction.normalized;

            // Normalize direction to a length of 1 so it doesn't affect flight speed
            direction = direction.normalized;

            // Move projectile
            transform.Translate(_speed * Time.deltaTime * direction);

            // Also move explosion particle object to projectile position
            // (since it is now unparented)
            explosionParticles.transform.position = transform.position;

            // If hit prematurely or lifetime is over max
            // TODO: (_hit is not set anywhere yet)
            if (_hitEnemy || lifeFrames > maxLifetimeFrames)
                break;

            lifeFrames++;
            yield return new WaitForEndOfFrame();
        }


        // Calculate distance again to check whether we are actually near our target
        // and can damage the enemy
        distance = Vector2.Distance(transform.position, _target);
        if (distance < 0.4f && _targetTransform != null)
        {
            EnemyData data = _targetTransform.GetComponent<EnemyData>();
            GameManager.Instance.EnemyHit(_targetTransform.gameObject, _damage);
        }

        // If we don't have a target 
        else if (_targetTransform == null && _hitEnemy)
        {
            var enemyObj = _hitCollision.gameObject;
            GameManager.Instance.EnemyHit(enemyObj, _damage);
        }

        // Destroy the graphic element
        var spriteObj = transform.Find("Sprite").gameObject;
        Destroy(spriteObj);

        // Run explosion particles
        explosionParticles.Play();

        while (explosionParticles.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        // Finally destroy particle object
        Destroy(explosionParticles.gameObject);
        Destroy(gameObject);
    }
}
