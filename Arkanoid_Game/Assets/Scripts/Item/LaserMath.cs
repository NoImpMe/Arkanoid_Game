using UnityEngine;

public static class LaserMath
{
    public static bool SpaceFiresLaser(bool hasHeldBall, PaddleMode paddleMode)
    {
        return !hasHeldBall && paddleMode == PaddleMode.Laser;
    }

    public static bool CanFire(bool leftFlying, bool rightFlying)
    {
        return !leftFlying && !rightFlying;
    }

    public static Vector2 LeftMuzzle(float paddleCenterX, float paddleWidth, float paddleTopY, float muzzleInset, float laserHeight)
    {
        return new Vector2(paddleCenterX - (paddleWidth * 0.5f - muzzleInset), paddleTopY + laserHeight * 0.5f);
    }

    public static Vector2 RightMuzzle(float paddleCenterX, float paddleWidth, float paddleTopY, float muzzleInset, float laserHeight)
    {
        return new Vector2(paddleCenterX + (paddleWidth * 0.5f - muzzleInset), paddleTopY + laserHeight * 0.5f);
    }
}
