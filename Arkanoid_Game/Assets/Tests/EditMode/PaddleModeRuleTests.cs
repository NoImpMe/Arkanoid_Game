using NUnit.Framework;
using UnityEngine;

public class PaddleModeRuleTests
{
    [Test]
    public void AfterPickup_PaddleModeItems_ReplacePreviousMode()
    {
        Assert.AreEqual(PaddleMode.Enlarge, PaddleModeRule.AfterPickup(PaddleMode.Laser, ItemType.Enlarge));
        Assert.AreEqual(PaddleMode.Laser, PaddleModeRule.AfterPickup(PaddleMode.Enlarge, ItemType.Lasers));
        Assert.AreEqual(PaddleMode.Catch, PaddleModeRule.AfterPickup(PaddleMode.Enlarge, ItemType.Catch));
        Assert.AreEqual(PaddleMode.Enlarge, PaddleModeRule.AfterPickup(PaddleMode.None, ItemType.Enlarge));
    }

    [TestCase(ItemType.Slow)]
    [TestCase(ItemType.Player)]
    [TestCase(ItemType.Disruption)]
    public void AfterPickup_NonPaddleItems_KeepCurrentMode(ItemType item)
    {
        foreach (PaddleMode mode in new[] { PaddleMode.None, PaddleMode.Laser, PaddleMode.Enlarge, PaddleMode.Catch })
        {
            Assert.AreEqual(mode, PaddleModeRule.AfterPickup(mode, item), mode.ToString());
        }
    }

    [Test]
    public void AfterBallCaught_CatchEndsAfterOneCatch()
    {
        Assert.AreEqual(PaddleMode.None, PaddleModeRule.AfterBallCaught(PaddleMode.Catch));
    }

    [Test]
    public void AfterBallCaught_OtherModes_AreKept()
    {
        Assert.AreEqual(PaddleMode.None, PaddleModeRule.AfterBallCaught(PaddleMode.None));
        Assert.AreEqual(PaddleMode.Laser, PaddleModeRule.AfterBallCaught(PaddleMode.Laser));
        Assert.AreEqual(PaddleMode.Enlarge, PaddleModeRule.AfterBallCaught(PaddleMode.Enlarge));
    }

    [Test]
    public void HasDuration_OnlyLaserAndEnlarge()
    {
        Assert.IsTrue(PaddleModeRule.HasDuration(PaddleMode.Laser));
        Assert.IsTrue(PaddleModeRule.HasDuration(PaddleMode.Enlarge));
        Assert.IsFalse(PaddleModeRule.HasDuration(PaddleMode.Catch));
        Assert.IsFalse(PaddleModeRule.HasDuration(PaddleMode.None));
    }

    [Test]
    public void DurationFor_UsesEachModeDuration()
    {
        Assert.AreEqual(5f, PaddleModeRule.DurationFor(PaddleMode.Laser, 5f, 7f), 1e-5f);
        Assert.AreEqual(7f, PaddleModeRule.DurationFor(PaddleMode.Enlarge, 5f, 7f), 1e-5f);
        Assert.AreEqual(0f, PaddleModeRule.DurationFor(PaddleMode.Catch, 5f, 7f), 1e-5f);
        Assert.AreEqual(0f, PaddleModeRule.DurationFor(PaddleMode.None, 5f, 7f), 1e-5f);
    }

    [Test]
    public void WidthFor_DefaultConfigs_EnlargeIsThreeOthersAreTwo()
    {
        PaddleConfig paddleConfig = ScriptableObject.CreateInstance<PaddleConfig>();
        ItemConfig itemConfig = ScriptableObject.CreateInstance<ItemConfig>();

        Assert.AreEqual(3f, PaddleModeRule.WidthFor(PaddleMode.Enlarge, paddleConfig.width, itemConfig.enlargeWidthMultiplier), 1e-5f);
        Assert.AreEqual(2f, PaddleModeRule.WidthFor(PaddleMode.None, paddleConfig.width, itemConfig.enlargeWidthMultiplier), 1e-5f);
        Assert.AreEqual(2f, PaddleModeRule.WidthFor(PaddleMode.Laser, paddleConfig.width, itemConfig.enlargeWidthMultiplier), 1e-5f);
        Assert.AreEqual(2f, PaddleModeRule.WidthFor(PaddleMode.Catch, paddleConfig.width, itemConfig.enlargeWidthMultiplier), 1e-5f);

        Object.DestroyImmediate(paddleConfig);
        Object.DestroyImmediate(itemConfig);
    }
}
