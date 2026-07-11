using System;

public static class FloatExtensions
{
    public static bool IsWithin(this float value, float target, float tolerance)
    {
        return MathF.Abs(value - target) <= tolerance;
    }
}