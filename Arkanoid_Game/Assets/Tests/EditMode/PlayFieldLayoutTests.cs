using NUnit.Framework;
using UnityEngine;

public class PlayFieldLayoutTests
{
    private const float Tolerance = 0.0001f;

    private PlayFieldLayout CreateSpecLayout()
    {
        return new PlayFieldLayout(-6.5f, 6.5f, 8.1f, -9.6f, 0.5f);
    }

    [Test]
    public void SideWalls_InnerFacesMatchPlayAreaAndCentersAtSpecX()
    {
        PlayFieldLayout layout = CreateSpecLayout();

        Assert.AreEqual(-6.5f, layout.LeftWall.xMax, Tolerance);
        Assert.AreEqual(6.5f, layout.RightWall.xMin, Tolerance);
        Assert.AreEqual(-6.75f, layout.LeftWall.center.x, Tolerance);
        Assert.AreEqual(6.75f, layout.RightWall.center.x, Tolerance);
        Assert.AreEqual(0.5f, layout.LeftWall.width, Tolerance);
    }

    [Test]
    public void TopWall_CenterAtSpecYAndSpansBothSideWalls()
    {
        PlayFieldLayout layout = CreateSpecLayout();

        Assert.AreEqual(8.1f, layout.TopWall.yMin, Tolerance);
        Assert.AreEqual(8.35f, layout.TopWall.center.y, Tolerance);
        Assert.AreEqual(layout.LeftWall.xMin, layout.TopWall.xMin, Tolerance);
        Assert.AreEqual(layout.RightWall.xMax, layout.TopWall.xMax, Tolerance);
    }

    [Test]
    public void Corners_HaveNoGapBetweenWalls()
    {
        PlayFieldLayout layout = CreateSpecLayout();

        Assert.AreEqual(layout.TopWall.yMax, layout.LeftWall.yMax, Tolerance);
        Assert.AreEqual(layout.TopWall.yMax, layout.RightWall.yMax, Tolerance);
        Assert.AreEqual(layout.DeadZone.yMin, layout.LeftWall.yMin, Tolerance);
        Assert.AreEqual(layout.DeadZone.yMin, layout.RightWall.yMin, Tolerance);
    }

    [Test]
    public void DeadZone_TopFaceAtDeadZoneYAndFillsPlayAreaWidth()
    {
        PlayFieldLayout layout = CreateSpecLayout();

        Assert.AreEqual(-9.6f, layout.DeadZone.yMax, Tolerance);
        Assert.AreEqual(-6.5f, layout.DeadZone.xMin, Tolerance);
        Assert.AreEqual(6.5f, layout.DeadZone.xMax, Tolerance);
    }

    [Test]
    public void FromConfig_DefaultStageConfigMatchesSpecLayout()
    {
        StageConfig config = ScriptableObject.CreateInstance<StageConfig>();
        PlayFieldLayout fromConfig = PlayFieldLayout.FromConfig(config);
        PlayFieldLayout expected = CreateSpecLayout();
        Object.DestroyImmediate(config);

        Assert.AreEqual(expected.LeftWall, fromConfig.LeftWall);
        Assert.AreEqual(expected.RightWall, fromConfig.RightWall);
        Assert.AreEqual(expected.TopWall, fromConfig.TopWall);
        Assert.AreEqual(expected.DeadZone, fromConfig.DeadZone);
    }
}
