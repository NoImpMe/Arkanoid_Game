using NUnit.Framework;
using UnityEngine;

public class BrickGridLayoutTests
{
    private const float Tolerance = 0.0001f;

    private BrickGridLayout CreateSpecLayout()
    {
        return new BrickGridLayout(6, 13, -6.5f, 9.07f, 2f, new Vector2(1f, 0.5f));
    }

    [Test]
    public void Count_IsThirteenBySixSeventyEight()
    {
        Assert.AreEqual(78, CreateSpecLayout().Count);
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
