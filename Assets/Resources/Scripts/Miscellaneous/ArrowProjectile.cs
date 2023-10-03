using Global;
using System.Collections;
using UnityEngine;

public class ArrowProjectile : Projectile
{
    private Quaternion _rotation;
    public override void InitializeProjectileWithTransform(Vector2 origin, Transform target, Color? color = null)
    {
        base.InitTransform(origin, target);
        StartAnimating();
    }

    public override void InitializeProjectileWithVector(Vector2 origin, Vector2 target, Color? color = null)
    {
        base.InitVector(origin, target);
        StartAnimating();
    }

    protected override void StartAnimating()
    {
        StartCoroutine(AnimateProjectileToTarget());
    }

    protected override IEnumerator AnimateProjectileToTarget()
    {
        float distance = Mathf.Infinity;
        int maxLifetimeFrames = 5000;
        int lifeFrames = 0;

        Vector2 direction = Vector2.zero;

        Transform spriteContainer = transform.Find("SpriteContainer");

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

            // Set sprite container's Z rotation to the direction's angle (thanks github copilot) 
            _rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            spriteContainer.rotation = _rotation;

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
            //EnemyData data = _targetTransform.GetComponent<EnemyData>();
            GameManager.Instance.EnemyHit(_targetTransform.gameObject, _damage);
        }

        // If we don't have a target 
        else if (_targetTransform == null && _hitEnemy)
        {
            var enemyObj = _hitCollision.gameObject;
            GameManager.Instance.EnemyHit(enemyObj, _damage);
        }

        // Destroy the gameobject
        Destroy(gameObject);
    }
}
