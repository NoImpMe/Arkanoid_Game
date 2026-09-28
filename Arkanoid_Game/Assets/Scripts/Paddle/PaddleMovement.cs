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

    public static Vector2 MovementRange(float playAreaLeft, float playAreaRight, float paddleWidth)
    {
        float halfWidth = paddleWidth * 0.5f;
        return new Vector2(playAreaLeft + halfWidth, playAreaRight - halfWidth);
    }

    public static float ClampToRange(float x, Vector2 movementRange)
    {
        return Mathf.Clamp(x, movementRange.x, movementRange.y);
    }
}
