using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 _moveDirection;
    public Rigidbody2D _rb { get; private set; }
    public Vector2 ExternalVelocity { get; set; }

    [SerializeField] private float _speed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _moveDirection * _speed + ExternalVelocity;
    }

    private void OnDisable()
    {
        ExternalVelocity = Vector2.zero;
    }

    private void OnMove(InputValue movementValue)
    {
        _moveDirection = movementValue.Get<Vector2>();
    }
}
