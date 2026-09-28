using UnityEngine;

public static class LayoutSession
{
    public static bool HasFinishedGame { get; private set; }

    public static void MarkGameFinished()
    {
        HasFinishedGame = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Reset()
    {
        HasFinishedGame = false;
    }
}
