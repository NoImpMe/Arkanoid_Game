using UnityEngine;

public static class BallSpeed
{
    public static float Increased(float verticalSpeed, float increment, float maxVerticalSpeed)
    {
        return Mathf.Min(verticalSpeed + increment, maxVerticalSpeed);
    }

    public static Vector2 WithVerticalSpeed(Vector2 velocity, float verticalSpeed)
    {
        float currentVerticalSpeed = Mathf.Abs(velocity.y);
        if (Mathf.Approximately(currentVerticalSpeed, 0f))
        {
            return velocity;
        }
        return velocity * (verticalSpeed / currentVerticalSpeed);
    }
}
