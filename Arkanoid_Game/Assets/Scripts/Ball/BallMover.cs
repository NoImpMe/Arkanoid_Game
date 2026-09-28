using System;
using UnityEngine;

public class BallMover
{
    private readonly float radius;
    private readonly float skinWidth;
    private readonly int maxIterations;
    private readonly float maxBounceAngleDegrees;
    private readonly int collisionMask;
    private readonly int paddleMask;
    private readonly int brickMask;
    private readonly int deadZoneMask;
    private readonly Collider[] overlapBuffer = new Collider[1];

    public BallMover(float radius, float skinWidth, int maxIterations, float maxBounceAngleDegrees,
        int collisionMask, int paddleMask, int brickMask, int deadZoneMask)
    {
        this.radius = radius;
        this.skinWidth = skinWidth;
        this.maxIterations = maxIterations;
        this.maxBounceAngleDegrees = maxBounceAngleDegrees;
        this.collisionMask = collisionMask;
        this.paddleMask = paddleMask;
        this.brickMask = brickMask;
        this.deadZoneMask = deadZoneMask;
    }

    public static BallMover FromConfig(BallConfig config)
    {
        return new BallMover(
            config.radius,
            config.skinWidth,
            config.maxCollisionIterations,
            config.maxBounceAngleDegrees,
            config.collisionLayers,
            config.paddleLayers,
            config.brickLayers,
            config.deadZoneLayers);
    }

    public BallMoveResult Move(Vector3 position, Vector2 velocity, float deltaTime, Action<Collider> onBrickHit = null,
        bool catchOnPaddle = false)
    {
        if (velocity.y < 0f && TryFindOverlappingPaddle(position, out Collider overlappedPaddle))
        {
            position.y = Mathf.Max(position.y, overlappedPaddle.bounds.max.y + radius + skinWidth);
            if (catchOnPaddle)
            {
                return new BallMoveResult(position, velocity, false, true);
            }
            velocity = BounceOffPaddle(overlappedPaddle, position, velocity);
        }

        float remainingDistance = velocity.magnitude * deltaTime;

        for (int iteration = 0; iteration < maxIterations && remainingDistance > 0f; iteration++)
        {
            Vector3 direction = velocity.normalized;
            int mask = velocity.y > 0f ? collisionMask & ~paddleMask : collisionMask;

            if (!Physics.SphereCast(position, radius, direction, out RaycastHit hit,
                    remainingDistance + skinWidth, mask, QueryTriggerInteraction.Ignore))
            {
                position += direction * remainingDistance;
                remainingDistance = 0f;
                break;
            }

            float travel = Mathf.Max(hit.distance - skinWidth, 0f);
            position += direction * travel;
            remainingDistance -= travel;

            if (IsInMask(hit.collider, deadZoneMask))
            {
                return new BallMoveResult(position, velocity, true);
            }

            if (IsInMask(hit.collider, paddleMask))
            {
                if (catchOnPaddle)
                {
                    return new BallMoveResult(position, velocity, false, true);
                }
                velocity = BounceOffPaddle(hit.collider, position, velocity);
                continue;
            }

            velocity = BallBounce.ReflectByNormalAxis(velocity, hit.normal);
            if (IsInMask(hit.collider, brickMask))
            {
                onBrickHit?.Invoke(hit.collider);
            }
        }

        return new BallMoveResult(position, velocity, false);
    }

    private bool TryFindOverlappingPaddle(Vector3 position, out Collider paddle)
    {
        int count = Physics.OverlapSphereNonAlloc(position, radius, overlapBuffer, paddleMask,
            QueryTriggerInteraction.Ignore);
        paddle = count > 0 ? overlapBuffer[0] : null;
        return count > 0;
    }

    private Vector2 BounceOffPaddle(Collider paddle, Vector3 ballPosition, Vector2 velocity)
    {
        Bounds paddleBounds = paddle.bounds;
        float hitOffset = BallBounce.PaddleHitOffset(ballPosition.x, paddleBounds.center.x, paddleBounds.size.x);
        return BallBounce.BounceOffPaddle(velocity.y, hitOffset, maxBounceAngleDegrees);
    }

    private static bool IsInMask(Collider collider, int mask)
    {
        return (mask & (1 << collider.gameObject.layer)) != 0;
    }
}
