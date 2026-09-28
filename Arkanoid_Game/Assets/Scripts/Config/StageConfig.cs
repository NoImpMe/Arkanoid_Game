using UnityEngine;

[CreateAssetMenu(fileName = "StageConfig", menuName = "Arkanoid/Stage Config")]
public class StageConfig : ScriptableObject
{
    [Header("Camera")]
    public float cameraOrthographicSize = 9.6f;
    public Vector3 cameraPosition = new Vector3(0f, 0f, -10f);

    [Header("Play Area")]
    public float playAreaLeft = -6.5f;
    public float playAreaRight = 6.5f;
    public float playAreaTop = 9.07f;
    public float deadZoneY = -9.6f;
    public float wallThickness = 0.5f;
    public float colliderDepth = 1f;

    [Header("Bricks")]
    public int brickColumns = 13;
    public Vector2 brickSize = new Vector2(1f, 0.5f);
    public float gapBelowTopWall = 2f;
    public BrickRowDefinition[] brickRows =
    {
        new BrickRowDefinition(2, 50),
        new BrickRowDefinition(1, 90),
        new BrickRowDefinition(1, 120),
        new BrickRowDefinition(1, 100),
        new BrickRowDefinition(1, 110),
        new BrickRowDefinition(1, 80)
    };

    [Header("Brick Visuals")]
    public Vector2 brickShadowOffset = new Vector2(0.25f, -0.25f);
    public Color brickShadowColor = new Color(0f, 0f, 0f, 0.5f);
    public float hitFlashDuration = 0.15f;
    public Color hitFlashColor = new Color(1f, 0.85f, 0.4f, 1f);
}
