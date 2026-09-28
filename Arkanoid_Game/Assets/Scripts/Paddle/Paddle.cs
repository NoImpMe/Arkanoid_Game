using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    [SerializeField] private PaddleConfig paddleConfig;
    [SerializeField] private StageConfig stageConfig;
    [SerializeField] private BoxCollider body;
    [SerializeField] private SpriteRenderer visual;

    private void Awake()
    {
        transform.position = Vector3.up * paddleConfig.positionY;
        body.center = Vector3.zero;
        body.size = new Vector3(paddleConfig.width, paddleConfig.height, stageConfig.colliderDepth);
        visual.transform.localScale = SpriteFitting.ScaleToFit(visual.sprite.bounds.size, paddleConfig.Size);
    }

    public Rect Area => WorldRect.FromCenter(transform.position, body.size);

    public Vector3 BallRestPosition(float ballRadius)
    {
        return transform.position + Vector3.up * (paddleConfig.HalfHeight + ballRadius);
    }

    public void Step(float deltaTime)
    {
        Vector3 position = transform.position;
        position.x = PaddleMovement.NextX(position.x, ReadInputDirection(), paddleConfig.moveSpeed, deltaTime,
            paddleConfig.minX, paddleConfig.maxX);
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
