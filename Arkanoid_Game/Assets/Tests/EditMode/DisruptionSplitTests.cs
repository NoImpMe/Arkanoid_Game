using NUnit.Framework;
using UnityEngine;

public class DisruptionSplitTests
{
    private const float Tolerance = 0.001f;
    private const float MaxAngle = 60f;

    private const float Spread = 15f;

    [TestCase(60f, 45f, 30f)]
    [TestCase(50f, 35f, 20f)]
    [TestCase(45f, 60f, 30f)]
    [TestCase(0f, 15f, -15f)]
    [TestCase(-45f, -30f, -60f)]
    [TestCase(-50f, -35f, -20f)]
    [TestCase(-60f, -45f, -30f)]
    public void SplitAngles_MatchesRule(float original, float expectedFirst, float expectedSecond)
    {
        (float first, float second) = DisruptionSplit.SplitAngles(original, Spread, MaxAngle);

        Assert.AreEqual(expectedFirst, first, Tolerance);
        Assert.AreEqual(expectedSecond, second, Tolerance);
    }

    [TestCase(60f)]
    [TestCase(50f)]
    [TestCase(45f)]
    [TestCase(0f)]
    [TestCase(-45f)]
    [TestCase(-50f)]
    [TestCase(-60f)]
    public void SplitAngles_ThreeBallsAllDifferentAndWithinMax(float original)
    {
        (float first, float second) = DisruptionSplit.SplitAngles(original, Spread, MaxAngle);

        Assert.Greater(Mathf.Abs(first - original), Tolerance);
        Assert.Greater(Mathf.Abs(second - original), Tolerance);
        Assert.Greater(Mathf.Abs(first - second), Tolerance);
        Assert.LessOrEqual(Mathf.Abs(first), MaxAngle + Tolerance);
        Assert.LessOrEqual(Mathf.Abs(second), MaxAngle + Tolerance);
    }

    [TestCase(60f, 45f, 30f)]
    [TestCase(50f, 35f, 20f)]
    [TestCase(45f, 60f, 30f)]
    [TestCase(0f, 15f, -15f)]
    [TestCase(-45f, -30f, -60f)]
    [TestCase(-50f, -35f, -20f)]
    [TestCase(-60f, -45f, -30f)]
    public void SplitVelocities_UpwardAndDownwardBalls_KeepVerticalSpeedAndDirection(float original, float expectedFirst, float expectedSecond)
    {
        foreach (float verticalSign in new[] { 1f, -1f })
        {
            Vector2 source = BallBounce.VelocityFromAngle(6f, original);
            source.y *= verticalSign;

            (float first, float second) = DisruptionSplit.SplitAngles(DisruptionSplit.AngleFromVertical(source), Spread, MaxAngle);
            Vector2 firstVelocity = DisruptionSplit.WithAngle(source, first, MaxAngle);
            Vector2 secondVelocity = DisruptionSplit.WithAngle(source, second, MaxAngle);

            Assert.AreEqual(6f * verticalSign, firstVelocity.y, Tolerance);
            Assert.AreEqual(6f * verticalSign, secondVelocity.y, Tolerance);
            Assert.AreEqual(expectedFirst, DisruptionSplit.AngleFromVertical(firstVelocity), Tolerance);
            Assert.AreEqual(expectedSecond, DisruptionSplit.AngleFromVertical(secondVelocity), Tolerance);
        }
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
