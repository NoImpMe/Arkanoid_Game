using NUnit.Framework;
using UnityEngine;

public class SpriteFittingTests
{
    [Test]
    public void ScaleToFit_ScalesEachAxisToTargetSize()
    {
        Vector3 scale = SpriteFitting.ScaleToFit(new Vector2(2.06f, 0.65f), new Vector2(2f, 0.5f));

        Assert.AreEqual(2f / 2.06f, scale.x, 0.0001f);
        Assert.AreEqual(0.5f / 0.65f, scale.y, 0.0001f);
        Assert.AreEqual(1f, scale.z);
    }
}
