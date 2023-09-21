using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Transform _target;

    private void Start()
    {
        _target = FindObjectOfType<Player>().transform;
    }

    private void Update()
    {
        if (_target != null) 
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            transform.Translate(direction * Time.deltaTime * _speed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            Destroy(gameObject);
        }
    }
}