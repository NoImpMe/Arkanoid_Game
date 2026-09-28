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
        for (int row = 0; row < layout.Rows; row++)
        {
            for (int column = 0; column < layout.Columns; column++)
            {
                Brick brick = Instantiate(brickPrefab, transform);
                brick.name = $"Brick_{row}_{column}";
                brick.Init(stageConfig.brickRows[row], stageConfig, layout.CellCenter(row, column));
            }
        }
        RemainingCount = layout.Count;
    }
}
