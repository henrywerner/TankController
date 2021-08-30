using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedIncrease : CollectableBase
{
    [SerializeField] private float _speedAmount = 0.2f;

    protected override void Collect(Player player)
    {
        TankController controller = player.GetComponent<TankController>();
        if (controller != null)
        {
            controller.MaxSpeed += _speedAmount;
        }
    }

    protected override void Movement(Rigidbody rb)
    {
        Quaternion turnOffset = Quaternion.Euler(MovementSpeed, MovementSpeed, MovementSpeed);
        rb.MoveRotation(rb.rotation * turnOffset);
    }
}
