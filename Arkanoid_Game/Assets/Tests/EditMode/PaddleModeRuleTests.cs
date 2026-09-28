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
