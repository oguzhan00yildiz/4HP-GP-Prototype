using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _stoppingDistance = 2.0f;
    [SerializeField] private Rigidbody2D _rigidbody;

    private Transform _target;
    private Vector3 _targetDirection;

    private void Start()
    {
        _target = FindObjectOfType<Player>().transform;
        _rigidbody = GetComponent<Rigidbody2D>();
        if(_rigidbody == null)
        {
            _rigidbody = gameObject.AddComponent<Rigidbody2D>();
            _rigidbody.gravityScale = 0.0f;
            _rigidbody.freezeRotation = true;
        }
    }

    private void Update()
    {
        if (_target != null) 
        {
            // Not normalized since we'll use magnitude to gauge distance
            Vector3 direction = (_target.position - transform.position);
            _targetDirection = direction;

            // Move in fixed update
        }
    }

    private void FixedUpdate()
    {
        if (_targetDirection.magnitude > _stoppingDistance)
        {
            Vector3 moveAmount = transform.position + _targetDirection.normalized * _speed * Time.fixedDeltaTime;
            _rigidbody.MovePosition(moveAmount);
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