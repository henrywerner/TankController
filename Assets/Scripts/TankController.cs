using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankController : MonoBehaviour
{
    [SerializeField] float _turnSpeed = 2f;
    [SerializeField] float _maxSpeed = 0.25f;
    [SerializeField] private Image _xhair;

    Rigidbody _rb = null;
    [SerializeField] GameObject _gun;


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

    private void FixedUpdate()
    {
        MoveTank();
        TurnTank();
        AimLoop();
    }

    public void MoveTank()
    {
        // calculate the move amount
        float moveAmountThisFrame = Input.GetAxis("Vertical") * _maxSpeed;
        // create a vector from amount and direction
        Vector3 moveOffset = transform.forward * moveAmountThisFrame;
        // apply vector to the rigidbody
        _rb.MovePosition(_rb.position + moveOffset);
        // technically adjusting vector is more accurate! (but more complex)
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
        //TODO: move crosshair motion to higher in gameloop
        _xhair.gameObject.transform.position = Input.mousePosition; // move the crosshair the current mouse position

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
}