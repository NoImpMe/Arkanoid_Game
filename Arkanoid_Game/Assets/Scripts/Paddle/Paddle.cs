using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    [SerializeField] private PaddleConfig paddleConfig;
    [SerializeField] private StageConfig stageConfig;
    [SerializeField] private BoxCollider body;
    [SerializeField] private SpriteRenderer visual;

    private Vector2 movementRange;

    public float NormalWidth => paddleConfig.width;
    public float Width { get; private set; }
    public Rect Area => WorldRect.FromCenter(transform.position, body.size);

    private void Awake()
    {
        transform.position = Vector3.up * paddleConfig.positionY;
        body.center = Vector3.zero;
        SetWidth(paddleConfig.width);
    }

    public void SetWidth(float width)
    {
        Width = width;
        body.size = new Vector3(width, paddleConfig.height, stageConfig.colliderDepth);
        visual.transform.localScale = SpriteFitting.ScaleToFit(visual.sprite.bounds.size, new Vector2(width, paddleConfig.height));
        movementRange = PaddleMovement.MovementRange(stageConfig.playAreaLeft, stageConfig.playAreaRight, width);

        Vector3 position = transform.position;
        position.x = PaddleMovement.ClampToRange(position.x, movementRange);
        transform.position = position;
    }

    public void SetTint(Color tint)
    {
        visual.color = tint;
    }

    public Vector3 BallRestPosition(float ballRadius)
    {
        return transform.position + Vector3.up * (paddleConfig.HalfHeight + ballRadius);
    }

    public void Step(float deltaTime)
    {
        Vector3 position = transform.position;
        position.x = PaddleMovement.NextX(position.x, ReadInputDirection(), paddleConfig.moveSpeed, deltaTime,
            movementRange.x, movementRange.y);
        transform.position = position;
    }

    private static float ReadInputDirection()
    {
        Keyboard keyboard = Keyboard.current;
        bool leftPressed = keyboard != null && (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed);
        bool rightPressed = keyboard != null && (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed);
        return PaddleMovement.InputDirection(leftPressed, rightPressed);
    }
}
