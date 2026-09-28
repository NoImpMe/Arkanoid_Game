using NUnit.Framework;

public class SessionHighScoreTests
{
    [SetUp]
    public void SetUp()
    {
        SessionHighScore.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        SessionHighScore.Clear();
    }

    [Test]
    public void Get_WithoutSubmit_ReturnsInitialHighScore()
    {
        Assert.AreEqual(50000, SessionHighScore.Get(50000));
    }

    [Test]
    public void Submit_KeepsValueAcrossRestart()
    {
        SessionHighScore.Submit(60000);

        Assert.AreEqual(60000, SessionHighScore.Get(50000));
    }

    [Test]
    public void Submit_LowerValue_DoesNotReduceHighScore()
    {
        SessionHighScore.Submit(60000);
        SessionHighScore.Submit(50000);

        Assert.AreEqual(60000, SessionHighScore.Get(50000));
    }
}
