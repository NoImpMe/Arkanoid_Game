using UnityEngine;

public class LaserMover
{
    private readonly Vector3 halfExtents;
    private readonly int hitMask;

    public LaserMover(Vector2 laserSize, int hitMask)
    {
        halfExtents = new Vector3(laserSize.x * 0.5f, laserSize.y * 0.5f, laserSize.x * 0.5f);
        this.hitMask = hitMask;
    }

    public static LaserMover FromConfig(ItemConfig config)
    {
        return new LaserMover(config.laserSize, config.laserHitLayers);
    }

    public LaserMoveResult Move(Vector3 center, float distance)
    {
        if (Physics.BoxCast(center, halfExtents, Vector3.up, out RaycastHit hit, Quaternion.identity, distance,
                hitMask, QueryTriggerInteraction.Ignore))
        {
            return new LaserMoveResult(center + Vector3.up * hit.distance, hit.collider);
        }

        return new LaserMoveResult(center + Vector3.up * distance, null);
    }

    public static bool TryDestroyBrick(Collider hitCollider, out Brick destroyedBrick)
    {
        destroyedBrick = null;
        if (hitCollider == null || !hitCollider.TryGetComponent(out Brick brick) || !brick.TakeHit())
        {
            return false;
        }

        destroyedBrick = brick;
        return true;
    }
}
