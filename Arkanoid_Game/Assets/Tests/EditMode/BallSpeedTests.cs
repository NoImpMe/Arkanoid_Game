using NUnit.Framework;
using UnityEngine;

public class BallSpeedTests
{
    private const float Tolerance = 0.0001f;

    [Test]
    public void Increased_AddsIncrementPerBrick()
    {
        Assert.AreEqual(6.05f, BallSpeed.Increased(6f, 0.05f, 9f), Tolerance);
    }

    [Test]
    public void Increased_StopsAtMaxVerticalSpeed()
    {
        Assert.AreEqual(9f, BallSpeed.Increased(8.98f, 0.05f, 9f), Tolerance);
        Assert.AreEqual(9f, BallSpeed.Increased(9f, 0.05f, 9f), Tolerance);
    }

    [Test]
    public void Increased_AllBricksFromInitialStopsAtMax()
    {
        float verticalSpeed = 6f;
        for (int destroyed = 0; destroyed < 78; destroyed++)
        {
            verticalSpeed = BallSpeed.Increased(verticalSpeed, 0.05f, 9f);
        }

        Assert.AreEqual(9f, verticalSpeed, Tolerance);
    }

    [Test]
    public void WithVerticalSpeed_ScalesBothAxesAndKeepsAngle()
    {
        Vector2 velocity = BallBounce.VelocityFromAngle(6f, 30f);
        velocity.y = -velocity.y;

        Vector2 result = BallSpeed.WithVerticalSpeed(velocity, 9f);

        Assert.AreEqual(-9f, result.y, Tolerance);
        Assert.AreEqual(9f * Mathf.Tan(30f * Mathf.Deg2Rad), result.x, Tolerance);
    }

    [Test]
    public void WithVerticalSpeed_ZeroVelocity_StaysZero()
    {
        Assert.AreEqual(Vector2.zero, BallSpeed.WithVerticalSpeed(Vector2.zero, 9f));
    }

    [Test]
    public void Slowed_AboveSlowSpeed_DropsToSlowSpeed()
    {
        Assert.AreEqual(4f, BallSpeed.Slowed(6f, 4f), 1e-5f);
        Assert.AreEqual(4f, BallSpeed.Slowed(9f, 4f), 1e-5f);
    }

    [Test]
    public void Slowed_AtOrBelowSlowSpeed_Unchanged()
    {
        Assert.AreEqual(4f, BallSpeed.Slowed(4f, 4f), 1e-5f);
        Assert.AreEqual(3.5f, BallSpeed.Slowed(3.5f, 4f), 1e-5f);
    }

    [Test]
    public void Slowed_ThenBrickDestroyed_SpeedsUpAgain()
    {
        float slowed = BallSpeed.Slowed(7f, 4f);

        Assert.AreEqual(4.05f, BallSpeed.Increased(slowed, 0.05f, 9f), 1e-5f);
    }

    [Test]
    public void Slowed_AppliedToVelocity_KeepsAngle()
    {
        Vector2 velocity = BallBounce.VelocityFromAngle(6f, 30f);

        Vector2 slowedVelocity = BallSpeed.WithVerticalSpeed(velocity, BallSpeed.Slowed(6f, 4f));

        Assert.AreEqual(4f, slowedVelocity.y, 1e-4f);
        Assert.AreEqual(4f * Mathf.Tan(30f * Mathf.Deg2Rad), slowedVelocity.x, 1e-4f);
    }
}
