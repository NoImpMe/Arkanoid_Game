using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private ItemConfig itemConfig;
    [SerializeField] private StageConfig stageConfig;
    [SerializeField] private Capsule capsule;

    private ItemDropper dropper;

    public bool IsCapsuleFalling => capsule.IsFalling;

    private void Awake()
    {
        dropper = ItemDropper.FromConfig(itemConfig, new System.Random());
        capsule.Hide();
    }

    public void TryDrop(Vector2 position, bool brickCanDropItem, bool hasMultipleBalls)
    {
        if (dropper.TryDrop(brickCanDropItem, IsCapsuleFalling, hasMultipleBalls, out ItemDefinition droppedItem))
        {
            capsule.Show(droppedItem, position, itemConfig);
        }
    }

    public bool Step(float deltaTime, Rect paddleArea, out ItemType caughtType)
    {
        caughtType = default;
        if (!capsule.IsFalling)
        {
            return false;
        }

        Rect areaBefore = capsule.Area;
        capsule.Fall(deltaTime);
        Rect areaAfter = capsule.Area;

        if (CapsuleMotion.IsCaught(areaBefore, areaAfter, paddleArea))
        {
            caughtType = capsule.Type;
            capsule.Hide();
            return true;
        }

        if (CapsuleMotion.HasLeftField(areaAfter, stageConfig.deadZoneY))
        {
            capsule.Hide();
        }
        return false;
    }

    public void ClearAll()
    {
        capsule.Hide();
    }
}
