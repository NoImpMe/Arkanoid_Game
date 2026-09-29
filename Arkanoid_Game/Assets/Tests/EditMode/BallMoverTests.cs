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
            LayerMask.GetMask("Paddle"), LayerMask.GetMask("Brick"), LayerMask.GetMask("DeadZone"));
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
    public void Move_VerticalBallGrazingTopCornerOfUnbreakableBrick_BouncesUpInsteadOfSticking()
    {
        CreateBox("Brick", Vector2.zero, new Vector2(1f, 0.5f));
        Vector3 position = new Vector3(0.608f, 1.5f, 0f);
        Vector2 velocity = new Vector2(0f, -6f);
        int framesTouchingBrick = 0;

        for (int frame = 0; frame < 60; frame++)
        {
            int hitsThisFrame = 0;
            BallMoveResult result = mover.Move(position, velocity, 1f / 60f, _ => hitsThisFrame++);
            position = result.Position;
            velocity = result.Velocity;
            if (hitsThisFrame > 0)
            {
                framesTouchingBrick++;
            }
        }

        Assert.LessOrEqual(framesTouchingBrick, 1);
        Assert.AreEqual(6f, velocity.y, Tolerance);
        Assert.Greater(position.y, 1f);
    }

    [TestCase(0f)]
    [TestCase(0.3f)]
    [TestCase(1f)]
    [TestCase(3.464f)]
    public void Move_BallFallingAcrossBrickTopCorner_NeverTouchesForMoreThanOneFrame(float horizontalSpeed)
    {
        CreateBox("Brick", Vector2.zero, new Vector2(1f, 0.5f));

        for (float offset = 0f; offset <= 0.16f; offset += 0.002f)
        {
            Vector3 position = new Vector3(0.5f + offset, 1.5f, 0f);
            Vector2 velocity = new Vector2(horizontalSpeed, -6f);
            int framesTouchingBrick = 0;

            for (int frame = 0; frame < 120; frame++)
            {
                int hitsThisFrame = 0;
                BallMoveResult result = mover.Move(position, velocity, 1f / 60f, _ => hitsThisFrame++);
                position = result.Position;
                velocity = result.Velocity;
                if (hitsThisFrame > 0)
                {
                    framesTouchingBrick++;
                }
            }

            Assert.LessOrEqual(framesTouchingBrick, 1, $"offset {offset}");
            Assert.AreEqual(6f, Mathf.Abs(velocity.y), Tolerance, $"offset {offset}");
        }
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
    public void Move_IntoBrick_ReportsThatBrickOnce()
    {
        BoxCollider brick = CreateBox("Brick", new Vector2(0f, 2f), new Vector2(1f, 0.5f));
        List<Collider> hitBricks = new List<Collider>();

        mover.Move(Vector3.zero, new Vector2(0f, 6f), 0.5f, hitBricks.Add);

        Assert.AreEqual(1, hitBricks.Count);
        Assert.AreSame(brick, hitBricks[0]);
    }

    [Test]
    public void Move_OnBoundaryOfTwoAdjacentBricks_ReportsOnlyOneBrick()
    {
        CreateBox("Brick", new Vector2(-0.5f, 2f), new Vector2(1f, 0.5f));
        CreateBox("Brick", new Vector2(0.5f, 2f), new Vector2(1f, 0.5f));
        List<Collider> hitBricks = new List<Collider>();

        BallMoveResult result = mover.Move(Vector3.zero, new Vector2(0f, 6f), 0.5f, hitBricks.Add);

        Assert.AreEqual(1, hitBricks.Count);
        Assert.Less(result.Velocity.y, 0f);
    }

    [Test]
    public void Move_WallHit_DoesNotReportBrick()
    {
        CreateBox("Wall", new Vector2(0f, 2f), new Vector2(4f, 0.5f));
        List<Collider> hitBricks = new List<Collider>();

        mover.Move(Vector3.zero, new Vector2(0f, 6f), 0.5f, hitBricks.Add);

        Assert.AreEqual(0, hitBricks.Count);
    }

    [Test]
    public void Move_AfterBrickColliderDisabled_PassesThroughWithoutSync()
    {
        BoxCollider brick = CreateBox("Brick", new Vector2(0f, 2f), new Vector2(1f, 0.5f));
        mover.Move(Vector3.zero, new Vector2(0f, 6f), 0.5f, hitCollider => hitCollider.enabled = false);

        BallMoveResult secondPass = mover.Move(Vector3.zero, new Vector2(0f, 6f), 0.5f);

        Assert.IsFalse(brick.enabled);
        Assert.AreEqual(3f, secondPass.Position.y, Tolerance);
        Assert.AreEqual(6f, secondPass.Velocity.y, Tolerance);
    }

    [Test]
    public void Move_IntoDeadZone_ReportsReachedDeadZone()
    {
        CreateBox("DeadZone", new Vector2(0f, -9.85f), new Vector2(13f, 0.5f));

        BallMoveResult result = mover.Move(new Vector3(0f, -9f, 0f), new Vector2(0f, -6f), 0.5f);

        Assert.IsTrue(result.ReachedDeadZone);
    }

    [Test]
    public void Move_OntoEnlargedPaddle_OffsetUsesCurrentWidth()
    {
        CreateBox("Paddle", new Vector2(0f, -8.35f), new Vector2(3f, 0.5f));

        BallMoveResult result = mover.Move(new Vector3(1f, -7f, 0f), new Vector2(0f, -6f), 0.5f);

        float expectedAngle = (1f / 1.5f) * MaxBounceAngle;
        Assert.AreEqual(6f, result.Velocity.y, Tolerance);
        Assert.AreEqual(6f * Mathf.Tan(expectedAngle * Mathf.Deg2Rad), result.Velocity.x, Tolerance);
    }

    [Test]
    public void Move_CatchOntoPaddle_StopsAtContactAndReportsCaught()
    {
        CreateBox("Paddle", new Vector2(0f, -8.35f), new Vector2(2f, 0.5f));

        BallMoveResult result = mover.Move(new Vector3(0.6f, -7f, 0f), new Vector2(0f, -6f), 0.5f, null, true);

        Assert.IsTrue(result.CaughtByPaddle);
        Assert.IsFalse(result.ReachedDeadZone);
        Assert.AreEqual(0.6f, result.Position.x, Tolerance);
        Assert.GreaterOrEqual(result.Position.y - Radius, -8.1f);
        Assert.Less(result.Position.y - Radius, -8.1f + Skin * 2f);
    }

    [Test]
    public void Move_CatchDisabled_StillBouncesOffPaddle()
    {
        CreateBox("Paddle", new Vector2(0f, -8.35f), new Vector2(2f, 0.5f));

        BallMoveResult result = mover.Move(new Vector3(0.6f, -7f, 0f), new Vector2(0f, -6f), 0.5f, null, false);

        Assert.IsFalse(result.CaughtByPaddle);
        Assert.Greater(result.Velocity.y, 0f);
    }

    [Test]
    public void Move_CatchUpwardThroughPaddle_IgnoresPaddle()
    {
        CreateBox("Paddle", new Vector2(0f, -8.35f), new Vector2(2f, 0.5f));

        BallMoveResult result = mover.Move(new Vector3(0f, -9.2f, 0f), new Vector2(0f, 6f), 0.5f, null, true);

        Assert.IsFalse(result.CaughtByPaddle);
        Assert.AreEqual(-6.2f, result.Position.y, Tolerance);
    }

    [Test]
    public void Move_CatchPaddleOverlapsFallingBall_CaughtAboveTheTop()
    {
        CreateBox("Paddle", new Vector2(0f, -8.35f), new Vector2(2f, 0.5f));

        BallMoveResult result = mover.Move(new Vector3(0.95f, -8.2f, 0f), new Vector2(0f, -6f), 0.016f, null, true);

        Assert.IsTrue(result.CaughtByPaddle);
        Assert.GreaterOrEqual(result.Position.y - Radius, -8.1f);
    }

    [Test]
    public void Move_CatchMode_WallAndBrickStillReflect()
    {
        BoxCollider brick = CreateBox("Brick", new Vector2(0f, 2f), new Vector2(1f, 0.5f));
        List<Collider> hitBricks = new List<Collider>();

        BallMoveResult result = mover.Move(Vector3.zero, new Vector2(0f, 6f), 0.5f, hitBricks.Add, true);

        Assert.IsFalse(result.CaughtByPaddle);
        Assert.AreEqual(1, hitBricks.Count);
        Assert.AreSame(brick, hitBricks[0]);
        Assert.Less(result.Velocity.y, 0f);
    }

    [Test]
    public void Move_TwoBallsSameBrickSameFrame_OnlyFirstBallReportsIt()
    {
        CreateBox("Brick", new Vector2(0f, 2f), new Vector2(1f, 0.5f));
        List<Collider> firstBallHits = new List<Collider>();
        List<Collider> secondBallHits = new List<Collider>();

        mover.Move(Vector3.zero, new Vector2(0f, 6f), 0.5f, hitBrick =>
        {
            firstBallHits.Add(hitBrick);
            hitBrick.enabled = false;
        });
        BallMoveResult secondResult = mover.Move(new Vector3(0.2f, 0f, 0f), new Vector2(0f, 6f), 0.5f, secondBallHits.Add);

        Assert.AreEqual(1, firstBallHits.Count);
        Assert.AreEqual(0, secondBallHits.Count);
        Assert.AreEqual(6f, secondResult.Velocity.y, Tolerance);
    }

    [Test]
    public void Move_CatchAtMaxSpeed_DoesNotTunnelThroughPaddle()
    {
        CreateBox("Paddle", new Vector2(0f, -8.35f), new Vector2(2f, 0.5f));

        BallMoveResult result = mover.Move(new Vector3(0f, -4f, 0f), new Vector2(0f, -18f), MaxFrameTime, null, true);

        Assert.IsTrue(result.CaughtByPaddle);
        Assert.GreaterOrEqual(result.Position.y - Radius, -8.1f);
    }
}
