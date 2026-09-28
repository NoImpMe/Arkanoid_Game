using UnityEngine;

[CreateAssetMenu(fileName = "PaddleConfig", menuName = "Arkanoid/Paddle Config")]
public class PaddleConfig : ScriptableObject
{
    [Header("Shape")]
    public float width = 2f;
    public float height = 0.5f;
    public float positionY = -8.35f;

    [Header("Movement")]
    public float moveSpeed = 12f;

    public Vector2 Size => new Vector2(width, height);
    public float HalfHeight => height * 0.5f;
}
