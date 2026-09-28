using UnityEngine;

public static class WorldRect
{
    public static Rect FromCenter(Vector2 center, Vector2 size)
    {
        return new Rect(center - size * 0.5f, size);
    }
}
