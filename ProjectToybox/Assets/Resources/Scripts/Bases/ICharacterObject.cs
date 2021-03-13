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

public interface ICharacterObject
{
    Direction Direction { get; set; }

    Stats Stats { get; set; }
}
