using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BallMoverTests
{
    private const float Radius = 0.15f;
    private const float Skin = 0.01f;
    private const int MaxIterations = 4;
    private const float MaxBounceAngle = 60f;
    private const float Tolerance = 0.001f;
    private const float MaxFrameTime = 0.3333f;

    private readonly List<GameObject> createdObjects = new List<GameObject>();
    private BallMover mover;

    [SetUp]
    public void SetUp()
    {
        int collisionMask = LayerMask.GetMask("Wall", "Paddle", "Brick", "DeadZone");
        mover = new BallMover(Radius, Skin, MaxIterations, MaxBounceAngle, collisionMask,
            LayerMask.GetMask("Paddle"), LayerMask.GetMask("DeadZone"));
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
    public void Move_NoObstacle_TravelsFullDistanceWithSameVelocity()
    {
        BallMoveResult result = mover.Move(Vector3.zero, new Vector2(3f, 4f), 0.5f);

        Assert.AreEqual(1.5f, result.Position.x, Tolerance);
        Assert.AreEqual(2f, result.Position.y, Tolerance);
        Assert.AreEqual(new Vector2(3f, 4f), result.Velocity);
        Assert.IsFalse(result.ReachedDeadZone);
    }

    [Test]
    public void Move_IntoSideWall_FlipsOnlyVxAndStaysInside()
    {
        BoxCollider wall = CreateBox("Wall", new Vector2(2.25f, 0f), new Vector2(0.5f, 20f));

        BallMoveResult result = mover.Move(Vector3.zero, new Vector2(6f, 6f), 0.5f);

        Assert.Less(result.Velocity.x, 0f);
        Assert.AreEqual(6f, result.Velocity.y, Tolerance);
        Assert.LessOrEqual(result.Position.x + Radius, wall.bounds.min.x);
        Assert.AreEqual(3f, result.Position.y, Tolerance);
    }

    [Test]
    public void Move_MaxSpeedStraightIntoThinBrick_DoesNotTunnel()
    {
        BoxCollider brick = CreateBox("Brick", new Vector2(0f, 3f), new Vector2(1f, 0.5f));

        BallMoveResult result = mover.Move(Vector3.zero, new Vector2(0f, 18f), MaxFrameTime);

        Assert.Less(result.Velocity.y, 0f);
        Assert.LessOrEqual(result.Position.y + Radius, brick.bounds.min.y);
    }

    [Test]
    public void Move_MaxSpeedAngledIntoThinBrick_DoesNotTunnel()
    {
        BoxCollider brick = CreateBox("Brick", new Vector2(0f, 3f), new Vector2(40f, 0.5f));
        Vector2 maxSpeedVelocity = BallBounce.VelocityFromAngle(9f, 60f);
        Assert.AreEqual(18f, maxSpeedVelocity.magnitude, Tolerance);

        BallMoveResult result = mover.Move(Vector3.zero, maxSpeedVelocity, MaxFrameTime);

        Assert.AreEqual(-9f, result.Velocity.y, Tolerance);
        Assert.LessOrEqual(result.Position.y + Radius, brick.bounds.min.y);
    }

    [Test]
    public void Move_IntoBoxCorner_ReflectsAwayAndStaysOutside()
    {
        BoxCollider box = CreateBox("Brick", new Vector2(2f, 2f), new Vector2(1f, 1f));

        BallMoveResult result = mover.Move(Vector3.zero, new Vector2(6f, 6f), 0.5f);

        Assert.AreEqual(6f, Mathf.Abs(result.Velocity.y), Tolerance);
        Assert.AreEqual(6f, Mathf.Abs(result.Velocity.x), Tolerance);
        Assert.IsTrue(result.Velocity.x < 0f || result.Velocity.y < 0f);
        Vector3 closest = box.ClosestPoint(result.Position);
        Assert.GreaterOrEqual(Vector3.Distance(closest, result.Position), Radius);
    }

    [Test]
    public void Move_OntoPaddleCenter_BouncesStraightUp()
    {
        CreateBox("Paddle", new Vector2(0f, -8.35f), new Vector2(2f, 0.5f));

        BallMoveResult result = mover.Move(new Vector3(0f, -7f, 0f), new Vector2(0f, -6f), 0.5f);

        Assert.AreEqual(6f, result.Velocity.y, Tolerance);
        Assert.AreEqual(0f, result.Velocity.x, Tolerance);
        Assert.Greater(result.Position.y, -8.1f);
    }

    [Test]
    public void Move_OntoPaddleRightEdge_BouncesAtMaxAngle()
    {
        CreateBox("Paddle", new Vector2(0f, -8.35f), new Vector2(2f, 0.5f));

        BallMoveResult result = mover.Move(new Vector3(1f, -7f, 0f), new Vector2(0f, -6f), 0.5f);

        Assert.AreEqual(6f, result.Velocity.y, Tolerance);
        Assert.AreEqual(6f * Mathf.Tan(60f * Mathf.Deg2Rad), result.Velocity.x, Tolerance);
    }

    [Test]
    public void Move_UpwardThroughPaddle_IgnoresPaddle()
    {
        CreateBox("Paddle", new Vector2(0f, -8.35f), new Vector2(2f, 0.5f));

        BallMoveResult result = mover.Move(new Vector3(0f, -9.2f, 0f), new Vector2(0f, 6f), 0.5f);

        Assert.AreEqual(6f, result.Velocity.y, Tolerance);
        Assert.AreEqual(-6.2f, result.Position.y, Tolerance);
    }

    [Test]
    public void Move_PaddleOverlapsFallingBall_BouncesUpAboveTheTop()
    {
        CreateBox("Paddle", new Vector2(0f, -8.35f), new Vector2(2f, 0.5f));

        BallMoveResult result = mover.Move(new Vector3(0.95f, -8.2f, 0f), new Vector2(0f, -6f), 0.016f);

        Assert.Greater(result.Velocity.y, 0f);
        Assert.GreaterOrEqual(result.Position.y - Radius, -8.1f);
    }

    [Test]
    public void Move_IntoDeadZone_ReportsReachedDeadZone()
    {
        CreateBox("DeadZone", new Vector2(0f, -9.85f), new Vector2(13f, 0.5f));

        BallMoveResult result = mover.Move(new Vector3(0f, -9f, 0f), new Vector2(0f, -6f), 0.5f);

        Assert.IsTrue(result.ReachedDeadZone);
    }
}
