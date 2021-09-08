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
    
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _moveSpeed = 0.3f;
    public Quaternion rotation; // TODO: have a better way to set the rotation on create
    
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
        // Player player = other.gameObject.GetComponent<Player>();
        // if (player != null)
        // {
        //     Collect(player);
        //     Feedback(); // spawn particles and sfx
        //     gameObject.SetActive(false);
        // }
    }
}
