using NUnit.Framework;
using UnityEngine;

public class BallBounceTests
{
    private const float Tolerance = 0.001f;
    private const float VerticalSpeed = 6f;
    private const float MaxBounceAngle = 60f;

    [Test]
    public void VelocityFromAngle_ThirtyDegrees_KeepsVerticalSpeedAndUsesTangent()
    {
        Vector2 velocity = BallBounce.VelocityFromAngle(VerticalSpeed, 30f);

        Assert.AreEqual(VerticalSpeed * Mathf.Tan(30f * Mathf.Deg2Rad), velocity.x, Tolerance);
        Assert.AreEqual(VerticalSpeed, velocity.y, Tolerance);
    }

    [Test]
    public void VelocityFromAngle_SixtyDegrees_ActualSpeedIsVerticalOverCosine()
    {
        Vector2 velocity = BallBounce.VelocityFromAngle(VerticalSpeed, 60f);

        Assert.AreEqual(12f, velocity.magnitude, Tolerance);
    }

    [Test]
    public void ReflectByNormalAxis_SideNormal_FlipsOnlyVx()
    {
        Vector2 result = BallBounce.ReflectByNormalAxis(new Vector2(3f, VerticalSpeed), Vector2.left);

        Assert.AreEqual(-3f, result.x, Tolerance);
        Assert.AreEqual(VerticalSpeed, result.y, Tolerance);
    }

    [Test]
    public void ReflectByNormalAxis_BottomNormal_FlipsOnlyVy()
    {
        Vector2 result = BallBounce.ReflectByNormalAxis(new Vector2(3f, VerticalSpeed), Vector2.down);

        Assert.AreEqual(3f, result.x, Tolerance);
        Assert.AreEqual(-VerticalSpeed, result.y, Tolerance);
    }

    [Test]
    public void ReflectByNormalAxis_ExactCornerNormal_FlipsBothAndKeepsVerticalSpeed()
    {
        Vector2 cornerNormal = new Vector2(-1f, -1f).normalized;

        Vector2 result = BallBounce.ReflectByNormalAxis(new Vector2(3f, VerticalSpeed), cornerNormal);

        Assert.AreEqual(-3f, result.x, Tolerance);
        Assert.AreEqual(-VerticalSpeed, result.y, Tolerance);
    }

    [Test]
    public void ReflectByNormalAxis_MostlyVerticalNormal_KeepsVerticalSpeedMagnitude()
    {
        Vector2 slantedNormal = new Vector2(0.3f, 0.95f).normalized;

        Vector2 result = BallBounce.ReflectByNormalAxis(new Vector2(-2f, -VerticalSpeed), slantedNormal);

        Assert.AreEqual(-2f, result.x, Tolerance);
        Assert.AreEqual(VerticalSpeed, result.y, Tolerance);
    }

    [Test]
    public void PaddleHitOffset_CenterEdgesAndBeyond_AreClampedToUnitRange()
    {
        Assert.AreEqual(0f, BallBounce.PaddleHitOffset(3f, 3f, 2f), Tolerance);
        Assert.AreEqual(1f, BallBounce.PaddleHitOffset(4f, 3f, 2f), Tolerance);
        Assert.AreEqual(-1f, BallBounce.PaddleHitOffset(2f, 3f, 2f), Tolerance);
        Assert.AreEqual(1f, BallBounce.PaddleHitOffset(5f, 3f, 2f), Tolerance);
        Assert.AreEqual(0.5f, BallBounce.PaddleHitOffset(3.5f, 3f, 2f), Tolerance);
    }

    [TestCase(-1f, -60f)]
    [TestCase(0f, 0f)]
    [TestCase(1f, 60f)]
    public void BounceOffPaddle_OffsetMapsToAngleAndGoesUpWithSameVerticalSpeed(float hitOffset, float expectedAngle)
    {
        Vector2 result = BallBounce.BounceOffPaddle(-VerticalSpeed, hitOffset, MaxBounceAngle);

        Assert.AreEqual(VerticalSpeed, result.y, Tolerance);
        Assert.AreEqual(VerticalSpeed * Mathf.Tan(expectedAngle * Mathf.Deg2Rad), result.x, Tolerance);
    }
}
