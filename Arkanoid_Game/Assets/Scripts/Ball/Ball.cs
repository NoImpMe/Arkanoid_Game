using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private BallConfig ballConfig;
    [SerializeField] private SpriteRenderer visual;

    private BallMover mover;
    private Action<Collider> brickHitHandler;
    private readonly CatchHold hold = new CatchHold();

    public event Action<Brick> BrickDestroyed;

    public Vector2 Velocity { get; private set; }
    public float VerticalSpeed { get; private set; }
    public float Radius => ballConfig.radius;
    public bool IsHeld => hold.IsHolding;

    private void Awake()
    {
        mover = BallMover.FromConfig(ballConfig);
        brickHitHandler = HandleBrickHit;
        visual.transform.localScale = SpriteFitting.ScaleToFit(visual.sprite.bounds.size, Vector2.one * ballConfig.Diameter);
    }

    public void PlaceAt(Vector3 position)
    {
        transform.position = position;
        Velocity = Vector2.zero;
        hold.Stop();
    }

    public void Launch()
    {
        hold.Stop();
        VerticalSpeed = ballConfig.initialVerticalSpeed;
        Velocity = BallBounce.VelocityFromAngle(VerticalSpeed, ballConfig.launchAngleDegrees);
    }

    public void SlowDown(float slowVerticalSpeed)
    {
        VerticalSpeed = BallSpeed.Slowed(VerticalSpeed, slowVerticalSpeed);
        Velocity = BallSpeed.WithVerticalSpeed(Velocity, VerticalSpeed);
    }

    public BallMoveResult Step(float deltaTime, bool catchOnPaddle)
    {
        Physics.SyncTransforms();
        BallMoveResult result = mover.Move(transform.position, Velocity, deltaTime, brickHitHandler, catchOnPaddle);
        transform.position = result.Position;

        if (result.ReachedDeadZone || result.CaughtByPaddle)
        {
            Velocity = Vector2.zero;
            return result;
        }

        Velocity = BallSpeed.WithVerticalSpeed(result.Velocity, VerticalSpeed);
        return result;
    }

    public void StartHold(Paddle paddle, float autoReleaseSeconds)
    {
        float halfWidth = paddle.Width * 0.5f;
        hold.Start(transform.position.x, paddle.transform.position.x, halfWidth, autoReleaseSeconds);
        Velocity = Vector2.zero;
        FollowPaddle(paddle, halfWidth);
    }

    public bool TickHold(Paddle paddle, float deltaTime)
    {
        FollowPaddle(paddle, paddle.Width * 0.5f);
        return hold.Tick(deltaTime);
    }

    public void ReleaseFromPaddle(Paddle paddle)
    {
        float hitOffset = BallBounce.PaddleHitOffset(transform.position.x, paddle.transform.position.x, paddle.Width);
        Velocity = BallBounce.BounceOffPaddle(VerticalSpeed, hitOffset, ballConfig.maxBounceAngleDegrees);
        hold.Stop();
    }

    private void FollowPaddle(Paddle paddle, float paddleHalfWidth)
    {
        Vector3 restPosition = paddle.BallRestPosition(Radius);
        restPosition.x = hold.HeldX(paddle.transform.position.x, paddleHalfWidth);
        transform.position = restPosition;
    }

    private void HandleBrickHit(Collider brickCollider)
    {
        if (!brickCollider.TryGetComponent(out Brick brick) || !brick.TakeHit())
        {
            return;
        }

        VerticalSpeed = BallSpeed.Increased(VerticalSpeed, ballConfig.verticalSpeedIncreasePerBrick, ballConfig.maxVerticalSpeed);
        BrickDestroyed?.Invoke(brick);
    }
}
