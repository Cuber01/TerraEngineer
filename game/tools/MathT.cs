using System;
using System.Collections.Generic;
using System.Numerics;
using Godot;
using TerraEngineer.entities.mobs;
using Vector2 = Godot.Vector2;


namespace TerraEngineer;

public static class MathT
{
    public const float PI = (float)Math.PI;
    public const int NEGATIVE_INF = -99999999;
    public const int POSITIVE_INF = 99999999;

    private static Random random = new Random();

    public static float RandomFloat(float min, float max)
    {
        if (min == max) return min;

        return (float)(min + random.NextDouble() * (max - min));
    }
    
    public static int RandomInt(int min, int max)
    {
        if (min == max) return min;

        return random.Next(min, max+1);
    }
    
    public static object RandomChooseList(List<int> list)
    {
        int i = RandomInt(0, random.Next(list.Count));
        return list[i];
    }

    public static object RandomChoose(params object[] args)
    {
        int i = RandomInt(0, random.Next(args.Length));
        return args[i];
    }

    public static Vector2 RandomVector2(float min, float max)
    {
        return new Vector2
        {
            X = (float)(min + random.NextDouble() * (max - min)),
            Y = (float)(min + random.NextDouble() * (max - min))
        };
    }

    public static Vector2 dir4ToVect2(Direction4 dir)
    {
        switch (dir)
        {
            case Direction4.Up:
                return Vector2.Up;
            case Direction4.Down:
                return Vector2.Down;
            case Direction4.Left:
                return Vector2.Left;
            case Direction4.Right:
                return Vector2.Right;
        }

        return Vector2.Zero;
    }

    // Taking clock hands direction as reference
    public static Direction4 rotateDir4(Direction4 dir, bool right)
    {
        switch (dir)
        {
            case Direction4.Up:
                return right ? Direction4.Right : Direction4.Left;
            case Direction4.Down:
                return right ? Direction4.Left : Direction4.Right;
            case Direction4.Left:
                return right ? Direction4.Up : Direction4.Down;
            case Direction4.Right:
                return right ? Direction4.Down : Direction4.Up;
        }
        
        return Direction4.None;
    }
    
    public static Vector2 rotateVec2(Vector2 dir, bool right)
    {
        if(dir.Y == -1)
            return right ? Vector2.Right : Vector2.Left;
        if(dir.Y == 1)
            return right ? Vector2.Left : Vector2.Right;
        if(dir.X == -1)
            return right ? Vector2.Up : Vector2.Down;
        if(dir.X == 1)
            return right ? Vector2.Down : Vector2.Up;
        
        return Vector2.Zero;
    }

    public static float Lerp(float from, float to, float weight, float delta)
    {
        return Mathf.Lerp(from, to, 1 - Mathf.Exp(-weight * delta));
    }

    public static Vector2 Lerp(Vector2 from, Vector2 to, float weight, float delta)
    {
        return new Vector2
        {
            X = MathT.Lerp(from.X, to.X, weight, delta),
            Y = MathT.Lerp(from.Y, to.Y, weight, delta)
        };
    }


}