using System;
using UnityEngine;

[Serializable]
public class BrickRowDefinition
{
    public Sprite sprite;
    public int durability;
    public int score;

    public BrickRowDefinition(int durability, int score)
    {
        this.durability = durability;
        this.score = score;
    }
}
