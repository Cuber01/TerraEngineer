using System;
using Godot;


namespace TerraEngineer;

public static class MathTools
{
    public const float PI = (float)Math.PI;
    public const int NEGATIVE_INF = -99999999;
    public const int POSITIVE_INF = 99999999;
    
    private static Random random = new Random();

    public static float RandomFloat(float min, float max)
    {
        if(min == max) return min;
        
        return (float)(min + random.NextDouble() * (max - min));
    }

    public static Vector2 RandomVector2(float min, float max)
    {
        return new Vector2
        {
            X = (float)(min + random.NextDouble() * (max - min)),
            Y = (float)(min + random.NextDouble() * (max - min))
        };
    }
    
    public static float Lerp(float from, float to, float weight) => Mathf.Lerp(from, to, weight);

    public static Vector2 Lerp(Vector2 from, Vector2 to, float weight)
    {
        return new Vector2
        {
            X = Mathf.Lerp(from.X, to.X, weight),
            Y = Mathf.Lerp(from.Y, to.Y, weight)
        };
    }
}