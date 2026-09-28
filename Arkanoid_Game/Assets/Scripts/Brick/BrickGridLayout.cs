using System.Collections.Generic;
using UnityEngine;

public class BrickGridLayout
{
    private readonly float left;
    private readonly float gridTop;
    private readonly Vector2 brickSize;
    private readonly HashSet<Vector2Int> goldCells = new HashSet<Vector2Int>();

    public int Rows { get; }
    public int Columns { get; }
    public int Count => Rows * Columns;
    public int GoldCount => goldCells.Count;
    public int DestructibleCount => Count - GoldCount;

    public BrickGridLayout(int rows, int columns, float left, float playAreaTop, float gapBelowTopWall, Vector2 brickSize,
        IEnumerable<Vector2Int> goldCellCandidates = null)
    {
        Rows = rows;
        Columns = columns;
        this.left = left;
        this.brickSize = brickSize;
        gridTop = playAreaTop - gapBelowTopWall;

        if (goldCellCandidates == null)
        {
            return;
        }

        foreach (Vector2Int cell in goldCellCandidates)
        {
            if (IsInsideGrid(cell))
            {
                goldCells.Add(cell);
            }
        }
    }

    public static BrickGridLayout FromConfig(StageConfig config)
    {
        return new BrickGridLayout(
            config.brickRows.Length,
            config.brickColumns,
            config.playAreaLeft,
            config.playAreaTop,
            config.gapBelowTopWall,
            config.brickSize,
            config.goldBrickCells);
    }

    public Vector2 CellCenter(int row, int column)
    {
        float x = left + (column + 0.5f) * brickSize.x;
        float y = gridTop - (row + 0.5f) * brickSize.y;
        return new Vector2(x, y);
    }

    public bool IsGoldCell(int row, int column)
    {
        return goldCells.Contains(new Vector2Int(row, column));
    }

    private bool IsInsideGrid(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < Rows && cell.y >= 0 && cell.y < Columns;
    }
}
