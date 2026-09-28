using NUnit.Framework;
using UnityEngine;

public class PaddleMovementTests
{
    private const float Tolerance = 0.0001f;

    [Test]
    public void InputDirection_LeftRightBothNone()
    {
        Assert.AreEqual(-1f, PaddleMovement.InputDirection(true, false));
        Assert.AreEqual(1f, PaddleMovement.InputDirection(false, true));
        Assert.AreEqual(0f, PaddleMovement.InputDirection(true, true));
        Assert.AreEqual(0f, PaddleMovement.InputDirection(false, false));
    }

    [Test]
    public void NextX_MovesBySpeedTimesDeltaTime()
    {
        Assert.AreEqual(1.2f, PaddleMovement.NextX(0f, 1f, 12f, 0.1f, -5.5f, 5.5f), Tolerance);
        Assert.AreEqual(-1.2f, PaddleMovement.NextX(0f, -1f, 12f, 0.1f, -5.5f, 5.5f), Tolerance);
    }

    [Test]
    public void NextX_ClampsToMovementRange()
    {
        Assert.AreEqual(5.5f, PaddleMovement.NextX(5f, 1f, 12f, 0.1f, -5.5f, 5.5f), Tolerance);
        Assert.AreEqual(-5.5f, PaddleMovement.NextX(-5f, -1f, 12f, 0.1f, -5.5f, 5.5f), Tolerance);
    }

    [Test]
    public void MovementRange_NormalAndEnlargedWidth_MatchSpec()
    {
        Vector2 normalRange = PaddleMovement.MovementRange(-6.5f, 6.5f, 2f);
        Vector2 enlargedRange = PaddleMovement.MovementRange(-6.5f, 6.5f, 3f);

        Assert.AreEqual(-5.5f, normalRange.x, Tolerance);
        Assert.AreEqual(5.5f, normalRange.y, Tolerance);
        Assert.AreEqual(-5f, enlargedRange.x, Tolerance);
        Assert.AreEqual(5f, enlargedRange.y, Tolerance);
    }

    [Test]
    public void MovementRange_DefaultConfigs_IsPlusMinusFivePointFive()
    {
        StageConfig stageConfig = ScriptableObject.CreateInstance<StageConfig>();
        PaddleConfig paddleConfig = ScriptableObject.CreateInstance<PaddleConfig>();

        Vector2 range = PaddleMovement.MovementRange(stageConfig.playAreaLeft, stageConfig.playAreaRight, paddleConfig.width);

        Assert.AreEqual(-5.5f, range.x, Tolerance);
        Assert.AreEqual(5.5f, range.y, Tolerance);
        Object.DestroyImmediate(stageConfig);
        Object.DestroyImmediate(paddleConfig);
    }

    [Test]
    public void ClampToRange_OutsideIsPulledInInsideIsKept()
    {
        Vector2 range = new Vector2(-5f, 5f);

        Assert.AreEqual(5f, PaddleMovement.ClampToRange(5.5f, range), Tolerance);
        Assert.AreEqual(-5f, PaddleMovement.ClampToRange(-5.5f, range), Tolerance);
        Assert.AreEqual(3f, PaddleMovement.ClampToRange(3f, range), Tolerance);
    }
}
