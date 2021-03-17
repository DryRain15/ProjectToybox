using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Color = System.Drawing.Color;

public static class Utils
{
    public static bool DirectionContains(Direction direction, Direction target)
    {
        return (direction & target) == target;
    }
    
    public static Vector3 GetAngularOffset(Direction direction, float distance, float zPos = 1.35f)
    {
        var xPos = 0f;
        var yPos = 0f;
        xPos += DirectionContains(direction, Direction.Left) ? -1f :
            DirectionContains(direction, Direction.Right) ? 1f : 0f;
        yPos += DirectionContains(direction, Direction.Down) ? -0.5f :
            DirectionContains(direction, Direction.Up) ? 0.5f : 0f;

        if (xPos * yPos == 0f)
        {
            xPos *= 1.25f;
            yPos *= 1.25f;
        }
        
        return new Vector3(xPos * distance, yPos * distance, zPos - yPos);
    }

    public static Direction RotateDirectionCW(Direction direction, int count = 1)
    {
        var newDir = direction;
        for (int i = 0; i < count; i++)
            switch (newDir)
            {
                case Direction.Up:
                    newDir = Direction.Up | Direction.Right;
                    break;
                case Direction.Up | Direction.Right:
                    newDir = Direction.Right;
                    break;
                case Direction.Right:
                    newDir = Direction.Down | Direction.Right;
                    break;
                case Direction.Down | Direction.Right:
                    newDir = Direction.Down;
                    break;
                case Direction.Down:
                    newDir = Direction.Down | Direction.Left;
                    break;
                case Direction.Down | Direction.Left:
                    newDir = Direction.Left;
                    break;
                case Direction.Left:
                    newDir = Direction.Up | Direction.Left;
                    break;
                case Direction.Up | Direction.Left:
                    newDir = Direction.Up;
                    break;
            }
        return newDir;
    }
    
    public static Direction RotateDirectionCCW(Direction direction, int count = 1)
    {
        var newDir = direction;
        for (int i = 0; i < count; i++)
            switch (newDir)
            {
                case Direction.Up:
                    newDir = Direction.Up | Direction.Left;
                    break;
                case Direction.Up | Direction.Left:
                    newDir = Direction.Left;
                    break;
                case Direction.Left:
                    newDir = Direction.Down | Direction.Left;
                    break;
                case Direction.Down | Direction.Left:
                    newDir = Direction.Down;
                    break;
                case Direction.Down:
                    newDir = Direction.Down | Direction.Right;
                    break;
                case Direction.Down | Direction.Right:
                    newDir = Direction.Right;
                    break;
                case Direction.Right:
                    newDir = Direction.Up | Direction.Right;
                    break;
                case Direction.Up | Direction.Right:
                    newDir = Direction.Up;
                    break;
            }
        return newDir;
    }

    public static void DamageRedPulse(SpriteRenderer sr)
    {
        CoroutineManager.Instance.StartCoroutineCall(DamageRedPulseRoutine(sr));
    }

    static IEnumerator DamageRedPulseRoutine(SpriteRenderer sr, float duration = 0.5f)
    {
        var origin = sr.color;
        var innerTimer = 0f;
        while (innerTimer < duration / 2)
        {
            sr.color = UnityEngine.Color.Lerp(
                UnityEngine.Color.white, UnityEngine.Color.red,
                innerTimer / (duration / 2));
            innerTimer += Time.deltaTime;
            yield return null;   
        }

        innerTimer = 0f;
        while (innerTimer < duration / 2)
        {
            sr.color = UnityEngine.Color.Lerp(
                UnityEngine.Color.red, UnityEngine.Color.white,
                innerTimer / (duration / 2));
            innerTimer += Time.deltaTime;
            yield return null;   
        }

        sr.color = origin;
    }

    public static KeyValuePair<string, object> EventParameterPairing(this string key, object value)
    {
        return new KeyValuePair<string, object>(key, value);
    }
}
