using NUnit.Framework;
using UnityEngine;

public class CapsuleMotionTests
{
    private static readonly Vector2 CapsuleSize = new Vector2(1f, 0.5f);
    private static readonly Rect PaddleArea = WorldRect.FromCenter(new Vector2(0f, -8.35f), new Vector2(2f, 0.5f));

    [Test]
    public void Fall_MovesStraightDownBySpeedTimesDeltaTime()
    {
        Vector2 next = CapsuleMotion.Fall(new Vector2(2f, 5f), 3f, 0.1f);

        Assert.AreEqual(2f, next.x, 1e-5f);
        Assert.AreEqual(4.7f, next.y, 1e-5f);
    }

    [Test]
    public void WorldRect_FromCenter_UsesHalfSizeOnEachSide()
    {
        Rect area = WorldRect.FromCenter(new Vector2(0f, -8.35f), new Vector2(2f, 0.5f));

        Assert.AreEqual(-1f, area.xMin, 1e-5f);
        Assert.AreEqual(1f, area.xMax, 1e-5f);
        Assert.AreEqual(-8.6f, area.yMin, 1e-5f);
        Assert.AreEqual(-8.1f, area.yMax, 1e-5f);
    }

    [Test]
    public void IsCaught_CapsuleReachesPaddleTop_Caught()
    {
        Rect before = WorldRect.FromCenter(new Vector2(0.5f, -7.8f), CapsuleSize);
        Rect after = WorldRect.FromCenter(new Vector2(0.5f, -7.9f), CapsuleSize);

        Assert.IsTrue(CapsuleMotion.IsCaught(before, after, PaddleArea));
    }

    [Test]
    public void IsCaught_CapsuleStillAbovePaddle_NotCaught()
    {
        Rect before = WorldRect.FromCenter(new Vector2(0f, -7.5f), CapsuleSize);
        Rect after = WorldRect.FromCenter(new Vector2(0f, -7.6f), CapsuleSize);

        Assert.IsFalse(CapsuleMotion.IsCaught(before, after, PaddleArea));
    }

    [Test]
    public void IsCaught_CapsuleBesidePaddle_NotCaught()
    {
        Rect before = WorldRect.FromCenter(new Vector2(3f, -8.2f), CapsuleSize);
        Rect after = WorldRect.FromCenter(new Vector2(3f, -8.4f), CapsuleSize);

        Assert.IsFalse(CapsuleMotion.IsCaught(before, after, PaddleArea));
    }

    [Test]
    public void IsCaught_LongFrameJumpsPastPaddle_StillCaughtBySweep()
    {
        Vector2 start = new Vector2(0f, -7.5f);
        Vector2 end = CapsuleMotion.Fall(start, 3f, 0.5f);
        Rect before = WorldRect.FromCenter(start, CapsuleSize);
        Rect after = WorldRect.FromCenter(end, CapsuleSize);

        Assert.IsFalse(after.Overlaps(PaddleArea));
        Assert.IsTrue(CapsuleMotion.IsCaught(before, after, PaddleArea));
    }

    [Test]
    public void HasLeftField_OnlyWhenWholeCapsuleIsBelowDeadZone()
    {
        Assert.IsFalse(CapsuleMotion.HasLeftField(WorldRect.FromCenter(new Vector2(0f, -9.5f), CapsuleSize), -9.6f));
        Assert.IsFalse(CapsuleMotion.HasLeftField(WorldRect.FromCenter(new Vector2(0f, -9.8f), CapsuleSize), -9.6f));
        Assert.IsTrue(CapsuleMotion.HasLeftField(WorldRect.FromCenter(new Vector2(0f, -9.9f), CapsuleSize), -9.6f));
    }
}
