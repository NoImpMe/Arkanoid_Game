using UnityEngine;

[CreateAssetMenu(fileName = "ItemConfig", menuName = "Arkanoid/Item Config")]
public class ItemConfig : ScriptableObject
{
    [Header("Drop")]
    [Range(0f, 1f)] public float dropChance = 0.2f;
    public ItemDefinition[] items =
    {
        new ItemDefinition(ItemType.Lasers, "L", new Color(1f, 0.25f, 0.25f, 1f), 18),
        new ItemDefinition(ItemType.Enlarge, "E", new Color(0.35f, 0.55f, 1f, 1f), 18),
        new ItemDefinition(ItemType.Catch, "C", new Color(0.3f, 0.9f, 0.35f, 1f), 18),
        new ItemDefinition(ItemType.Slow, "S", new Color(1f, 0.6f, 0.15f, 1f), 18),
        new ItemDefinition(ItemType.Disruption, "D", new Color(0.45f, 0.9f, 1f, 1f), 18),
        new ItemDefinition(ItemType.Player, "P", new Color(0.75f, 0.75f, 0.75f, 1f), 10)
    };

    [Header("Capsule")]
    public Vector2 capsuleSize = new Vector2(1f, 0.5f);
    public float fallSpeed = 3f;
    public Sprite capsuleSprite;
    public float letterFontSize = 3f;
    public Color letterColor = Color.white;

    [Header("Player")]
    public int maxReserveLives = 5;

    [Header("Slow")]
    public float slowVerticalSpeed = 4f;

    [Header("Enlarge")]
    public float enlargeWidthMultiplier = 1.5f;
}
