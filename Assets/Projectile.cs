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

        StartCoroutine(AnimateProjectile());
    }

    IEnumerator AnimateProjectile()
    {
        float distance = Mathf.Infinity;

        while(distance > 0.05f)
        {
            // Update distance
            distance = Vector2.Distance(transform.position, _target);

            // Get direction from point A to point B
            Vector2 direction = _target - _origin;

            // Normalize direction to a length of 1 so it doesn't affect flight speed
            direction = direction.normalized;

            // Move projectile
            transform.Translate(_speed * Time.deltaTime * direction);

            if (_hit)
                yield break;

            yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _hit = true;

        collision.SendMessage("TakeDamage", _damage);
    }
}
