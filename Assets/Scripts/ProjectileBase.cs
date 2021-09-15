using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    
    /*
     * Things to add:
     * Projectile movement
     * Movement speed
     * Visuals trail renderer
     */
    
    // note: I'm going to make a separate script for friendly and enemy projectiles 
    
    [Header("Body stuff")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private GameObject _parent;
    [SerializeField] private ParticleSystem _hitParticles;
    [SerializeField] private AudioClip _hitSound;
    
    [Header("Stats")]
    [SerializeField] private int _damage;
    [SerializeField] private float _moveSpeed = 0.3f;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        
        _rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        Movement(_rb);
    }

    protected virtual void Movement(Rigidbody rb)
    {
        Vector3 moveOffset = transform.forward * _moveSpeed;
        rb.MovePosition(rb.position + moveOffset);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            print("hit self");
            //Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
        else if (other.gameObject.CompareTag(_parent.tag))
        {
            //Physics.IgnoreCollision(_parent.GetComponent<Collider>(), _rb.GetComponent<Collider>());
            print("hit parent: " + other.gameObject.name);
            //Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
        else
        {
            print("HIT OTHER: "  + other.gameObject.name);
            IDamageable obj = other.gameObject.GetComponent<IDamageable>();
            obj?.Damage(_damage); // only apply damage if obj isn't null
            Feedback(); // spawn particles and sfx
            gameObject.SetActive(false);
        }
    }

    private void Feedback()
    {
        // particles
        if (_hitParticles != null)
        {
            _hitParticles = Instantiate(_hitParticles, transform.position, Quaternion.identity);
            _hitParticles.gameObject.SetActive(true);
            //print("Hit Particles: " + _hitParticles.transform.rotation.eulerAngles);
        }
        
        // sfx
        if (_hitSound != null)
        {
            AudioHelper.PlayClip2D(_hitSound, 1f);
        }
        
        Destroy(gameObject);
    }
}
