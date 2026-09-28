using NUnit.Framework;

public class BrickDurabilityTests
{
    [Test]
    public void NormalBrick_DestroyedOnFirstHit()
    {
        BrickDurability durability = new BrickDurability(1);

        Assert.IsTrue(durability.TakeHit());
        Assert.IsTrue(durability.IsDestroyed);
    }

    [Test]
    public void SilverBrick_SurvivesFirstHitAndDestroyedOnSecond()
    {
        BrickDurability durability = new BrickDurability(2);

        Assert.IsFalse(durability.TakeHit());
        Assert.AreEqual(1, durability.RemainingHits);
        Assert.IsTrue(durability.TakeHit());
    }

    [Test]
    public void IndestructibleBrick_NeverDestroyedAfterManyHits()
    {
        BrickDurability durability = BrickDurability.Indestructible();

        for (int hit = 0; hit < 100; hit++)
        {
            Assert.IsFalse(durability.TakeHit());
        }
        Assert.IsTrue(durability.IsIndestructible);
        Assert.IsFalse(durability.IsDestroyed);
    }

    [Test]
    public void NormalBrick_IsNotIndestructible()
    {
        Assert.IsFalse(new BrickDurability(2).IsIndestructible);
    }

    [Test]
    public void ExtraHitAfterDestroyed_StaysAtZero()
    {
        BrickDurability durability = new BrickDurability(1);
        durability.TakeHit();

        Assert.IsTrue(durability.TakeHit());
        Assert.AreEqual(0, durability.RemainingHits);
    }
}
