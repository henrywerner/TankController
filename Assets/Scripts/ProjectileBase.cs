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
    public Quaternion rotation; // TODO: have a better way to set the rotation on create
    
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
    
    private void OnCollisionEnter(Collision other)
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
        else if (other.gameObject.name == "Boss")
        {
            print("hit boss: " + other.gameObject.name);
            Boss b = other.gameObject.GetComponent<Boss>();
            b.DecreaseHealth(1);
            Feedback(); // spawn particles and sfx
            gameObject.SetActive(false);
        }
        else
        {
            print("HIT OTHER: "  + other.gameObject.name);
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
        }
        
        // sfx
        if (_hitSound != null)
        {
            AudioHelper.PlayClip2D(_hitSound, 1f);
        }
        
        Destroy(gameObject);
    }
}
