using System;
using TerraEngineer.entities.mobs;

public static class Extensions
{
    // float
    public static bool IsWithin(this float value, float target, float tolerance)
    {
        return MathF.Abs(value - target) <= tolerance;
    }
    
    // DirectionX
    public static DirectionX Opposite(this DirectionX value)
    {
        return (DirectionX)(-(int)value);
    }
}