public class Killer : Enemy
{
    protected override void PlayerImpact(Player player)
    {
        player.Kill();
    }
}
