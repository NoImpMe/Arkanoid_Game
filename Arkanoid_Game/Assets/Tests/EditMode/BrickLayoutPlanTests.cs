using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BrickLayoutPlanTests
{
    private const int Rows = 6;
    private const int Columns = 13;
    private const int SeedCount = 200;

    private static readonly Vector2Int[] SpecGoldCells =
    {
        new Vector2Int(5, 0),
        new Vector2Int(5, 3),
        new Vector2Int(5, 6),
        new Vector2Int(5, 9),
        new Vector2Int(5, 12)
    };

    private static readonly int[] RowScores = { 50, 90, 120, 100, 110, 80 };
    private static readonly int[] RowHits = { 2, 1, 1, 1, 1, 1 };

    private BrickLayoutPlan CreateSpecDefault()
    {
        return BrickLayoutPlan.CreateDefault(Rows, Columns, SpecGoldCells);
    }

    private BrickLayoutPlan CreateSpecRandom(int seed)
    {
        return BrickLayoutPlan.CreateRandom(CreateSpecDefault(), new System.Random(seed));
    }

    private static int[] CountByDefinition(BrickLayoutPlan plan)
    {
        int[] counts = new int[Rows];
        for (int row = 0; row < plan.Rows; row++)
        {
            for (int column = 0; column < plan.Columns; column++)
            {
                if (!plan.IsGold(row, column))
                {
                    counts[plan.RowDefinitionIndex(row, column)]++;
                }
            }
        }
        return counts;
    }

    private static int TotalScore(BrickLayoutPlan plan)
    {
        int[] counts = CountByDefinition(plan);
        int total = 0;
        for (int index = 0; index < counts.Length; index++)
        {
            total += counts[index] * RowScores[index];
        }
        return total;
    }

    private static int TotalHits(BrickLayoutPlan plan)
    {
        int[] counts = CountByDefinition(plan);
        int total = 0;
        for (int index = 0; index < counts.Length; index++)
        {
            total += counts[index] * RowHits[index];
        }
        return total;
    }

    private static bool AllDestructibleReachableFromBottom(BrickLayoutPlan plan)
    {
        bool[,] visited = new bool[plan.Rows, plan.Columns];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        int bottomRow = plan.Rows - 1;
        bool topSpaceReached = false;

        void Visit(int row, int column)
        {
            if (row < 0 || row >= plan.Rows || column < 0 || column >= plan.Columns)
            {
                return;
            }
            if (visited[row, column] || plan.IsGold(row, column))
            {
                return;
            }
            visited[row, column] = true;
            queue.Enqueue(new Vector2Int(row, column));
        }

        for (int column = 0; column < plan.Columns; column++)
        {
            Visit(bottomRow, column);
        }

        while (queue.Count > 0)
        {
            Vector2Int cell = queue.Dequeue();
            if (cell.x == 0 && !topSpaceReached)
            {
                topSpaceReached = true;
                for (int column = 0; column < plan.Columns; column++)
                {
                    Visit(0, column);
                }
            }
            Visit(cell.x - 1, cell.y);
            Visit(cell.x + 1, cell.y);
            Visit(cell.x, cell.y - 1);
            Visit(cell.x, cell.y + 1);
        }

        for (int row = 0; row < plan.Rows; row++)
        {
            for (int column = 0; column < plan.Columns; column++)
            {
                if (!plan.IsGold(row, column) && !visited[row, column])
                {
                    return false;
                }
            }
        }
        return true;
    }

    [Test]
    public void Default_CountsAreSeventyEightFiveGoldSeventyThreeDestructible()
    {
        BrickLayoutPlan plan = CreateSpecDefault();

        Assert.AreEqual(5, plan.GoldCount);
        Assert.AreEqual(73, plan.DestructibleCount);
        Assert.AreEqual(78, plan.GoldCount + plan.DestructibleCount);
    }

    [Test]
    public void Default_GoldOnlyAtSixthRowColumnsOneFourSevenTenThirteen_OthersUseOwnRow()
    {
        BrickLayoutPlan plan = CreateSpecDefault();
        int[] goldColumnsOneBased = { 1, 4, 7, 10, 13 };

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                bool expectedGold = row == 5 && Array.IndexOf(goldColumnsOneBased, column + 1) >= 0;
                Assert.AreEqual(expectedGold, plan.IsGold(row, column), $"row {row + 1}, column {column + 1}");
                if (!expectedGold)
                {
                    Assert.AreEqual(row, plan.RowDefinitionIndex(row, column));
                }
            }
        }
    }

    [Test]
    public void Default_TotalScoreIs6750AndHitsAre86()
    {
        BrickLayoutPlan plan = CreateSpecDefault();

        Assert.AreEqual(6750, TotalScore(plan));
        Assert.AreEqual(86, TotalHits(plan));
    }

    [Test]
    public void Default_GoldCellsOutOfRangeOrDuplicate_AreIgnored()
    {
        Vector2Int[] cells = { new Vector2Int(5, 0), new Vector2Int(5, 0), new Vector2Int(6, 0), new Vector2Int(0, 13), new Vector2Int(-1, 2) };

        BrickLayoutPlan plan = BrickLayoutPlan.CreateDefault(Rows, Columns, cells);

        Assert.AreEqual(1, plan.GoldCount);
        Assert.AreEqual(77, plan.DestructibleCount);
    }

    [Test]
    public void Default_DestroyingAllDestructible_ClearsStageWhileGoldRemains()
    {
        BrickLayoutPlan plan = CreateSpecDefault();
        int remaining = plan.DestructibleCount;

        for (int destroyed = 0; destroyed < 72; destroyed++)
        {
            remaining--;
        }
        Assert.IsFalse(GameSession.IsStageCleared(remaining));

        remaining--;
        Assert.IsTrue(GameSession.IsStageCleared(remaining));
        Assert.AreEqual(5, plan.GoldCount);
    }

    [Test]
    public void Default_FromStageConfigDefaults_MatchesSpecGoldCells()
    {
        StageConfig config = ScriptableObject.CreateInstance<StageConfig>();
        BrickLayoutPlan plan = BrickLayoutPlan.CreateDefault(config.brickRows.Length, config.brickColumns, config.goldBrickCells);
        UnityEngine.Object.DestroyImmediate(config);

        foreach (Vector2Int cell in SpecGoldCells)
        {
            Assert.IsTrue(plan.IsGold(cell.x, cell.y));
        }
        Assert.AreEqual(5, plan.GoldCount);
    }

    [Test]
    public void Random_GoldIsFiveInOneRowAtDistinctColumns()
    {
        for (int seed = 1; seed <= SeedCount; seed++)
        {
            BrickLayoutPlan plan = CreateSpecRandom(seed);
            HashSet<int> goldRows = new HashSet<int>();
            HashSet<int> goldColumns = new HashSet<int>();
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (plan.IsGold(row, column))
                    {
                        goldRows.Add(row);
                        goldColumns.Add(column);
                    }
                }
            }

            Assert.AreEqual(5, plan.GoldCount, $"seed {seed}");
            Assert.AreEqual(1, goldRows.Count, $"seed {seed}");
            Assert.AreEqual(5, goldColumns.Count, $"seed {seed}");
        }
    }

    [Test]
    public void Random_GoldRowVariesAcrossSeeds()
    {
        HashSet<int> observedGoldRows = new HashSet<int>();
        for (int seed = 1; seed <= SeedCount; seed++)
        {
            BrickLayoutPlan plan = CreateSpecRandom(seed);
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (plan.IsGold(row, column))
                    {
                        observedGoldRows.Add(row);
                    }
                }
            }
        }

        Assert.AreEqual(Rows, observedGoldRows.Count);
    }

    [Test]
    public void Random_CompositionTotalScoreAndHitsNeverChange()
    {
        int[] expectedCounts = CountByDefinition(CreateSpecDefault());

        for (int seed = 1; seed <= SeedCount; seed++)
        {
            BrickLayoutPlan plan = CreateSpecRandom(seed);

            CollectionAssert.AreEqual(expectedCounts, CountByDefinition(plan), $"seed {seed}");
            Assert.AreEqual(6750, TotalScore(plan), $"seed {seed}");
            Assert.AreEqual(86, TotalHits(plan), $"seed {seed}");
            Assert.AreEqual(73, plan.DestructibleCount, $"seed {seed}");
        }
    }

    [Test]
    public void Random_SameSeed_ProducesSameLayout()
    {
        BrickLayoutPlan first = CreateSpecRandom(42);
        BrickLayoutPlan second = CreateSpecRandom(42);

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                Assert.AreEqual(first.RowDefinitionIndex(row, column), second.RowDefinitionIndex(row, column));
            }
        }
    }

    [Test]
    public void Random_DifferentSeeds_ProduceDifferentLayouts()
    {
        BrickLayoutPlan first = CreateSpecRandom(1);
        BrickLayoutPlan second = CreateSpecRandom(2);
        bool anyDifference = false;

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                anyDifference |= first.RowDefinitionIndex(row, column) != second.RowDefinitionIndex(row, column);
            }
        }

        Assert.IsTrue(anyDifference);
    }

    [Test]
    public void Random_EveryDestructibleBrickIsReachableFromBottom()
    {
        for (int seed = 1; seed <= SeedCount; seed++)
        {
            Assert.IsTrue(AllDestructibleReachableFromBottom(CreateSpecRandom(seed)), $"seed {seed}");
        }
    }

    [Test]
    public void ReachabilityCheck_DetectsSealedPocket()
    {
        Vector2Int[] pocketAtLeftEdge = { new Vector2Int(1, 0), new Vector2Int(2, 1), new Vector2Int(3, 0) };

        BrickLayoutPlan sealedPlan = BrickLayoutPlan.CreateDefault(Rows, Columns, pocketAtLeftEdge);

        Assert.IsFalse(AllDestructibleReachableFromBottom(sealedPlan));
        Assert.IsTrue(AllDestructibleReachableFromBottom(CreateSpecDefault()));
    }

    [Test]
    public void Random_GoldCountNotFewerThanColumns_Throws()
    {
        Vector2Int[] fullRow = new Vector2Int[Columns];
        for (int column = 0; column < Columns; column++)
        {
            fullRow[column] = new Vector2Int(5, column);
        }
        BrickLayoutPlan source = BrickLayoutPlan.CreateDefault(Rows, Columns, fullRow);

        Assert.Throws<ArgumentException>(() => BrickLayoutPlan.CreateRandom(source, new System.Random(1)));
    }

    [Test]
    public void ForSession_FirstGame_ReturnsDefaultPlanUnchanged()
    {
        BrickLayoutPlan defaultPlan = CreateSpecDefault();

        BrickLayoutPlan chosen = BrickLayoutPlan.ForSession(defaultPlan, false, new System.Random(1));

        Assert.AreSame(defaultPlan, chosen);
    }

    [Test]
    public void ForSession_AfterGameFinished_ReturnsRandomPlanWithSameComposition()
    {
        BrickLayoutPlan defaultPlan = CreateSpecDefault();

        BrickLayoutPlan chosen = BrickLayoutPlan.ForSession(defaultPlan, true, new System.Random(1));

        Assert.AreNotSame(defaultPlan, chosen);
        CollectionAssert.AreEqual(CountByDefinition(defaultPlan), CountByDefinition(chosen));
        Assert.AreEqual(6750, TotalScore(chosen));
    }

    [Test]
    public void ForSession_AfterGameFinished_SameSeedMatchesCreateRandom()
    {
        BrickLayoutPlan viaSession = BrickLayoutPlan.ForSession(CreateSpecDefault(), true, new System.Random(9));
        BrickLayoutPlan direct = CreateSpecRandom(9);

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                Assert.AreEqual(direct.RowDefinitionIndex(row, column), viaSession.RowDefinitionIndex(row, column));
            }
        }
    }
}
