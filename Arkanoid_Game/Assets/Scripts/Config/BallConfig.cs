using UnityEngine;

[CreateAssetMenu(fileName = "BallConfig", menuName = "Arkanoid/Ball Config")]
public class BallConfig : ScriptableObject
{
    [Header("Shape")]
    public float radius = 0.15f;

    [Header("Speed")]
    public float initialVerticalSpeed = 6f;
    public float launchAngleDegrees = 30f;
    public float maxBounceAngleDegrees = 60f;
    public float verticalSpeedIncreasePerBrick = 0.05f;
    public float maxVerticalSpeed = 9f;

    [Header("Collision")]
    public float skinWidth = 0.01f;
    public int maxCollisionIterations = 4;
    public LayerMask collisionLayers;
    public LayerMask paddleLayers;
    public LayerMask brickLayers;
    public LayerMask deadZoneLayers;

    public float Diameter => radius * 2f;
}
