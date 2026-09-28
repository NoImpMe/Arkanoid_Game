using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class LaserMoverTests
{
    private const float Tolerance = 0.001f;
    private const float MaxFrameTime = 0.3333f;
    private static readonly Vector2 LaserSize = new Vector2(0.1f, 0.5f);

    private readonly List<GameObject> createdObjects = new List<GameObject>();
    private LaserMover mover;

    [SetUp]
    public void SetUp()
    {
        mover = new LaserMover(LaserSize, LayerMask.GetMask("Wall", "Brick"));
    }

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject createdObject in createdObjects)
        {
            Object.DestroyImmediate(createdObject);
        }
        createdObjects.Clear();
    }

    private BoxCollider CreateBox(string layerName, Vector2 center, Vector2 size)
    {
        GameObject box = new GameObject(layerName);
        createdObjects.Add(box);
        box.layer = LayerMask.NameToLayer(layerName);
        box.transform.position = center;
        BoxCollider collider = box.AddComponent<BoxCollider>();
        collider.size = new Vector3(size.x, size.y, 1f);
        Physics.SyncTransforms();
        return collider;
    }

    [Test]
    public void Move_NoObstacle_MovesStraightUpByDistance()
    {
        LaserMoveResult result = mover.Move(new Vector3(1f, -7.85f, 0f), 1.5f);

        Assert.IsNull(result.HitCollider);
        Assert.AreEqual(1f, result.Position.x, Tolerance);
        Assert.AreEqual(-6.35f, result.Position.y, Tolerance);
    }

    [Test]
    public void Move_IntoBrick_StopsAtBrickBottomAndReportsIt()
    {
        BoxCollider brick = CreateBox("Brick", new Vector2(0f, 2f), new Vector2(1f, 0.5f));

        LaserMoveResult result = mover.Move(new Vector3(0.3f, 0f, 0f), 5f);

        Assert.AreSame(brick, result.HitCollider);
        Assert.AreEqual(1.75f - LaserSize.y * 0.5f, result.Position.y, Tolerance);
    }

    [Test]
    public void Move_MaxFrameTime_DoesNotTunnelThroughThinBrick()
    {
        BoxCollider brick = CreateBox("Brick", new Vector2(0f, 2f), new Vector2(1f, 0.5f));

        LaserMoveResult result = mover.Move(new Vector3(0f, 0f, 0f), 15f * MaxFrameTime);

        Assert.AreSame(brick, result.HitCollider);
        Assert.LessOrEqual(result.Position.y + LaserSize.y * 0.5f, brick.bounds.min.y + Tolerance);
    }

    [Test]
    public void Move_IntoTopWall_ReportsWall()
    {
        BoxCollider wall = CreateBox("Wall", new Vector2(0f, 9.32f), new Vector2(14f, 0.5f));

        LaserMoveResult result = mover.Move(new Vector3(0f, 8.5f, 0f), 2f);

        Assert.AreSame(wall, result.HitCollider);
    }

    [Test]
    public void Move_PaddleLayer_IsIgnored()
    {
        CreateBox("Paddle", new Vector2(0f, 1f), new Vector2(2f, 0.5f));

        LaserMoveResult result = mover.Move(new Vector3(0f, 0f, 0f), 2f);

        Assert.IsNull(result.HitCollider);
        Assert.AreEqual(2f, result.Position.y, Tolerance);
    }

    [Test]
    public void Move_DisabledBrickCollider_PassesThrough()
    {
        BoxCollider brick = CreateBox("Brick", new Vector2(0f, 2f), new Vector2(1f, 0.5f));
        brick.enabled = false;

        LaserMoveResult result = mover.Move(new Vector3(0f, 0f, 0f), 4f);

        Assert.IsNull(result.HitCollider);
    }

    [Test]
    public void Move_BesideBrick_Misses()
    {
        CreateBox("Brick", new Vector2(0f, 2f), new Vector2(1f, 0.5f));

        LaserMoveResult result = mover.Move(new Vector3(0.6f, 0f, 0f), 4f);

        Assert.IsNull(result.HitCollider);
    }
}
