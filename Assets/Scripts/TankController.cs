using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class TankController : MonoBehaviour
{
    [SerializeField] float _turnSpeed = 2f;
    [SerializeField] float _maxSpeed = 0.25f;
    [SerializeField] private Image _xhair;

    Rigidbody _rb = null;
    [SerializeField] GameObject _gun;

    [SerializeField] private GameObject _bullet;
    [SerializeField] private float _fireRate;
    private bool _gunReady = true;

    private Vector3 _bodyRotation;

    [Header("VFX / SFX")]
    [SerializeField] private AudioClip _gunshotSound;
    [SerializeField] private ParticleSystem _gunshotParticles;

    public float MaxSpeed
    {
        get => _maxSpeed;
        set => _maxSpeed = value;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (_xhair == null)
            print("crosshair not found");

        _bodyRotation = Vector3.zero;
    }

    private void Update()
    {
        _xhair.gameObject.transform.position = Input.mousePosition; // move the crosshair the current mouse position

        AimLoop();
    }

    private void FixedUpdate()
    {
        MoveTank();
        Fire();
    }

    public void MoveTank()
    {
        Vector3 inputRaw = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        inputRaw.Normalize(); // clamp all inputs to 1 so that way you can't diagonal strafe

        if (inputRaw != Vector3.zero)
            _bodyRotation = Quaternion.LookRotation(inputRaw).eulerAngles;

        _rb.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.Euler(_bodyRotation.x, Mathf.Round(_bodyRotation.y / 45) * 45, _bodyRotation.z),
            0.5f); // I took this from online

        _rb.MovePosition(_rb.position + new Vector3(inputRaw.x * _maxSpeed, 0, inputRaw.z * _maxSpeed));
    }

    void AimLoop()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.up, Vector3.zero);

        // Use raycast to convert mouse position into world position
        if (plane.Raycast(ray, out var distance))
        {
            Vector3 reticle = ray.GetPoint(distance);
            Vector3 direction = reticle - transform.position;
            float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            _gun.transform.rotation = Quaternion.Euler(0, rotation, 0); // rotate gun towards mouse cursor
        }
    }

    void Fire()
    {
        if (Input.GetButton("Fire1") && _gunReady)
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        _gunReady = false;

        // fire gun
        GameObject bullet = Instantiate(_bullet, _gun.transform.position, _gun.transform.rotation);
        Physics.IgnoreCollision(bullet.transform.GetComponent<Collider>(), GetComponent<Collider>());
        
        // play noise
        AudioHelper.PlayClip2D(_gunshotSound, 0.8f);
        
        // vfx
        if (_gunshotParticles != null)
        {
            // ParticleSystem gunshotVFX = Instantiate(_gunshotParticles, transform.position, Quaternion.identity);
            // gunshotVFX.gameObject.transform.rotation = _gun.transform.rotation;
            // gunshotVFX.gameObject.SetActive(true);
            
            _gunshotParticles.Play();
        }

        // wait for fire rate duration 
        yield return new WaitForSecondsRealtime(_fireRate);

        _gunReady = true;
    }
}