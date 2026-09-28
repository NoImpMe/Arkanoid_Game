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
        BrickSetup goldSetup = BrickSetup.Gold(stageConfig);

        for (int row = 0; row < layout.Rows; row++)
        {
            BrickSetup rowSetup = BrickSetup.FromRow(stageConfig.brickRows[row], stageConfig);
            for (int column = 0; column < layout.Columns; column++)
            {
                bool isGold = layout.IsGoldCell(row, column);
                Brick brick = Instantiate(brickPrefab, transform);
                brick.name = isGold ? $"GoldBrick_{row}_{column}" : $"Brick_{row}_{column}";
                brick.Init(isGold ? goldSetup : rowSetup, stageConfig, layout.CellCenter(row, column));
            }
        }
        RemainingCount = layout.DestructibleCount;
    }
}
