using NUnit.Framework;
using UnityEngine;

public class BrickGridLayoutTests
{
    private const float Tolerance = 0.0001f;

    private static readonly Vector2Int[] SpecGoldCells =
    {
        new Vector2Int(5, 0),
        new Vector2Int(5, 3),
        new Vector2Int(5, 6),
        new Vector2Int(5, 9),
        new Vector2Int(5, 12)
    };

    private BrickGridLayout CreateSpecLayout()
    {
        return new BrickGridLayout(6, 13, -6.5f, 9.07f, 2f, new Vector2(1f, 0.5f), SpecGoldCells);
    }

    [Test]
    public void Count_IsSeventyEightWithFiveGoldAndSeventyThreeDestructible()
    {
        BrickGridLayout layout = CreateSpecLayout();

        Assert.AreEqual(78, layout.Count);
        Assert.AreEqual(5, layout.GoldCount);
        Assert.AreEqual(73, layout.DestructibleCount);
    }

    [Test]
    public void IsGoldCell_OnlySixthRowColumnsOneFourSevenTenThirteen()
    {
        BrickGridLayout layout = CreateSpecLayout();
        int[] goldColumnsOneBased = { 1, 4, 7, 10, 13 };

        for (int row = 0; row < layout.Rows; row++)
        {
            for (int column = 0; column < layout.Columns; column++)
            {
                bool expected = row == 5 && System.Array.IndexOf(goldColumnsOneBased, column + 1) >= 0;
                Assert.AreEqual(expected, layout.IsGoldCell(row, column), $"row {row + 1}, column {column + 1}");
            }
        }
    }

    [Test]
    public void GoldCells_OutOfRangeOrDuplicate_AreIgnored()
    {
        Vector2Int[] cells = { new Vector2Int(5, 0), new Vector2Int(5, 0), new Vector2Int(6, 0), new Vector2Int(0, 13), new Vector2Int(-1, 2) };

        BrickGridLayout layout = new BrickGridLayout(6, 13, -6.5f, 9.07f, 2f, new Vector2(1f, 0.5f), cells);

        Assert.AreEqual(1, layout.GoldCount);
        Assert.AreEqual(77, layout.DestructibleCount);
    }

    [Test]
    public void DestroyingAllDestructible_ClearsStageWhileGoldRemains()
    {
        BrickGridLayout layout = CreateSpecLayout();
        int remaining = layout.DestructibleCount;

        for (int destroyed = 0; destroyed < 72; destroyed++)
        {
            remaining--;
        }
        Assert.IsFalse(GameSession.IsStageCleared(remaining));

        remaining--;
        Assert.IsTrue(GameSession.IsStageCleared(remaining));
        Assert.AreEqual(5, layout.GoldCount);
    }

    [Test]
    public void FromConfig_DefaultStageConfigHasSpecGoldCells()
    {
        StageConfig config = ScriptableObject.CreateInstance<StageConfig>();
        BrickGridLayout fromConfig = BrickGridLayout.FromConfig(config);
        Object.DestroyImmediate(config);

        Assert.AreEqual(5, fromConfig.GoldCount);
        foreach (Vector2Int cell in SpecGoldCells)
        {
            Assert.IsTrue(fromConfig.IsGoldCell(cell.x, cell.y));
        }
    }

    [Test]
    public void CellCenter_FirstAndLastMatchSpec()
    {
        BrickGridLayout layout = CreateSpecLayout();

        Vector2 first = layout.CellCenter(0, 0);
        Vector2 last = layout.CellCenter(5, 12);

        Assert.AreEqual(-6f, first.x, Tolerance);
        Assert.AreEqual(6.82f, first.y, Tolerance);
        Assert.AreEqual(6f, last.x, Tolerance);
        Assert.AreEqual(4.32f, last.y, Tolerance);
    }

    [Test]
    public void EdgeBricks_DoNotOverlapSideWalls()
    {
        BrickGridLayout layout = CreateSpecLayout();
        float halfWidth = 0.5f;

        Assert.GreaterOrEqual(layout.CellCenter(0, 0).x - halfWidth, -6.5f - Tolerance);
        Assert.LessOrEqual(layout.CellCenter(0, 12).x + halfWidth, 6.5f + Tolerance);
    }

    [Test]
    public void AdjacentBricks_HaveZeroGap()
    {
        BrickGridLayout layout = CreateSpecLayout();

        Assert.AreEqual(1f, layout.CellCenter(0, 1).x - layout.CellCenter(0, 0).x, Tolerance);
        Assert.AreEqual(0.5f, layout.CellCenter(0, 0).y - layout.CellCenter(1, 0).y, Tolerance);
    }

    [Test]
    public void FromConfig_DefaultStageConfigMatchesSpecLayout()
    {
        StageConfig config = ScriptableObject.CreateInstance<StageConfig>();
        BrickGridLayout fromConfig = BrickGridLayout.FromConfig(config);
        Object.DestroyImmediate(config);

        Assert.AreEqual(78, fromConfig.Count);
        Assert.AreEqual(CreateSpecLayout().CellCenter(3, 7), fromConfig.CellCenter(3, 7));
    }

    [Test]
    public void DefaultStageConfig_RowDurabilityAndScoreMatchSpec()
    {
        StageConfig config = ScriptableObject.CreateInstance<StageConfig>();
        int[] expectedDurability = { 2, 1, 1, 1, 1, 1 };
        int[] expectedScore = { 50, 90, 120, 100, 110, 80 };

        for (int row = 0; row < expectedScore.Length; row++)
        {
            Assert.AreEqual(expectedDurability[row], config.brickRows[row].durability);
            Assert.AreEqual(expectedScore[row], config.brickRows[row].score);
        }
        Object.DestroyImmediate(config);
    }
}
