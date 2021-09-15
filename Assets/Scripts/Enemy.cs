using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private ParticleSystem _inpactParticles;
    [SerializeField] private AudioClip _impactSound;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            PlayerImpact(player);
            ImpactFeedback();
        }
    }

    protected virtual void PlayerImpact(Player player)
    {
        player.Damage(_damageAmount);
    }

    private void ImpactFeedback()
    {
        // particles
        if (_inpactParticles != null)
        {
            _inpactParticles = Instantiate(_inpactParticles, transform.position, Quaternion.identity);
        }
        // audio
        if (_impactSound != null)
        {
            AudioHelper.PlayClip2D(_impactSound, 1f);
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    // TODO: Make virtual
    public void Move()
    {
        
    }
}
