using System;
using System.Collections.Generic;
using UnityEngine;

public class BrickLayoutPlan
{
    public const int GoldCell = -1;
    private const int NewSeedEveryTime = 0;

    private readonly int[,] cells;

    public int Rows { get; }
    public int Columns { get; }
    public int GoldCount { get; }
    public int DestructibleCount => Rows * Columns - GoldCount;

    private BrickLayoutPlan(int[,] cells)
    {
        this.cells = cells;
        Rows = cells.GetLength(0);
        Columns = cells.GetLength(1);
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                if (cells[row, column] == GoldCell)
                {
                    GoldCount++;
                }
            }
        }
    }

    public static BrickLayoutPlan CreateDefault(int rows, int columns, IEnumerable<Vector2Int> goldCells)
    {
        int[,] cells = new int[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                cells[row, column] = row;
            }
        }

        if (goldCells != null)
        {
            foreach (Vector2Int cell in goldCells)
            {
                if (cell.x >= 0 && cell.x < rows && cell.y >= 0 && cell.y < columns)
                {
                    cells[cell.x, cell.y] = GoldCell;
                }
            }
        }

        return new BrickLayoutPlan(cells);
    }

    public static BrickLayoutPlan CreateRandom(BrickLayoutPlan source, System.Random random)
    {
        if (source.GoldCount >= source.Columns)
        {
            throw new ArgumentException("Gold bricks must be fewer than the number of columns to keep every brick reachable.");
        }

        List<int> destructibleBricks = new List<int>();
        for (int row = 0; row < source.Rows; row++)
        {
            for (int column = 0; column < source.Columns; column++)
            {
                if (!source.IsGold(row, column))
                {
                    destructibleBricks.Add(source.cells[row, column]);
                }
            }
        }

        bool[,] isGold = new bool[source.Rows, source.Columns];
        int goldRow = random.Next(source.Rows);
        int[] columnOrder = new int[source.Columns];
        for (int column = 0; column < columnOrder.Length; column++)
        {
            columnOrder[column] = column;
        }
        Shuffle(columnOrder, random);
        for (int index = 0; index < source.GoldCount; index++)
        {
            isGold[goldRow, columnOrder[index]] = true;
        }

        Shuffle(destructibleBricks, random);
        int[,] cells = new int[source.Rows, source.Columns];
        int nextBrick = 0;
        for (int row = 0; row < source.Rows; row++)
        {
            for (int column = 0; column < source.Columns; column++)
            {
                cells[row, column] = isGold[row, column] ? GoldCell : destructibleBricks[nextBrick++];
            }
        }

        return new BrickLayoutPlan(cells);
    }

    public static System.Random CreateRandomSource(int seed)
    {
        return seed == NewSeedEveryTime ? new System.Random() : new System.Random(seed);
    }

    public bool IsGold(int row, int column)
    {
        return cells[row, column] == GoldCell;
    }

    public int RowDefinitionIndex(int row, int column)
    {
        return cells[row, column];
    }

    private static void Shuffle<T>(IList<T> items, System.Random random)
    {
        for (int index = items.Count - 1; index > 0; index--)
        {
            int swapIndex = random.Next(index + 1);
            T temporary = items[index];
            items[index] = items[swapIndex];
            items[swapIndex] = temporary;
        }
    }
}
