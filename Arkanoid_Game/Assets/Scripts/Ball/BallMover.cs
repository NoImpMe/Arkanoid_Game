using UnityEngine;

public class BallMover
{
    private readonly float radius;
    private readonly float skinWidth;
    private readonly int maxIterations;
    private readonly float maxBounceAngleDegrees;
    private readonly int collisionMask;
    private readonly int paddleMask;
    private readonly int deadZoneMask;
    private readonly Collider[] overlapBuffer = new Collider[1];

    public BallMover(float radius, float skinWidth, int maxIterations, float maxBounceAngleDegrees,
        int collisionMask, int paddleMask, int deadZoneMask)
    {
        this.radius = radius;
        this.skinWidth = skinWidth;
        this.maxIterations = maxIterations;
        this.maxBounceAngleDegrees = maxBounceAngleDegrees;
        this.collisionMask = collisionMask;
        this.paddleMask = paddleMask;
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
            config.deadZoneLayers);
    }

    public BallMoveResult Move(Vector3 position, Vector2 velocity, float deltaTime)
    {
        if (velocity.y < 0f && TryFindOverlappingPaddle(position, out Collider overlappedPaddle))
        {
            velocity = BounceOffPaddle(overlappedPaddle, position, velocity);
            position.y = Mathf.Max(position.y, overlappedPaddle.bounds.max.y + radius + skinWidth);
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

            velocity = IsInMask(hit.collider, paddleMask)
                ? BounceOffPaddle(hit.collider, position, velocity)
                : BallBounce.ReflectByNormalAxis(velocity, hit.normal);
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
