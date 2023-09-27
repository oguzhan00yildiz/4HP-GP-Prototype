using System.Collections;
using UnityEngine;
using Enemies;
using Global;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _damage;

    private Vector2 _origin, _target;
    private Transform _targetTransform;

    private bool _hit = false;

    /// <summary>
    /// Initialize projectile with a transform as target (will follow the transform's position changes)
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <param name="color"></param>
    public void InitializeProjectileWithTransform(Vector2 origin, Transform target, Color color)
    {
        _origin = origin;
        _targetTransform = target;

        // Set projectile sprite color
        SpriteRenderer projectileSpriteRend = GetComponentInChildren<SpriteRenderer>();
        projectileSpriteRend.color = color;

        // Set "flight" particle color if there's a particle system (why is it so complicated??)
        // also does not look that good

        var sys = transform.Find("FlightParticles").GetComponent<ParticleSystem>();
        var main = sys.main;
        ParticleSystem.MinMaxGradient gradient = new()
        {
            color = new Color()
            {
                r = color.r,
                g = color.g,
                b = color.b,
                a = 0.75f
            }
        };
        main.startColor = gradient;

        StartCoroutine(AnimateProjectileTowardsTransform());
    }

    // Used to keep track of last known direction of projectile
    // in case the target transform is destroyed and is null
    private Vector2 lastKnownDirection = Vector2.zero;
    /// <summary>
    /// Animates projectile towards a transform's position
    /// </summary>
    IEnumerator AnimateProjectileTowardsTransform()
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
            if (_hit || lifeFrames > maxLifetimeFrames)
                break;

            lifeFrames++;
            yield return new WaitForEndOfFrame();
        }

        // Just destroy the projectile object for now
        Destroy(gameObject);
        distance = Vector2.Distance(transform.position, _target);
        if (distance < 0.4f && _targetTransform != null)
        {
            EnemyData data = _targetTransform.GetComponent<EnemyData>();
            GameManager.Instance.EnemyHit(_targetTransform.gameObject, _damage);
        }

        // Run explosion particles
        explosionParticles.Play();

        // Wait while system is done playing
        while (explosionParticles.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        // Finally destroy particle object
        Destroy(explosionParticles.gameObject);
    }
}
