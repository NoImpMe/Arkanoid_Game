using UnityEngine;

public static class SpriteFitting
{
    public static Vector3 ScaleToFit(Vector2 spriteSize, Vector2 targetSize)
    {
        return new Vector3(targetSize.x / spriteSize.x, targetSize.y / spriteSize.y, 1f);
    }
}
