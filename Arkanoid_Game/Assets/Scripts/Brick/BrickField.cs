using UnityEngine;

public class BrickField : MonoBehaviour
{
    [SerializeField] private StageConfig stageConfig;
    [SerializeField] private Brick brickPrefab;

    public int RemainingCount { get; private set; }

    private void Awake()
    {
        SpawnBricks();
    }

    public void NotifyBrickDestroyed()
    {
        RemainingCount--;
    }

    private void SpawnBricks()
    {
        BrickGridLayout layout = BrickGridLayout.FromConfig(stageConfig);
        BrickLayoutPlan plan = CreatePlan(layout);
        BrickSetup goldSetup = BrickSetup.Gold(stageConfig);
        BrickSetup[] rowSetups = new BrickSetup[stageConfig.brickRows.Length];
        for (int index = 0; index < rowSetups.Length; index++)
        {
            rowSetups[index] = BrickSetup.FromRow(stageConfig.brickRows[index], stageConfig);
        }

        for (int row = 0; row < layout.Rows; row++)
        {
            for (int column = 0; column < layout.Columns; column++)
            {
                bool isGold = plan.IsGold(row, column);
                Brick brick = Instantiate(brickPrefab, transform);
                brick.name = isGold ? $"GoldBrick_{row}_{column}" : $"Brick_{row}_{column}";
                BrickSetup setup = isGold ? goldSetup : rowSetups[plan.RowDefinitionIndex(row, column)];
                brick.Init(setup, stageConfig, layout.CellCenter(row, column));
            }
        }
        RemainingCount = plan.DestructibleCount;
    }

    private BrickLayoutPlan CreatePlan(BrickGridLayout layout)
    {
        BrickLayoutPlan defaultPlan = BrickLayoutPlan.CreateDefault(layout.Rows, layout.Columns, stageConfig.goldBrickCells);
        return BrickLayoutPlan.ForSession(defaultPlan, LayoutSession.HasFinishedGame, new System.Random());
    }
}
