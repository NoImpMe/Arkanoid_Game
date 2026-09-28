using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class PaddleTests
{
    private const float Tolerance = 0.0001f;

    private readonly List<Object> createdObjects = new List<Object>();
    private Paddle paddle;
    private BoxCollider body;

    [SetUp]
    public void SetUp()
    {
        Texture2D texture = new Texture2D(4, 4);
        createdObjects.Add(texture);
        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, 4f, 4f), new Vector2(0.5f, 0.5f), 4f);
        createdObjects.Add(sprite);
        PaddleConfig paddleConfig = ScriptableObject.CreateInstance<PaddleConfig>();
        createdObjects.Add(paddleConfig);
        StageConfig stageConfig = ScriptableObject.CreateInstance<StageConfig>();
        createdObjects.Add(stageConfig);

        GameObject root = new GameObject("Paddle");
        createdObjects.Add(root);
        body = root.AddComponent<BoxCollider>();
        GameObject visualObject = new GameObject("Visual");
        visualObject.transform.SetParent(root.transform);
        SpriteRenderer visual = visualObject.AddComponent<SpriteRenderer>();
        visual.sprite = sprite;
        paddle = root.AddComponent<Paddle>();

        SerializedObject serializedPaddle = new SerializedObject(paddle);
        serializedPaddle.FindProperty("paddleConfig").objectReferenceValue = paddleConfig;
        serializedPaddle.FindProperty("stageConfig").objectReferenceValue = stageConfig;
        serializedPaddle.FindProperty("body").objectReferenceValue = body;
        serializedPaddle.FindProperty("visual").objectReferenceValue = visual;
        serializedPaddle.ApplyModifiedPropertiesWithoutUndo();

        root.transform.position = new Vector3(0f, paddleConfig.positionY, 0f);
        paddle.SetWidth(paddleConfig.width);
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

    [Test]
    public void SetWidth_Enlarged_ResizesColliderAndArea()
    {
        paddle.SetWidth(3f);

        Assert.AreEqual(3f, paddle.Width, Tolerance);
        Assert.AreEqual(3f, body.size.x, Tolerance);
        Assert.AreEqual(3f, paddle.Area.width, Tolerance);
    }

    [Test]
    public void SetWidth_EnlargedNearRightWall_PullsPaddleInsideNewRange()
    {
        paddle.transform.position = new Vector3(5.5f, paddle.transform.position.y, 0f);

        paddle.SetWidth(3f);

        Assert.AreEqual(5f, paddle.transform.position.x, Tolerance);
        Assert.AreEqual(6.5f, paddle.Area.xMax, Tolerance);
    }

    [Test]
    public void SetWidth_BackToNormal_KeepsPositionAndRestoresWidth()
    {
        paddle.SetWidth(3f);
        paddle.transform.position = new Vector3(-5f, paddle.transform.position.y, 0f);

        paddle.SetWidth(2f);

        Assert.AreEqual(-5f, paddle.transform.position.x, Tolerance);
        Assert.AreEqual(2f, body.size.x, Tolerance);
    }
}
