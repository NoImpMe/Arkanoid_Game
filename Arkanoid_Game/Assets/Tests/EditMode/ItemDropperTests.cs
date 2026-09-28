using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class ItemDropperTests
{
    private ItemConfig config;

    [SetUp]
    public void SetUp()
    {
        config = ScriptableObject.CreateInstance<ItemConfig>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(config);
    }

    [Test]
    public void DefaultConfig_MatchesSpecCommonRulesAndWeights()
    {
        Assert.AreEqual(0.2f, config.dropChance, 1e-6f);
        Assert.AreEqual(3f, config.fallSpeed, 1e-6f);
        Assert.AreEqual(new Vector2(1f, 0.5f), config.capsuleSize);
        Assert.AreEqual(5, config.maxReserveLives);

        CollectionAssert.AreEqual(
            new[] { ItemType.Lasers, ItemType.Enlarge, ItemType.Catch, ItemType.Slow, ItemType.Disruption, ItemType.Player },
            config.items.Select(item => item.type).ToArray());
        CollectionAssert.AreEqual(new[] { "L", "E", "C", "S", "D", "P" }, config.items.Select(item => item.letter).ToArray());
        CollectionAssert.AreEqual(new[] { 18, 18, 18, 18, 18, 10 }, config.items.Select(item => item.dropWeight).ToArray());
        Assert.AreEqual(100, ItemDropper.TotalWeight(config.items));
    }

    [Test]
    public void CanDrop_OnlyWhenBrickCanDropAndNoCapsuleAndSingleBall()
    {
        Assert.IsTrue(ItemDropper.CanDrop(true, false, false));
        Assert.IsFalse(ItemDropper.CanDrop(false, false, false));
        Assert.IsFalse(ItemDropper.CanDrop(true, true, false));
        Assert.IsFalse(ItemDropper.CanDrop(true, false, true));
    }

    [Test]
    public void TryDrop_BlockedConditions_NeverDropEvenWithCertainChance()
    {
        ItemDropper dropper = new ItemDropper(1f, config.items, new System.Random(1));

        for (int attempt = 0; attempt < 1000; attempt++)
        {
            Assert.IsFalse(dropper.TryDrop(false, false, false, out ItemDefinition silverOrGold));
            Assert.IsNull(silverOrGold);
            Assert.IsFalse(dropper.TryDrop(true, true, false, out _));
            Assert.IsFalse(dropper.TryDrop(true, false, true, out _));
        }
    }

    [Test]
    public void TryDrop_NormalBrick_DropsAboutTwentyPercent()
    {
        ItemDropper dropper = ItemDropper.FromConfig(config, new System.Random(1234));
        const int attempts = 100000;

        int drops = 0;
        for (int attempt = 0; attempt < attempts; attempt++)
        {
            if (dropper.TryDrop(true, false, false, out ItemDefinition item))
            {
                Assert.IsNotNull(item);
                drops++;
            }
        }

        Assert.AreEqual(0.2, (double)drops / attempts, 0.01);
    }

    [Test]
    public void PickIndex_WeightBoundariesMapToExpectedItems()
    {
        Assert.AreEqual(0, ItemDropper.PickIndex(config.items, 0));
        Assert.AreEqual(0, ItemDropper.PickIndex(config.items, 17));
        Assert.AreEqual(1, ItemDropper.PickIndex(config.items, 18));
        Assert.AreEqual(4, ItemDropper.PickIndex(config.items, 89));
        Assert.AreEqual(5, ItemDropper.PickIndex(config.items, 90));
        Assert.AreEqual(5, ItemDropper.PickIndex(config.items, 99));
    }

    [Test]
    public void TryDrop_FixedSeed_TypeRatiosFollowWeights()
    {
        ItemDropper dropper = new ItemDropper(1f, config.items, new System.Random(42));
        const int attempts = 100000;

        int[] counts = new int[config.items.Length];
        for (int attempt = 0; attempt < attempts; attempt++)
        {
            Assert.IsTrue(dropper.TryDrop(true, false, false, out ItemDefinition item));
            counts[System.Array.IndexOf(config.items, item)]++;
        }

        for (int index = 0; index < counts.Length; index++)
        {
            double expected = (double)config.items[index].dropWeight / ItemDropper.TotalWeight(config.items);
            Assert.AreEqual(expected, (double)counts[index] / attempts, 0.01, config.items[index].letter);
        }
    }

    [Test]
    public void TryDrop_SameSeed_ProducesSameSequence()
    {
        ItemDropper first = ItemDropper.FromConfig(config, new System.Random(7));
        ItemDropper second = ItemDropper.FromConfig(config, new System.Random(7));

        for (int attempt = 0; attempt < 500; attempt++)
        {
            bool firstDropped = first.TryDrop(true, false, false, out ItemDefinition firstItem);
            bool secondDropped = second.TryDrop(true, false, false, out ItemDefinition secondItem);
            Assert.AreEqual(firstDropped, secondDropped);
            Assert.AreSame(firstItem, secondItem);
        }
    }
}
