using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Arkanoid/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Rules")]
    public int startingLives = 3;

    [Header("High Score Save")]
    public string highScoreFileName = "highscore.dat";

    [Header("Messages")]
    public string readyMessage = "PRESS SPACE";
    public string clearMessage = "ROUND CLEAR";
    public string gameOverMessage = "GAME OVER";
    public string restartHint = "PRESS R TO RESTART";
}
