using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private BallConfig ballConfig;
    [SerializeField] private SpriteRenderer visual;

    private BallMover mover;

    public Vector2 Velocity { get; private set; }
    public float Radius => ballConfig.radius;

    private void Awake()
    {
        mover = BallMover.FromConfig(ballConfig);
        visual.transform.localScale = SpriteFitting.ScaleToFit(visual.sprite.bounds.size, Vector2.one * ballConfig.Diameter);
    }

    public void PlaceAt(Vector3 position)
    {
        transform.position = position;
        Velocity = Vector2.zero;
    }

    public void Launch()
    {
        Velocity = BallBounce.VelocityFromAngle(ballConfig.initialVerticalSpeed, ballConfig.launchAngleDegrees);
    }

    public bool Step(float deltaTime)
    {
        Physics.SyncTransforms();
        BallMoveResult result = mover.Move(transform.position, Velocity, deltaTime);
        transform.position = result.Position;
        Velocity = result.ReachedDeadZone ? Vector2.zero : result.Velocity;
        return result.ReachedDeadZone;
    }
}
