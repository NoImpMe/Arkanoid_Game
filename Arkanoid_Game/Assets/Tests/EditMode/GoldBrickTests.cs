using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class GoldBrickTests
{
    private readonly List<Object> createdObjects = new List<Object>();
    private StageConfig config;
    private Sprite sprite;

    [SetUp]
    public void SetUp()
    {
        Texture2D texture = new Texture2D(4, 4);
        createdObjects.Add(texture);
        sprite = Sprite.Create(texture, new Rect(0f, 0f, 4f, 4f), new Vector2(0.5f, 0.5f), 4f);
        createdObjects.Add(sprite);

        config = ScriptableObject.CreateInstance<StageConfig>();
        config.goldBrickSprite = sprite;
        config.brickRows[5].sprite = sprite;
        createdObjects.Add(config);
    }

    [TearDown]
    public void TearDown()
    {
        foreach (Object createdObject in createdObjects)
        {
            Object.DestroyImmediate(createdObject);
        }
        createdObjects.Clear();
    }

    private Brick CreateBrick(BrickSetup setup)
    {
        GameObject root = new GameObject("Brick");
        createdObjects.Add(root);
        BoxCollider body = root.AddComponent<BoxCollider>();
        GameObject visualObject = new GameObject("Visual");
        visualObject.transform.SetParent(root.transform);
        GameObject shadowObject = new GameObject("Shadow");
        shadowObject.transform.SetParent(root.transform);
        Brick brick = root.AddComponent<Brick>();

        SerializedObject serializedBrick = new SerializedObject(brick);
        serializedBrick.FindProperty("body").objectReferenceValue = body;
        serializedBrick.FindProperty("visual").objectReferenceValue = visualObject.AddComponent<SpriteRenderer>();
        serializedBrick.FindProperty("shadow").objectReferenceValue = shadowObject.AddComponent<SpriteRenderer>();
        serializedBrick.ApplyModifiedPropertiesWithoutUndo();

        brick.Init(setup, config, Vector2.zero);
        return brick;
    }

    [Test]
    public void GoldSetup_IsIndestructibleWithZeroScoreAndConfigTint()
    {
        BrickSetup gold = BrickSetup.Gold(config);

        Assert.IsTrue(gold.IsIndestructible);
        Assert.AreEqual(0, gold.Score);
        Assert.AreEqual(config.goldBrickTint, gold.Tint);
        Assert.AreEqual(config.goldBrickHitFlashColor, gold.HitFlashColor);
        Assert.AreSame(config.goldBrickSprite, gold.Sprite);
    }

    [Test]
    public void GoldBrick_ManyHits_StaysActiveWithColliderAndReportsNoDestruction()
    {
        Brick brick = CreateBrick(BrickSetup.Gold(config));
        BoxCollider body = brick.GetComponent<BoxCollider>();

        for (int hit = 0; hit < 10; hit++)
        {
            Assert.IsFalse(brick.TakeHit());
        }

        Assert.IsTrue(brick.gameObject.activeSelf);
        Assert.IsTrue(body.enabled);
        Assert.IsTrue(brick.IsIndestructible);
        Assert.AreEqual(0, brick.Score);
    }

    [Test]
    public void GoldBrick_InitAppliesTintFromConfig()
    {
        Brick brick = CreateBrick(BrickSetup.Gold(config));

        SpriteRenderer visual = brick.transform.Find("Visual").GetComponent<SpriteRenderer>();
        Assert.AreEqual(config.goldBrickTint, visual.color);
    }

    [Test]
    public void GreenBrick_OneHit_IsDestroyedAndColliderDisabled()
    {
        Brick brick = CreateBrick(BrickSetup.FromRow(config.brickRows[5], config));
        BoxCollider body = brick.GetComponent<BoxCollider>();

        Assert.IsTrue(brick.TakeHit());
        Assert.IsFalse(body.enabled);
        Assert.IsFalse(brick.gameObject.activeSelf);
        Assert.AreEqual(80, brick.Score);
    }

    [Test]
    public void GoldHit_DoesNotRaiseScoreOrVerticalSpeed()
    {
        Brick gold = CreateBrick(BrickSetup.Gold(config));
        GameSession session = new GameSession(3, 0);
        float verticalSpeed = 6f;

        for (int hit = 0; hit < 5; hit++)
        {
            if (gold.TakeHit())
            {
                session.AddScore(gold.Score);
                verticalSpeed = BallSpeed.Increased(verticalSpeed, 0.05f, 9f);
            }
        }

        Assert.AreEqual(0, session.Score);
        Assert.AreEqual(6f, verticalSpeed);
    }

    [Test]
    public void BrickSetup_CanDropItem_OnlyNormalColorBricks()
    {
        Assert.IsFalse(BrickSetup.FromRow(config.brickRows[0], config).CanDropItem, "silver");
        for (int row = 1; row < config.brickRows.Length; row++)
        {
            Assert.IsTrue(BrickSetup.FromRow(config.brickRows[row], config).CanDropItem, $"row {row}");
        }
        Assert.IsFalse(BrickSetup.Gold(config).CanDropItem, "gold");
    }

    [Test]
    public void Brick_Init_CopiesCanDropItemFromSetup()
    {
        Assert.IsTrue(CreateBrick(BrickSetup.FromRow(config.brickRows[5], config)).CanDropItem);
        Assert.IsFalse(CreateBrick(BrickSetup.Gold(config)).CanDropItem);
    }
}
