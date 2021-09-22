using System;
using UnityEngine;

public class Missile : ProjectileBase
{
    [SerializeField] private float _startSpeed = 0.3f;
    [SerializeField] private float _acceleration = 1.1f;
    private float _speed;

    private void Start()
    {
        _speed = _startSpeed;
    }

    protected override void Movement(Rigidbody rb)
    {
        Vector3 moveOffset = transform.forward * _speed;
        _speed *= _acceleration;
        rb.MovePosition(rb.position + moveOffset);
    }
}