using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] private BoxCollider body;
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private SpriteRenderer shadow;

    private StageConfig stageConfig;
    private BrickDurability durability;
    private Color baseColor;
    private Color hitFlashColor;
    private Coroutine flashRoutine;

    public int Score { get; private set; }
    public bool IsIndestructible => durability.IsIndestructible;

    public void Init(BrickSetup setup, StageConfig config, Vector2 center)
    {
        stageConfig = config;
        durability = setup.CreateDurability();
        Score = setup.Score;
        baseColor = setup.Tint;
        hitFlashColor = setup.HitFlashColor;

        transform.position = center;
        body.center = Vector3.zero;
        body.size = new Vector3(config.brickSize.x, config.brickSize.y, config.colliderDepth);

        Vector3 spriteScale = SpriteFitting.ScaleToFit(setup.Sprite.bounds.size, config.brickSize);
        visual.sprite = setup.Sprite;
        visual.color = baseColor;
        visual.transform.localScale = spriteScale;
        shadow.sprite = setup.Sprite;
        shadow.transform.localScale = spriteScale;
        shadow.transform.localPosition = config.brickShadowOffset;
        shadow.color = config.brickShadowColor;
    }

    public bool TakeHit()
    {
        if (durability.TakeHit())
        {
            body.enabled = false;
            gameObject.SetActive(false);
            return true;
        }

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashRoutine());
        return false;
    }

    private IEnumerator FlashRoutine()
    {
        visual.color = hitFlashColor;
        yield return new WaitForSeconds(stageConfig.hitFlashDuration);
        visual.color = baseColor;
        flashRoutine = null;
    }
}
