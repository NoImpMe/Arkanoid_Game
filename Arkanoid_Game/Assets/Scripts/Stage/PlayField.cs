using UnityEngine;

public class PlayField : MonoBehaviour
{
    [SerializeField] private StageConfig stageConfig;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private BoxCollider leftWall;
    [SerializeField] private BoxCollider rightWall;
    [SerializeField] private BoxCollider topWall;
    [SerializeField] private BoxCollider deadZone;

    public PlayFieldLayout Layout { get; private set; }

    private void Awake()
    {
        Apply();
    }

    public void Apply()
    {
        Layout = PlayFieldLayout.FromConfig(stageConfig);

        gameCamera.orthographic = true;
        gameCamera.orthographicSize = stageConfig.cameraOrthographicSize;
        gameCamera.transform.position = stageConfig.cameraPosition;

        PlaceBox(leftWall, Layout.LeftWall);
        PlaceBox(rightWall, Layout.RightWall);
        PlaceBox(topWall, Layout.TopWall);
        PlaceBox(deadZone, Layout.DeadZone);
    }

    private void PlaceBox(BoxCollider box, Rect area)
    {
        box.transform.position = area.center;
        box.transform.rotation = Quaternion.identity;
        box.transform.localScale = Vector3.one;
        box.center = Vector3.zero;
        box.size = new Vector3(area.width, area.height, stageConfig.colliderDepth);
    }
}
