using TMPro;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private TextMeshPro letter;

    private Vector2 size;
    private float fallSpeed;

    public ItemType Type { get; private set; }
    public bool IsFalling => gameObject.activeSelf;
    public Rect Area => WorldRect.FromCenter(transform.position, size);

    public void Show(ItemDefinition item, Vector2 position, ItemConfig config)
    {
        Type = item.type;
        size = config.capsuleSize;
        fallSpeed = config.fallSpeed;
        transform.position = position;

        visual.sprite = config.capsuleSprite;
        visual.color = item.tint;
        visual.transform.localScale = SpriteFitting.ScaleToFit(visual.sprite.bounds.size, size);

        letter.rectTransform.sizeDelta = size;
        letter.text = item.letter;
        letter.fontSize = config.letterFontSize;
        letter.color = config.letterColor;

        gameObject.SetActive(true);
    }

    public void Fall(float deltaTime)
    {
        transform.position = CapsuleMotion.Fall(transform.position, fallSpeed, deltaTime);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
