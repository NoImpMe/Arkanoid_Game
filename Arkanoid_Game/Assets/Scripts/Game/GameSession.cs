using UnityEngine;

public class GameSession
{
    public int Lives { get; private set; }
    public int Score { get; private set; }
    public int HighScore { get; private set; }

    public int ReserveLives => Mathf.Max(Lives - 1, 0);
    public bool IsGameOver => Lives <= 0;

    public GameSession(int startingLives, int highScore)
    {
        Lives = startingLives;
        HighScore = highScore;
    }

    public void AddScore(int points)
    {
        Score += points;
        HighScore = Mathf.Max(HighScore, Score);
    }

    public void AddLife(int maxReserveLives)
    {
        if (ReserveLives < maxReserveLives)
        {
            Lives++;
        }
    }

    public void LoseLife()
    {
        Lives = Mathf.Max(Lives - 1, 0);
    }

    public static bool IsStageCleared(int remainingBricks)
    {
        return remainingBricks <= 0;
    }
}
