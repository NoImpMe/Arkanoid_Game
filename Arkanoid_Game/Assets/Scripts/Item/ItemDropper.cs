public class ItemDropper
{
    private readonly float dropChance;
    private readonly ItemDefinition[] items;
    private readonly System.Random random;

    public ItemDropper(float dropChance, ItemDefinition[] items, System.Random random)
    {
        this.dropChance = dropChance;
        this.items = items;
        this.random = random;
    }

    public static ItemDropper FromConfig(ItemConfig config, System.Random random)
    {
        return new ItemDropper(config.dropChance, config.items, random);
    }

    public static bool CanDrop(bool brickCanDropItem, bool isCapsuleFalling, bool hasMultipleBalls)
    {
        return brickCanDropItem && !isCapsuleFalling && !hasMultipleBalls;
    }

    public bool TryDrop(bool brickCanDropItem, bool isCapsuleFalling, bool hasMultipleBalls, out ItemDefinition droppedItem)
    {
        droppedItem = null;
        if (!CanDrop(brickCanDropItem, isCapsuleFalling, hasMultipleBalls) || items.Length == 0)
        {
            return false;
        }

        if (random.NextDouble() >= dropChance)
        {
            return false;
        }

        droppedItem = items[PickIndex(items, random.Next(TotalWeight(items)))];
        return true;
    }

    public static int TotalWeight(ItemDefinition[] items)
    {
        int total = 0;
        foreach (ItemDefinition item in items)
        {
            total += item.dropWeight;
        }
        return total;
    }

    public static int PickIndex(ItemDefinition[] items, int roll)
    {
        int cumulativeWeight = 0;
        for (int index = 0; index < items.Length; index++)
        {
            cumulativeWeight += items[index].dropWeight;
            if (roll < cumulativeWeight)
            {
                return index;
            }
        }
        return items.Length - 1;
    }
}
