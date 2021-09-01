public class Treasure : CollectableBase
{
    protected override void Collect(Player player)
    {
        InventoryManager inventory = player.GetComponent<InventoryManager>();
        if (inventory != null)
        {
            //add treasure
            inventory.IncreaseTreasure();
        }
    }
}
