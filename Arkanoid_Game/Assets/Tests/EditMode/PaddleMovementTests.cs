using NUnit.Framework;

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
}
