using UnityEngine;

public readonly struct LaserMoveResult
{
    public Vector3 Position { get; }
    public Collider HitCollider { get; }

    public LaserMoveResult(Vector3 position, Collider hitCollider)
    {
        Position = position;
        HitCollider = hitCollider;
    }
}
