using NUnit.Framework;
using UnityEngine;

public class EffectTimerTests
{
    private const float Tolerance = 0.0001f;

    [Test]
    public void Tick_ExpiresOnlyAfterFullDuration()
    {
        EffectTimer timer = new EffectTimer();
        timer.Start(5f);

        Assert.IsFalse(timer.Tick(4.9f));
        Assert.IsTrue(timer.IsRunning);
        Assert.AreEqual(0.1f, timer.RemainingSeconds, Tolerance);

        Assert.IsTrue(timer.Tick(0.1f));
        Assert.IsFalse(timer.IsRunning);
    }

    [Test]
    public void Tick_AfterExpired_DoesNotReportAgain()
    {
        EffectTimer timer = new EffectTimer();
        timer.Start(5f);
        timer.Tick(6f);

        Assert.IsFalse(timer.Tick(1f));
    }

    [Test]
    public void Tick_NotStarted_NeverExpires()
    {
        EffectTimer timer = new EffectTimer();

        Assert.IsFalse(timer.Tick(10f));
        Assert.IsFalse(timer.IsRunning);
    }

    [Test]
    public void Start_AgainWhileRunning_ResetsToFullDuration()
    {
        EffectTimer timer = new EffectTimer();
        timer.Start(5f);
        timer.Tick(4f);

        timer.Start(5f);

        Assert.AreEqual(5f, timer.RemainingSeconds, Tolerance);
        Assert.IsFalse(timer.Tick(4f));
        Assert.IsTrue(timer.Tick(1f));
    }

    [Test]
    public void Stop_WhileRunning_PreventsExpiry()
    {
        EffectTimer timer = new EffectTimer();
        timer.Start(5f);

        timer.Stop();

        Assert.IsFalse(timer.IsRunning);
        Assert.IsFalse(timer.Tick(10f));
    }

    [Test]
    public void DefaultConfig_LaserEnlargeAndSlowDurationsAreFiveSeconds()
    {
        ItemConfig config = ScriptableObject.CreateInstance<ItemConfig>();

        Assert.AreEqual(5f, config.laserDurationSeconds, Tolerance);
        Assert.AreEqual(5f, config.enlargeDurationSeconds, Tolerance);
        Assert.AreEqual(5f, config.slowDurationSeconds, Tolerance);
        Object.DestroyImmediate(config);
    }
}
