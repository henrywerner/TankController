using System;
using UnityEngine;

public abstract class CollectableBase : MonoBehaviour
{
    protected abstract void Collect(Player player);

    [SerializeField] private float _movementSpeed = 1;
    protected float MovementSpeed => _movementSpeed;
    
    [SerializeField] private ParticleSystem _collectParticles;
    [SerializeField] private AudioClip _collectSound;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Movement(_rb);
    }

    protected virtual void Movement(Rigidbody rb)
    {
        Quaternion turnOffset = Quaternion.Euler(0, _movementSpeed, 0);
        rb.MoveRotation(_rb.rotation * turnOffset);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            Collect(player);
            Feedback(); // spawn particles and sfx
            gameObject.SetActive(false);
        }
    }

    private void Feedback()
    {
        // particles
        if (_collectParticles != null)
        {
            _collectParticles = Instantiate(_collectParticles, transform.position, Quaternion.identity);
        }

        if (_collectSound != null)
        {
            AudioHelper.PlayClip2D(_collectSound, 1f);
        }
    }
}