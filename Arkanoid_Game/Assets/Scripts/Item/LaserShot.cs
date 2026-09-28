using UnityEngine;

public class LaserShot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer visual;

    public bool IsFlying => gameObject.activeSelf;

    public void Fire(Vector2 position, ItemConfig config)
    {
        transform.position = position;
        visual.sprite = config.laserSprite;
        visual.color = config.laserTint;
        visual.transform.localScale = SpriteFitting.ScaleToFit(visual.sprite.bounds.size, config.laserSize);
        gameObject.SetActive(true);
    }

    public void MoveTo(Vector3 position)
    {
        transform.position = position;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
