using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private Paddle paddle;
    [SerializeField] private Ball ball;
    [SerializeField] private BrickField brickField;
    [SerializeField] private Hud hud;

    private GameSession session;

    public GameState State { get; private set; }
    public int Score => session.Score;
    public int Lives => session.Lives;

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
        session = new GameSession(gameConfig.startingLives, SessionHighScore.Get(gameConfig.initialHighScore));
        hud.CreateLifeIcons(session.ReserveLives);
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
                bool reachedDeadZone = ball.Step(deltaTime);
                if (reachedDeadZone && State == GameState.Playing)
                {
                    HandleMiss();
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

    private void HandleBrickDestroyed(Brick brick)
    {
        session.AddScore(brick.Score);
        SessionHighScore.Submit(session.HighScore);
        brickField.NotifyBrickDestroyed();
        RefreshHud();

        if (GameSession.IsStageCleared(brickField.RemainingCount))
        {
            EnterEnd(GameState.Clear, gameConfig.clearMessage);
        }
    }

    private void HandleMiss()
    {
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
