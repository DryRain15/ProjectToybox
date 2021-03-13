using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags] public enum Direction
{
    None = 0,
    Up = 1,
    Left = 2,
    Right = 4,
    Down = 8,
}

public interface IMovable
{
    Vector3 Velocity { get; set; }
    void MoveTo();
}
