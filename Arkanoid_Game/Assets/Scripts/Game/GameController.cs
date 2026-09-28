using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    [SerializeField] private Paddle paddle;
    [SerializeField] private Ball ball;
    [SerializeField] private BrickField brickField;

    public GameState State { get; private set; }
    public int Score { get; private set; }

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
        EnterReady();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        paddle.Step(deltaTime);

        switch (State)
        {
            case GameState.Ready:
                ball.PlaceAt(paddle.BallRestPosition(ball.Radius));
                if (LaunchPressed())
                {
                    ball.Launch();
                    State = GameState.Playing;
                }
                break;
            case GameState.Playing:
                if (ball.Step(deltaTime))
                {
                    EnterReady();
                }
                break;
        }
    }

    private void HandleBrickDestroyed(Brick brick)
    {
        Score += brick.Score;
        brickField.NotifyBrickDestroyed();
    }

    private void EnterReady()
    {
        State = GameState.Ready;
        ball.PlaceAt(paddle.BallRestPosition(ball.Radius));
    }

    private static bool LaunchPressed()
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard.spaceKey.wasPressedThisFrame;
    }
}
