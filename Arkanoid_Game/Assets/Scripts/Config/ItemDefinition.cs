using System;
using UnityEngine;

[Serializable]
public class ItemDefinition
{
    public ItemType type;
    public string letter;
    public Color tint;
    public int dropWeight;

    public ItemDefinition(ItemType type, string letter, Color tint, int dropWeight)
    {
        this.type = type;
        this.letter = letter;
        this.tint = tint;
        this.dropWeight = dropWeight;
    }
}
