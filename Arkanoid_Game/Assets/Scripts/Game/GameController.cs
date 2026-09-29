using System;
using System.Collections.Generic;
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
    [SerializeField] private LaserController laserController;

    private readonly List<Ball> balls = new List<Ball>();
    private readonly EffectTimer paddleModeTimer = new EffectTimer();
    private readonly EffectTimer slowTimer = new EffectTimer();
    private Action<Brick> brickDestroyedHandler;
    private GameSession session;
    private HighScoreStore highScoreStore;
    private PaddleMode paddleMode;

    public GameState State { get; private set; }
    public int Score => session.Score;
    public int Lives => session.Lives;
    public int ActiveBallCount => CountActiveBalls();

    private void Awake()
    {
        brickDestroyedHandler = HandleBrickDestroyed;
        balls.Add(ball);
        for (int index = 1; index < itemConfig.disruptionBallCount; index++)
        {
            Ball extraBall = Instantiate(ball, ball.transform.parent);
            extraBall.name = $"{ball.name} ({index})";
            extraBall.gameObject.SetActive(false);
            balls.Add(extraBall);
        }
    }

    private void OnEnable()
    {
        foreach (Ball eachBall in balls)
        {
            eachBall.BrickDestroyed += HandleBrickDestroyed;
        }
    }

    private void OnDisable()
    {
        foreach (Ball eachBall in balls)
        {
            eachBall.BrickDestroyed -= HandleBrickDestroyed;
        }
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
                if (paddleModeTimer.Tick(deltaTime))
                {
                    SetPaddleMode(PaddleMode.None);
                }
                if (slowTimer.Tick(deltaTime))
                {
                    RestoreBallsFromSlow();
                }
                if (LaserMath.IsAutoFireMode(paddleMode))
                {
                    laserController.TryFire(paddle);
                }
                StepBalls(deltaTime, IsKeyPressed(Key.Space));
                if (State != GameState.Playing)
                {
                    break;
                }
                laserController.Step(deltaTime, brickDestroyedHandler);
                if (State != GameState.Playing)
                {
                    break;
                }
                if (DisruptionSplit.ShouldLoseLife(ActiveBallCount))
                {
                    HandleMiss();
                    break;
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

    private void StepBalls(float deltaTime, bool releasePressed)
    {
        foreach (Ball eachBall in balls)
        {
            if (!eachBall.gameObject.activeSelf)
            {
                continue;
            }

            if (eachBall.IsHeld)
            {
                bool autoRelease = eachBall.TickHold(paddle, deltaTime);
                if (autoRelease || releasePressed)
                {
                    eachBall.ReleaseFromPaddle(paddle);
                }
                continue;
            }

            BallMoveResult moveResult = eachBall.Step(deltaTime, paddleMode == PaddleMode.Catch);
            if (State != GameState.Playing)
            {
                return;
            }
            if (moveResult.ReachedDeadZone)
            {
                eachBall.gameObject.SetActive(false);
            }
            else if (moveResult.CaughtByPaddle)
            {
                eachBall.StartHold(paddle, itemConfig.catchAutoReleaseSeconds);
                SetPaddleMode(PaddleModeRule.AfterBallCaught(paddleMode));
            }
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

        itemController.TryDrop(brick.transform.position, brick.CanDropItem, DisruptionSplit.HasMultipleBalls(ActiveBallCount));
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
                foreach (Ball eachBall in balls)
                {
                    if (eachBall.gameObject.activeSelf)
                    {
                        eachBall.SlowDown(itemConfig.slowVerticalSpeed);
                    }
                }
                slowTimer.Start(itemConfig.slowDurationSeconds);
                break;
            case ItemType.Disruption:
                SplitBall();
                break;
        }

        SetPaddleMode(PaddleModeRule.AfterPickup(paddleMode, itemType));
        if (itemType == ItemType.Lasers || itemType == ItemType.Enlarge)
        {
            paddleModeTimer.Start(PaddleModeRule.DurationFor(paddleMode, itemConfig.laserDurationSeconds, itemConfig.enlargeDurationSeconds));
        }
    }

    private void RestoreBallsFromSlow()
    {
        foreach (Ball eachBall in balls)
        {
            if (eachBall.gameObject.activeSelf)
            {
                eachBall.RestoreFromSlow();
            }
        }
    }

    private void SplitBall()
    {
        Ball sourceBall = FindActiveBall();
        if (sourceBall == null)
        {
            return;
        }

        if (sourceBall.IsHeld)
        {
            sourceBall.ReleaseFromPaddle(paddle);
        }

        float spread = itemConfig.disruptionSpreadDegrees;
        LaunchExtraBall(sourceBall, DisruptionSplit.Rotated(sourceBall.Velocity, -spread, sourceBall.MaxBounceAngleDegrees));
        LaunchExtraBall(sourceBall, DisruptionSplit.Rotated(sourceBall.Velocity, spread, sourceBall.MaxBounceAngleDegrees));
    }

    private void LaunchExtraBall(Ball sourceBall, Vector2 velocity)
    {
        foreach (Ball eachBall in balls)
        {
            if (!eachBall.gameObject.activeSelf)
            {
                eachBall.gameObject.SetActive(true);
                eachBall.LaunchFrom(sourceBall.transform.position, sourceBall.VerticalSpeed, sourceBall.SlowReduction, velocity);
                return;
            }
        }
    }

    private Ball FindActiveBall()
    {
        foreach (Ball eachBall in balls)
        {
            if (eachBall.gameObject.activeSelf)
            {
                return eachBall;
            }
        }
        return null;
    }

    private int CountActiveBalls()
    {
        int count = 0;
        foreach (Ball eachBall in balls)
        {
            if (eachBall.gameObject.activeSelf)
            {
                count++;
            }
        }
        return count;
    }

    private void SetPaddleMode(PaddleMode mode)
    {
        paddleMode = mode;
        if (!PaddleModeRule.HasDuration(mode))
        {
            paddleModeTimer.Stop();
        }
        paddle.SetWidth(PaddleModeRule.WidthFor(mode, paddle.NormalWidth, itemConfig.enlargeWidthMultiplier));
        paddle.SetTint(mode == PaddleMode.Laser ? itemConfig.laserPaddleTint : Color.white);
    }

    private void HandleMiss()
    {
        itemController.ClearAll();
        laserController.ClearAll();
        SetPaddleMode(PaddleMode.None);
        slowTimer.Stop();
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
        foreach (Ball eachBall in balls)
        {
            eachBall.gameObject.SetActive(eachBall == ball);
        }
        ball.PlaceAt(paddle.BallRestPosition(ball.Radius));
        hud.ShowMessage(gameConfig.readyMessage);
    }

    private void EnterEnd(GameState endState, string message)
    {
        State = endState;
        foreach (Ball eachBall in balls)
        {
            eachBall.PlaceAt(eachBall.transform.position);
        }
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
