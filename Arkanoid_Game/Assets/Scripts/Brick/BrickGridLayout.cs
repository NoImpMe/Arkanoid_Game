using UnityEngine;

public class BrickGridLayout
{
    private readonly float left;
    private readonly float gridTop;
    private readonly Vector2 brickSize;

    public int Rows { get; }
    public int Columns { get; }
    public int Count => Rows * Columns;

    public BrickGridLayout(int rows, int columns, float left, float playAreaTop, float gapBelowTopWall, Vector2 brickSize)
    {
        Rows = rows;
        Columns = columns;
        this.left = left;
        this.brickSize = brickSize;
        gridTop = playAreaTop - gapBelowTopWall;
    }

    public static BrickGridLayout FromConfig(StageConfig config)
    {
        return new BrickGridLayout(
            config.brickRows.Length,
            config.brickColumns,
            config.playAreaLeft,
            config.playAreaTop,
            config.gapBelowTopWall,
            config.brickSize);
    }

    public Vector2 CellCenter(int row, int column)
    {
        float x = left + (column + 0.5f) * brickSize.x;
        float y = gridTop - (row + 0.5f) * brickSize.y;
        return new Vector2(x, y);
    }
}
