using NUnit.Framework;
using UnityEngine;

public class CatchHoldTests
{
    private const float Tolerance = 0.0001f;

    [Test]
    public void Start_KeepsOffsetFromPaddleCenter()
    {
        CatchHold hold = new CatchHold();

        hold.Start(2.5f, 2f, 1f, 2f);

        Assert.IsTrue(hold.IsHolding);
        Assert.AreEqual(0.5f, hold.OffsetX, Tolerance);
        Assert.AreEqual(3.5f, hold.HeldX(3f, 1f), Tolerance);
    }

    [Test]
    public void Start_SideHit_ClampsOffsetToPaddleEdge()
    {
        CatchHold hold = new CatchHold();

        hold.Start(1.3f, 0f, 1f, 2f);

        Assert.AreEqual(1f, hold.OffsetX, Tolerance);
    }

    [Test]
    public void HeldX_NarrowerPaddle_ClampsToCurrentHalfWidth()
    {
        CatchHold hold = new CatchHold();
        hold.Start(1.4f, 0f, 1.5f, 2f);

        Assert.AreEqual(1.4f, hold.HeldX(0f, 1.5f), Tolerance);
        Assert.AreEqual(1f, hold.HeldX(0f, 1f), Tolerance);
    }

    [Test]
    public void Tick_ReleasesAutomaticallyAfterTwoSeconds()
    {
        ItemConfig config = ScriptableObject.CreateInstance<ItemConfig>();
        CatchHold hold = new CatchHold();
        hold.Start(0f, 0f, 1f, config.catchAutoReleaseSeconds);

        Assert.IsFalse(hold.Tick(1.9f));
        Assert.IsTrue(hold.Tick(0.1f));
        Assert.AreEqual(2f, config.catchAutoReleaseSeconds, Tolerance);
        Object.DestroyImmediate(config);
    }

    [Test]
    public void Tick_NotHolding_NeverReleases()
    {
        CatchHold hold = new CatchHold();

        Assert.IsFalse(hold.Tick(10f));

        hold.Start(0f, 0f, 1f, 2f);
        hold.Stop();
        Assert.IsFalse(hold.IsHolding);
        Assert.IsFalse(hold.Tick(10f));
    }

    [Test]
    public void Start_AgainAfterRelease_ResetsTimer()
    {
        CatchHold hold = new CatchHold();
        hold.Start(0f, 0f, 1f, 2f);
        hold.Tick(1.5f);
        hold.Stop();

        hold.Start(0f, 0f, 1f, 2f);

        Assert.AreEqual(0f, hold.ElapsedSeconds, Tolerance);
        Assert.IsFalse(hold.Tick(1.5f));
    }

    [Test]
    public void Release_AngleFromHeldOffsetMatchesPaddleFormula()
    {
        CatchHold hold = new CatchHold();
        hold.Start(0.5f, 0f, 1f, 2f);
        float heldX = hold.HeldX(0f, 1f);

        float hitOffset = BallBounce.PaddleHitOffset(heldX, 0f, 2f);
        Vector2 launchVelocity = BallBounce.BounceOffPaddle(4f, hitOffset, 60f);

        Assert.AreEqual(0.5f, hitOffset, Tolerance);
        Assert.AreEqual(4f, launchVelocity.y, Tolerance);
        Assert.AreEqual(4f * Mathf.Tan(30f * Mathf.Deg2Rad), launchVelocity.x, Tolerance);
    }
}
