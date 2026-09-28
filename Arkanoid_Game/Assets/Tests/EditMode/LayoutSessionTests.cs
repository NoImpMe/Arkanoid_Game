using NUnit.Framework;

public class LayoutSessionTests
{
    [SetUp]
    public void SetUp()
    {
        LayoutSession.Reset();
    }

    [TearDown]
    public void TearDown()
    {
        LayoutSession.Reset();
    }

    [Test]
    public void NewSession_HasNotFinishedGame()
    {
        Assert.IsFalse(LayoutSession.HasFinishedGame);
    }

    [Test]
    public void MarkGameFinished_StaysFinishedAcrossMultipleGames()
    {
        LayoutSession.MarkGameFinished();
        LayoutSession.MarkGameFinished();

        Assert.IsTrue(LayoutSession.HasFinishedGame);
    }

    [Test]
    public void Reset_ReturnsToFirstGameState()
    {
        LayoutSession.MarkGameFinished();

        LayoutSession.Reset();

        Assert.IsFalse(LayoutSession.HasFinishedGame);
    }
}
