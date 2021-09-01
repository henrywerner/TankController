using System.Collections;
using UnityEngine;

public class Slower : Enemy
{
    [SerializeField] private AudioClip _unslowedSound;
    [SerializeField] private float _speedAmount = 2f;
    
    protected override void PlayerImpact(Player player)
    {
        TankController controller = player.GetComponent<TankController>();
        if (controller != null)
        {
            controller.MaxSpeed /= _speedAmount;
            StartCoroutine(SlowForDuration(controller));
        }
    }

    IEnumerator SlowForDuration(TankController controller)
    {
        yield return new WaitForSecondsRealtime(2f);
        
        // restore speed
        controller.MaxSpeed *= _speedAmount;
        AudioHelper.PlayClip2D(_unslowedSound, 1f);
    }
}