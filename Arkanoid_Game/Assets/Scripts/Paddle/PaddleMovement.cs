using UnityEngine;

public static class PaddleMovement
{
    public static float InputDirection(bool leftPressed, bool rightPressed)
    {
        float direction = 0f;
        if (leftPressed)
        {
            direction -= 1f;
        }
        if (rightPressed)
        {
            direction += 1f;
        }
        return direction;
    }

    public static float NextX(float currentX, float direction, float speed, float deltaTime, float minX, float maxX)
    {
        return Mathf.Clamp(currentX + direction * speed * deltaTime, minX, maxX);
    }
}
