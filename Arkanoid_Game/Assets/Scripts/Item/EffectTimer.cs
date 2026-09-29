public class EffectTimer
{
    public bool IsRunning { get; private set; }
    public float RemainingSeconds { get; private set; }

    public void Start(float durationSeconds)
    {
        RemainingSeconds = durationSeconds;
        IsRunning = true;
    }

    public bool Tick(float deltaTime)
    {
        if (!IsRunning)
        {
            return false;
        }

        RemainingSeconds -= deltaTime;
        if (RemainingSeconds > 0f)
        {
            return false;
        }

        Stop();
        return true;
    }

    public void Stop()
    {
        IsRunning = false;
        RemainingSeconds = 0f;
    }
}
