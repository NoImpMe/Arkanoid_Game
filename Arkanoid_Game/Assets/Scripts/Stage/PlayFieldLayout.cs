using UnityEngine;

public class PlayFieldLayout
{
    public Rect LeftWall { get; }
    public Rect RightWall { get; }
    public Rect TopWall { get; }
    public Rect DeadZone { get; }

    public PlayFieldLayout(float left, float right, float top, float deadZoneY, float wallThickness)
    {
        float outerLeft = left - wallThickness;
        float outerRight = right + wallThickness;
        float outerTop = top + wallThickness;
        float outerBottom = deadZoneY - wallThickness;

        LeftWall = Rect.MinMaxRect(outerLeft, outerBottom, left, outerTop);
        RightWall = Rect.MinMaxRect(right, outerBottom, outerRight, outerTop);
        TopWall = Rect.MinMaxRect(outerLeft, top, outerRight, outerTop);
        DeadZone = Rect.MinMaxRect(left, outerBottom, right, deadZoneY);
    }

    public static PlayFieldLayout FromConfig(StageConfig config)
    {
        return new PlayFieldLayout(
            config.playAreaLeft,
            config.playAreaRight,
            config.playAreaTop,
            config.deadZoneY,
            config.wallThickness);
    }
}
