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

    [Test]
    public void AddLife_BelowMax_IncreasesLivesAndReserve()
    {
        GameSession session = new GameSession(3, 0);

        session.AddLife(5);

        Assert.AreEqual(4, session.Lives);
        Assert.AreEqual(3, session.ReserveLives);
    }

    [Test]
    public void AddLife_AtMaxReserve_StaysAtMax()
    {
        GameSession session = new GameSession(3, 0);

        for (int pickup = 0; pickup < 10; pickup++)
        {
            session.AddLife(5);
        }

        Assert.AreEqual(6, session.Lives);
        Assert.AreEqual(5, session.ReserveLives);
    }
}
