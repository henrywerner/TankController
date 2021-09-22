
using UnityEngine;

public class HealthIncrease : CollectableBase
{
    [SerializeField] int _healthAdded = 1;

    protected override void Collect(Player player)
    {
        player.IncreaseHealth(_healthAdded);
    }
}
