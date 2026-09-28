using UnityEngine;

public static class SessionHighScore
{
    private static bool hasValue;
    private static int value;

    public static int Get(int initialHighScore)
    {
        return hasValue ? value : initialHighScore;
    }

    public static void Submit(int highScore)
    {
        value = hasValue ? Mathf.Max(value, highScore) : highScore;
        hasValue = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Clear()
    {
        hasValue = false;
        value = default;
    }
}
