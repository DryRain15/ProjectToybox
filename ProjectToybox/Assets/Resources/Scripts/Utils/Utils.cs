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

    public static void DamageRedPulse(SpriteRenderer sr, float duration = 0.5f)
    {
        CoroutineManager.Instance.StartCoroutineCall(DamageRedPulseRoutine(sr, duration));
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

    public static AudioSource CreateSource(this MonoBehaviour super, AudioClip clip)
    {
        var source = super.gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        return source;
    }

    public static Direction ClampVectorToDirection(Vector3 velocity)
    {
        var norm = velocity.normalized;
        Direction dir = Direction.None;
        if (norm.x < -0.5f)
            dir |= Direction.Left;
        else if (norm.x > 0.5f)
            dir |= Direction.Right;
        if (norm.y < -0.5f)
            dir |= Direction.Down;
        else if (norm.y > 0.5f)
            dir |= Direction.Up;

        return dir;
    }

    public static Vector3 DirectionToVector(Direction direction)
    {
        var v = Vector3.zero;
        if (DirectionContains(direction, Direction.Left)) v.x = -2f;
        if (DirectionContains(direction, Direction.Right)) v.x = 2f;
        if (DirectionContains(direction, Direction.Down)) v.y = -1f;
        if (DirectionContains(direction, Direction.Up)) v.y = 1f;

        return v;
    }

    public static bool IsHorizontal(Direction direction)
    {
        return !DirectionContains(direction, Direction.Down) && !DirectionContains(direction, Direction.Up);
    }

    public static bool IsVertical(Direction direction)
    {
        return !DirectionContains(direction, Direction.Left) && !DirectionContains(direction, Direction.Right);
    }

    public static bool TargetInRange(this IFieldObject fo, IFieldObject target, float range)
    {
        var center = fo.Position;
        var a = range;
        var b = range * 0.5f;
        var point = target.Position;
        
        return Mathf.Pow(center.x - point.x, 2f) / (a * a) + Mathf.Pow(center.y - point.y, 2f) / (b * b) <= 1;
    }

    public static bool TargetInRangeAngle(this IFieldObject fo, IFieldObject target, float range, Direction direction)
    {
        var inRange = fo.TargetInRange(target, range);
        if (!inRange) return false;

        var dir = (target.Position - fo.Position).normalized;
        switch (direction)
        {
            case Direction.Up:
                return (dir.y > 0f && Mathf.Abs(dir.x) <= dir.y * 2);
            case Direction.Up | Direction.Left:
                return (dir.y > 0f && dir.x < 0f);
            case Direction.Left:
                return (dir.x < 0f && Mathf.Abs(dir.y) * -2 >= dir.x);
            case Direction.Down | Direction.Left:
                return (dir.y < 0f && dir.x < 0f);
            case Direction.Down:
                return (dir.y < 0f && Mathf.Abs(dir.x) <= dir.y * -2);
            case Direction.Down | Direction.Right:
                return (dir.y < 0f && dir.x > 0f);
            case Direction.Right:
                return (dir.x > 0f && Mathf.Abs(dir.y) * 2 <= dir.x);
            case Direction.Up | Direction.Right:
                return (dir.y > 0f && dir.x > 0f);
        }

        return false;
    }

    public static (float, float) DirectionToRange(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return (1f, 0.132f);
            case Direction.Up | Direction.Left:
                return (0.25f, 1f);
            case Direction.Left:
                return (0.367f, 0.857f);
            case Direction.Down | Direction.Left:
                return (0.5f, 0.75f);
            case Direction.Down:
                return (0.637f, 0.637f);
            case Direction.Down | Direction.Right:
                return (0.75f, 0.5f);
            case Direction.Right:
                return (0.857f, 0.367f);
            case Direction.Up | Direction.Right:
                return (1f, 0.25f);
        }

        return (0f, 0f);
    }

    public static void SetPosition(this ICharacterObject fo, Vector3 pos)
    {
        fo.Transform.position = new Vector3(pos.x, pos.y, fo.Transform.position.z);
    }
}
