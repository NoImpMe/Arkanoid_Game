using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private ItemConfig itemConfig;
    [SerializeField] private Paddle paddle;
    [SerializeField] private Ball ball;
    [SerializeField] private BrickField brickField;
    [SerializeField] private Hud hud;
    [SerializeField] private ItemController itemController;

    private GameSession session;
    private HighScoreStore highScoreStore;
    private PaddleMode paddleMode;

    public GameState State { get; private set; }
    public int Score => session.Score;
    public int Lives => session.Lives;

    private bool HasMultipleBalls => false;

    private void OnEnable()
    {
        ball.BrickDestroyed += HandleBrickDestroyed;
    }

    private void OnDisable()
    {
        ball.BrickDestroyed -= HandleBrickDestroyed;
    }

    private void Start()
    {
        highScoreStore = HighScoreStore.CreateInPersistentData(gameConfig.highScoreFileName);
        int savedHighScore = highScoreStore.Load(out HighScoreLoadStatus loadStatus);
        if (loadStatus == HighScoreLoadStatus.ResetAfterCorruption)
        {
            Debug.LogWarning($"High score file was unreadable and has been reset: {highScoreStore.FilePath}");
        }

        session = new GameSession(gameConfig.startingLives, savedHighScore);
        hud.CreateLifeIcons(itemConfig.maxReserveLives);
        RefreshHud();
        EnterReady();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        switch (State)
        {
            case GameState.Ready:
                paddle.Step(deltaTime);
                ball.PlaceAt(paddle.BallRestPosition(ball.Radius));
                if (IsKeyPressed(Key.Space))
                {
                    ball.Launch();
                    hud.HideMessage();
                    State = GameState.Playing;
                }
                break;
            case GameState.Playing:
                paddle.Step(deltaTime);
                if (ball.IsHeld)
                {
                    UpdateHeldBall(deltaTime);
                }
                else
                {
                    BallMoveResult moveResult = ball.Step(deltaTime, paddleMode == PaddleMode.Catch);
                    if (State != GameState.Playing)
                    {
                        break;
                    }
                    if (moveResult.ReachedDeadZone)
                    {
                        HandleMiss();
                        break;
                    }
                    if (moveResult.CaughtByPaddle)
                    {
                        ball.StartHold(paddle, itemConfig.catchAutoReleaseSeconds);
                    }
                }
                if (itemController.Step(deltaTime, paddle.Area, out ItemType caughtType))
                {
                    ApplyItem(caughtType);
                }
                break;
            case GameState.Clear:
            case GameState.GameOver:
                if (IsKeyPressed(Key.R))
                {
                    RestartScene();
                }
                break;
        }
    }

    private void UpdateHeldBall(float deltaTime)
    {
        bool autoRelease = ball.TickHold(paddle, deltaTime);
        if (autoRelease || IsKeyPressed(Key.Space))
        {
            ball.ReleaseFromPaddle(paddle);
        }
    }

    private void HandleBrickDestroyed(Brick brick)
    {
        session.AddScore(brick.Score);
        brickField.NotifyBrickDestroyed();
        RefreshHud();

        if (GameSession.IsStageCleared(brickField.RemainingCount))
        {
            EnterEnd(GameState.Clear, gameConfig.clearMessage);
            return;
        }

        itemController.TryDrop(brick.transform.position, brick.CanDropItem, HasMultipleBalls);
    }

    private void ApplyItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Player:
                session.AddLife(itemConfig.maxReserveLives);
                RefreshHud();
                break;
            case ItemType.Slow:
                ball.SlowDown(itemConfig.slowVerticalSpeed);
                break;
        }

        SetPaddleMode(PaddleModeRule.AfterPickup(paddleMode, itemType));
    }

    private void SetPaddleMode(PaddleMode mode)
    {
        paddleMode = mode;
        paddle.SetWidth(PaddleModeRule.WidthFor(mode, paddle.NormalWidth, itemConfig.enlargeWidthMultiplier));
    }

    private void HandleMiss()
    {
        itemController.ClearAll();
        SetPaddleMode(PaddleMode.None);
        session.LoseLife();
        RefreshHud();

        if (session.IsGameOver)
        {
            EnterEnd(GameState.GameOver, gameConfig.gameOverMessage);
            return;
        }
        EnterReady();
    }

    private void EnterReady()
    {
        State = GameState.Ready;
        ball.PlaceAt(paddle.BallRestPosition(ball.Radius));
        hud.ShowMessage(gameConfig.readyMessage);
    }

    private void EnterEnd(GameState endState, string message)
    {
        State = endState;
        ball.PlaceAt(ball.transform.position);
        hud.ShowMessage($"{message}\n\n{gameConfig.restartHint}");
        LayoutSession.MarkGameFinished();
        SaveHighScore();
    }

    private void OnApplicationQuit()
    {
        SaveHighScore();
    }

    private void SaveHighScore()
    {
        if (session == null || highScoreStore == null)
        {
            return;
        }

        if (!highScoreStore.TrySave(session.HighScore))
        {
            Debug.LogWarning($"Failed to save high score: {highScoreStore.FilePath}");
        }
    }

    private void RefreshHud()
    {
        hud.SetScore(session.Score);
        hud.SetHighScore(session.HighScore);
        hud.SetReserveLives(session.ReserveLives);
    }

    private static void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private static bool IsKeyPressed(Key key)
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard[key].wasPressedThisFrame;
    }
}
