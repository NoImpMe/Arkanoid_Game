using System;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private ItemConfig itemConfig;
    [SerializeField] private LaserShot leftShot;
    [SerializeField] private LaserShot rightShot;

    private LaserMover mover;

    public bool CanFire => LaserMath.CanFire(leftShot.IsFlying, rightShot.IsFlying);

    private void Awake()
    {
        mover = LaserMover.FromConfig(itemConfig);
        ClearAll();
    }

    public bool TryFire(Paddle paddle)
    {
        if (!CanFire)
        {
            return false;
        }

        float paddleX = paddle.transform.position.x;
        float paddleTop = paddle.Area.yMax;
        float laserHeight = itemConfig.laserSize.y;
        leftShot.Fire(LaserMath.LeftMuzzle(paddleX, paddle.Width, paddleTop, itemConfig.laserMuzzleInset, laserHeight), itemConfig);
        rightShot.Fire(LaserMath.RightMuzzle(paddleX, paddle.Width, paddleTop, itemConfig.laserMuzzleInset, laserHeight), itemConfig);
        return true;
    }

    public void Step(float deltaTime, Action<Brick> onBrickDestroyed)
    {
        StepShot(leftShot, deltaTime, onBrickDestroyed);
        StepShot(rightShot, deltaTime, onBrickDestroyed);
    }

    public void ClearAll()
    {
        leftShot.Hide();
        rightShot.Hide();
    }

    private void StepShot(LaserShot shot, float deltaTime, Action<Brick> onBrickDestroyed)
    {
        if (!shot.IsFlying)
        {
            return;
        }

        LaserMoveResult result = mover.Move(shot.transform.position, itemConfig.laserSpeed * deltaTime);
        shot.MoveTo(result.Position);
        if (result.HitCollider == null)
        {
            return;
        }

        shot.Hide();
        if (LaserMover.TryDestroyBrick(result.HitCollider, out Brick destroyedBrick))
        {
            onBrickDestroyed(destroyedBrick);
        }
    }
}
