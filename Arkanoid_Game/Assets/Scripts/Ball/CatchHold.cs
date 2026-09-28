using UnityEngine;

public class CatchHold
{
    private float autoReleaseSeconds;

    public bool IsHolding { get; private set; }
    public float OffsetX { get; private set; }
    public float ElapsedSeconds { get; private set; }

    public void Start(float ballX, float paddleX, float paddleHalfWidth, float autoReleaseSeconds)
    {
        this.autoReleaseSeconds = autoReleaseSeconds;
        OffsetX = Mathf.Clamp(ballX - paddleX, -paddleHalfWidth, paddleHalfWidth);
        ElapsedSeconds = 0f;
        IsHolding = true;
    }

    public bool Tick(float deltaTime)
    {
        if (!IsHolding)
        {
            return false;
        }

        ElapsedSeconds += deltaTime;
        return ElapsedSeconds >= autoReleaseSeconds;
    }

    public float HeldX(float paddleX, float paddleHalfWidth)
    {
        return paddleX + Mathf.Clamp(OffsetX, -paddleHalfWidth, paddleHalfWidth);
    }

    public void Stop()
    {
        IsHolding = false;
    }
}
