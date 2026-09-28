using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private BallConfig ballConfig;
    [SerializeField] private SpriteRenderer visual;

    private BallMover mover;
    private Action<Collider> brickHitHandler;

    public event Action<Brick> BrickDestroyed;

    public Vector2 Velocity { get; private set; }
    public float VerticalSpeed { get; private set; }
    public float Radius => ballConfig.radius;

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
    }

    public void Launch()
    {
        VerticalSpeed = ballConfig.initialVerticalSpeed;
        Velocity = BallBounce.VelocityFromAngle(VerticalSpeed, ballConfig.launchAngleDegrees);
    }

    public void SlowDown(float slowVerticalSpeed)
    {
        VerticalSpeed = BallSpeed.Slowed(VerticalSpeed, slowVerticalSpeed);
        Velocity = BallSpeed.WithVerticalSpeed(Velocity, VerticalSpeed);
    }

    public bool Step(float deltaTime)
    {
        Physics.SyncTransforms();
        BallMoveResult result = mover.Move(transform.position, Velocity, deltaTime, brickHitHandler);
        transform.position = result.Position;

        if (result.ReachedDeadZone)
        {
            Velocity = Vector2.zero;
            return true;
        }

        Velocity = BallSpeed.WithVerticalSpeed(result.Velocity, VerticalSpeed);
        return false;
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
