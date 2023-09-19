using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _damage;

    private Vector2 _origin, _target;

    private bool _hit = false;
    
    public void InitializeProjectile(Vector2 origin, Vector2 target, Color color)
    {
        _origin = origin;
        _target = target;

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

        StartCoroutine(AnimateProjectile());
    }

    IEnumerator AnimateProjectile()
    {
        float distance = Mathf.Infinity;
        int maxLifetimeFrames = 15_000;
        int lifeFrames = 0;

        // Find "end explosion" particles
        var explosionParticles = transform.Find("ExplosionParticles").GetComponent<ParticleSystem>();
        // Stop any particles from being created during flight
        explosionParticles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        // Set parent to null (unparent particles from projectile, remember to destroy)
        explosionParticles.transform.parent = null;

        while (distance > 0.05f)
        {
            // Update distance
            distance = Vector2.Distance(transform.position, _target);

            // Get direction from point A to point B
            Vector2 direction = _target - _origin;

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

        // Run explosion particles
        explosionParticles.Play();

        // Wait while system is done playing
        while(explosionParticles.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        // Finally destroy particle object
        Destroy(explosionParticles.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _hit = true;

        collision.SendMessage("TakeDamage", _damage);
    }
}
