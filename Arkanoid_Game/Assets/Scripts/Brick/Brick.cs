using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] private BoxCollider body;
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private SpriteRenderer shadow;

    private StageConfig stageConfig;
    private BrickDurability durability;
    private Color originalColor;

    public int Score { get; private set; }

    public void Init(BrickRowDefinition row, StageConfig config, Vector2 center)
    {
        stageConfig = config;
        durability = new BrickDurability(row.durability);
        Score = row.score;

        transform.position = center;
        body.center = Vector3.zero;
        body.size = new Vector3(config.brickSize.x, config.brickSize.y, config.colliderDepth);

        Vector3 spriteScale = SpriteFitting.ScaleToFit(row.sprite.bounds.size, config.brickSize);
        visual.sprite = row.sprite;
        visual.transform.localScale = spriteScale;
        shadow.sprite = row.sprite;
        shadow.transform.localScale = spriteScale;
        shadow.transform.localPosition = config.brickShadowOffset;
        shadow.color = config.brickShadowColor;
        originalColor = visual.color;
    }

    public bool TakeHit()
    {
        if (durability.TakeHit())
        {
            body.enabled = false;
            gameObject.SetActive(false);
            return true;
        }

        StartCoroutine(FlashRoutine());
        return false;
    }

    private IEnumerator FlashRoutine()
    {
        visual.color = stageConfig.hitFlashColor;
        yield return new WaitForSeconds(stageConfig.hitFlashDuration);
        visual.color = originalColor;
    }
}
