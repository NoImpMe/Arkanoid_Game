using NUnit.Framework;

public class GameSessionTests
{
    [Test]
    public void NewSession_StartsWithThreeLivesAndTwoReserve()
    {
        GameSession session = new GameSession(3, 50000);

        Assert.AreEqual(3, session.Lives);
        Assert.AreEqual(2, session.ReserveLives);
        Assert.AreEqual(0, session.Score);
        Assert.IsFalse(session.IsGameOver);
    }

    [Test]
    public void LoseLife_ThreeTimes_IsGameOver()
    {
        GameSession session = new GameSession(3, 50000);

        session.LoseLife();
        session.LoseLife();
        Assert.IsFalse(session.IsGameOver);
        Assert.AreEqual(0, session.ReserveLives);

        session.LoseLife();
        Assert.IsTrue(session.IsGameOver);
    }

    [Test]
    public void LoseLife_AfterGameOver_StaysAtZero()
    {
        GameSession session = new GameSession(1, 50000);

        session.LoseLife();
        session.LoseLife();

        Assert.AreEqual(0, session.Lives);
        Assert.AreEqual(0, session.ReserveLives);
    }

    [Test]
    public void AddScore_BelowHighScore_KeepsHighScore()
    {
        GameSession session = new GameSession(3, 50000);

        session.AddScore(120);

        Assert.AreEqual(120, session.Score);
        Assert.AreEqual(50000, session.HighScore);
    }

    [Test]
    public void AddScore_AboveHighScore_RaisesHighScoreTogether()
    {
        GameSession session = new GameSession(3, 100);

        session.AddScore(90);
        session.AddScore(50);

        Assert.AreEqual(140, session.Score);
        Assert.AreEqual(140, session.HighScore);
    }

    [TestCase(0, true)]
    [TestCase(1, false)]
    [TestCase(78, false)]
    public void IsStageCleared_OnlyWhenNoBricksRemain(int remainingBricks, bool expected)
    {
        Assert.AreEqual(expected, GameSession.IsStageCleared(remainingBricks));
    }
}
