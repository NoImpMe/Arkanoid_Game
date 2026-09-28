using UnityEngine;

public readonly struct BallMoveResult
{
    public Vector3 Position { get; }
    public Vector2 Velocity { get; }
    public bool ReachedDeadZone { get; }
    public bool CaughtByPaddle { get; }

    public BallMoveResult(Vector3 position, Vector2 velocity, bool reachedDeadZone, bool caughtByPaddle = false)
    {
        Position = position;
        Velocity = velocity;
        ReachedDeadZone = reachedDeadZone;
        CaughtByPaddle = caughtByPaddle;
    }
}
