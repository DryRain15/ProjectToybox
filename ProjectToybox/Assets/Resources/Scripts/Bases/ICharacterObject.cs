using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEngine;

public class Stats
{
    public float Hp;
    public float HpGen;
    public float Atk;
    public float Fever;
    public float MoveSpeed;
}

public enum AnimState
{
    Stand = 0,
    Move = 1,
    Attack = 2,
    Interact = 3,
    Hold = 4,
    HoldMove = 5,
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
