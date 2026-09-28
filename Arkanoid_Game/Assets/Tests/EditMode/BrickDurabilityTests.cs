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
    public void ExtraHitAfterDestroyed_StaysAtZero()
    {
        BrickDurability durability = new BrickDurability(1);
        durability.TakeHit();

        Assert.IsTrue(durability.TakeHit());
        Assert.AreEqual(0, durability.RemainingHits);
    }
}
