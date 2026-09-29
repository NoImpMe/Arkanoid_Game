using UnityEngine;

public static class BallBounce
{
    public static Vector2 VelocityFromAngle(float verticalSpeed, float angleDegrees)
    {
        float speed = Mathf.Abs(verticalSpeed);
        return new Vector2(speed * Mathf.Tan(angleDegrees * Mathf.Deg2Rad), speed);
    }

    public static Vector2 ReflectByNormalAxis(Vector2 velocity, Vector2 normal)
    {
        float absNormalX = Mathf.Abs(normal.x);
        float absNormalY = Mathf.Abs(normal.y);
        bool flipX = absNormalX >= absNormalY;
        bool flipY = absNormalY >= absNormalX;

        Vector2 reflected = ReflectAxes(velocity, normal, flipX, flipY);
        if (Vector2.Dot(reflected, normal) < 0f)
        {
            reflected = ReflectAxes(velocity, normal, true, true);
        }
        return reflected;
    }

    private static Vector2 ReflectAxes(Vector2 velocity, Vector2 normal, bool flipX, bool flipY)
    {
        float vx = flipX ? Mathf.Abs(velocity.x) * Mathf.Sign(normal.x) : velocity.x;
        float vy = flipY ? Mathf.Abs(velocity.y) * Mathf.Sign(normal.y) : velocity.y;
        return new Vector2(vx, vy);
    }

    public static float PaddleHitOffset(float ballX, float paddleCenterX, float paddleWidth)
    {
        float halfWidth = paddleWidth * 0.5f;
        return Mathf.Clamp((ballX - paddleCenterX) / halfWidth, -1f, 1f);
    }

    public static Vector2 BounceOffPaddle(float verticalSpeed, float hitOffset, float maxBounceAngleDegrees)
    {
        return VelocityFromAngle(verticalSpeed, hitOffset * maxBounceAngleDegrees);
    }
}
