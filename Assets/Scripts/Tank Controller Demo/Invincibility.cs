using UnityEngine;


public class Invincibility : PowerUpBase
{
    protected override void PowerUp(Player player)
    {
        // make invincible
        player.IsInvincible = true;

        // change color
        player.SetColor(Color.cyan);
        player.EnableLight(true);
    }

    protected override void PowerDown(Player player)
    {
        // make un-invincible
        player.IsInvincible = false;

        // revert color
        player.ResetColor();
        player.EnableLight(false);
    }
}