using System;
using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEngine;

public enum AnimState
{
    Stand = 0,
    Move = 1,
    Hold = 2,
    HoldMove = 3,
    Attack = 4,
    Interact = 5,
    Dance = 6,
    Dash = 7,
}

public interface ICharacterObject
{
    GameObject GameObject { get; }
    Direction Direction { get; set; }
    AnimState AnimState { get; set; }

    Transform Transform { get; }
    Stats Stats { get; set; }
}
