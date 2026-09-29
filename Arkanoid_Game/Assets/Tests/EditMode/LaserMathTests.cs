using NUnit.Framework;
using UnityEngine;

public class LaserMathTests
{
    private const float Tolerance = 0.0001f;

    [Test]
    public void Muzzles_NormalPaddle_AreTwoTenthsInsideEachEndOnTop()
    {
        Vector2 left = LaserMath.LeftMuzzle(0f, 2f, -8.1f, 0.2f, 0.5f);
        Vector2 right = LaserMath.RightMuzzle(0f, 2f, -8.1f, 0.2f, 0.5f);

        Assert.AreEqual(-0.8f, left.x, Tolerance);
        Assert.AreEqual(0.8f, right.x, Tolerance);
        Assert.AreEqual(-7.85f, left.y, Tolerance);
        Assert.AreEqual(-7.85f, right.y, Tolerance);
    }

    [Test]
    public void Muzzles_FollowPaddleCenterAndWidth()
    {
        Assert.AreEqual(2.2f, LaserMath.LeftMuzzle(3f, 2f, -8.1f, 0.2f, 0.5f).x, Tolerance);
        Assert.AreEqual(4.3f, LaserMath.RightMuzzle(3f, 3f, -8.1f, 0.2f, 0.5f).x, Tolerance);
    }

    [Test]
    public void IsAutoFireMode_OnlyInLaserMode()
    {
        Assert.IsTrue(LaserMath.IsAutoFireMode(PaddleMode.Laser));
        Assert.IsFalse(LaserMath.IsAutoFireMode(PaddleMode.None));
        Assert.IsFalse(LaserMath.IsAutoFireMode(PaddleMode.Enlarge));
        Assert.IsFalse(LaserMath.IsAutoFireMode(PaddleMode.Catch));
    }

    [Test]
    public void CanFire_OnlyWhenBothShotsAreGone()
    {
        Assert.IsTrue(LaserMath.CanFire(false, false, 1f, 1f));
        Assert.IsFalse(LaserMath.CanFire(true, false, 1f, 1f));
        Assert.IsFalse(LaserMath.CanFire(false, true, 1f, 1f));
        Assert.IsFalse(LaserMath.CanFire(true, true, 1f, 1f));
    }

    [Test]
    public void CanFire_OnlyAfterFireIntervalHasPassed()
    {
        Assert.IsFalse(LaserMath.CanFire(false, false, 0f, 1f));
        Assert.IsFalse(LaserMath.CanFire(false, false, 0.99f, 1f));
        Assert.IsTrue(LaserMath.CanFire(false, false, 1f, 1f));
        Assert.IsTrue(LaserMath.CanFire(false, false, 2.5f, 1f));
    }

    [Test]
    public void DefaultConfig_LaserValuesMatchSpec()
    {
        ItemConfig config = ScriptableObject.CreateInstance<ItemConfig>();

        Assert.AreEqual(new Vector2(0.1f, 0.5f), config.laserSize);
        Assert.AreEqual(15f, config.laserSpeed, Tolerance);
        Assert.AreEqual(0.2f, config.laserMuzzleInset, Tolerance);
        Assert.AreEqual(1f, config.laserFireInterval, Tolerance);
        Object.DestroyImmediate(config);
    }
}
