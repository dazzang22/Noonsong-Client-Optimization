using System;

public static class TimeManager
{
    public static bool IsTimeLoaded() => true;

    public static DateTime GetCurrentTime() => DateTime.UtcNow;
}