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
    public float playAreaTop = 8.1f;
    public float deadZoneY = -9.6f;
    public float wallThickness = 0.5f;
    public float colliderDepth = 1f;
}
