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
    }

    private void Update()
    {
        _xhair.gameObject.transform.position = Input.mousePosition; // move the crosshair the current mouse position
    }

    private void FixedUpdate()
    {
        MoveTank();
        //TurnTank();
        AimLoop();
        Fire();
    }

    public void MoveTank()
    {
        // // calculate the move amount
        // float moveAmountThisFrame = Input.GetAxis("Vertical") * _maxSpeed;
        // // create a vector from amount and direction
        // Vector3 moveOffset = transform.forward * moveAmountThisFrame;
        // // apply vector to the rigidbody
        // _rb.MovePosition(_rb.position + moveOffset);
        // // technically adjusting vector is more accurate! (but more complex)

        //Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.rotation = Quaternion.LookRotation(direction);

        // float moveAmountThisFrame = Input.GetAxis("Vertical") * _maxSpeed;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (direction != Vector3.zero)
            _rb.rotation = Quaternion.LookRotation(direction); // rotate to face the input direction

        direction = Vector3.ClampMagnitude(direction, 1f); // don't move faster on diagonal strafe

        _rb.position = _rb.position + new Vector3(direction.x * _maxSpeed * Time.deltaTime, 0, direction.z * _maxSpeed * Time.deltaTime);
    }

    public void TurnTank()
    {
        // calculate the turn amount
        float turnAmountThisFrame = Input.GetAxis("Horizontal") * _turnSpeed;
        // create a Quaternion from amount and direction (x,y,z)
        Quaternion turnOffset = Quaternion.Euler(0, turnAmountThisFrame, 0);
        // apply quaternion to the rigidbody
        _rb.MoveRotation(_rb.rotation * turnOffset);
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
        
        yield return new WaitForSecondsRealtime(_fireRate);
        
        // fire gun

        Instantiate(_bullet, _rb.position, _gun.transform.rotation);
        print("shoot");

        _gunReady = true;
    }
}