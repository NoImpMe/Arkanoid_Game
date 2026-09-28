using NUnit.Framework;
using UnityEngine;

public class DisruptionSplitTests
{
    private const float Tolerance = 0.001f;
    private const float MaxAngle = 60f;

    private static void AssertAngleAndVerticalSpeed(Vector2 velocity, float expectedAngle, float expectedVy)
    {
        Assert.AreEqual(expectedVy, velocity.y, Tolerance);
        Assert.AreEqual(expectedAngle, DisruptionSplit.AngleFromVertical(velocity), Tolerance);
    }

    [Test]
    public void Rotated_ThirtyDegreesUpward_SplitsToFifteenAndFortyFive()
    {
        Vector2 original = BallBounce.VelocityFromAngle(6f, 30f);

        AssertAngleAndVerticalSpeed(DisruptionSplit.Rotated(original, -15f, MaxAngle), 15f, 6f);
        AssertAngleAndVerticalSpeed(DisruptionSplit.Rotated(original, 15f, MaxAngle), 45f, 6f);
    }

    [Test]
    public void Rotated_BeyondMaxAngle_ClampsToSixty()
    {
        Vector2 original = BallBounce.VelocityFromAngle(6f, 50f);

        AssertAngleAndVerticalSpeed(DisruptionSplit.Rotated(original, -15f, MaxAngle), 35f, 6f);
        AssertAngleAndVerticalSpeed(DisruptionSplit.Rotated(original, 15f, MaxAngle), 60f, 6f);

        Vector2 leftward = BallBounce.VelocityFromAngle(6f, -55f);
        AssertAngleAndVerticalSpeed(DisruptionSplit.Rotated(leftward, -15f, MaxAngle), -60f, 6f);
    }

    [Test]
    public void Rotated_DownwardBall_KeepsDownwardDirectionAndVerticalSpeed()
    {
        Vector2 original = new Vector2(7f * Mathf.Tan(30f * Mathf.Deg2Rad), -7f);

        Vector2 rotated = DisruptionSplit.Rotated(original, 15f, MaxAngle);

        Assert.AreEqual(-7f, rotated.y, Tolerance);
        Assert.AreEqual(7f * Mathf.Tan(45f * Mathf.Deg2Rad), rotated.x, Tolerance);
    }

    [Test]
    public void Rotated_StraightUp_SplitsSymmetrically()
    {
        Vector2 original = new Vector2(0f, 4f);

        Vector2 left = DisruptionSplit.Rotated(original, -15f, MaxAngle);
        Vector2 right = DisruptionSplit.Rotated(original, 15f, MaxAngle);

        Assert.AreEqual(-right.x, left.x, Tolerance);
        Assert.AreEqual(4f, left.y, Tolerance);
        Assert.AreEqual(4f, right.y, Tolerance);
    }

    [TestCase(0, true)]
    [TestCase(1, false)]
    [TestCase(3, false)]
    public void ShouldLoseLife_OnlyWhenNoBallRemains(int activeBalls, bool expected)
    {
        Assert.AreEqual(expected, DisruptionSplit.ShouldLoseLife(activeBalls));
    }

    [TestCase(1, false)]
    [TestCase(2, true)]
    [TestCase(3, true)]
    public void HasMultipleBalls_TwoOrMore(int activeBalls, bool expected)
    {
        Assert.AreEqual(expected, DisruptionSplit.HasMultipleBalls(activeBalls));
    }

    [Test]
    public void DefaultConfig_ThreeBallsAndFifteenDegrees()
    {
        ItemConfig config = ScriptableObject.CreateInstance<ItemConfig>();

        Assert.AreEqual(3, config.disruptionBallCount);
        Assert.AreEqual(15f, config.disruptionSpreadDegrees, Tolerance);
        Object.DestroyImmediate(config);
    }
}
