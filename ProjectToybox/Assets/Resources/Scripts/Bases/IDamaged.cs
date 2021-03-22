using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType
{
    None,
    Player,
    Enemy,
    Neutral,
}

public class DamageState
{
    public ICharacterObject Sender;
    public ICharacterObject Getter;
    public float Damage;
    public float KnockBack;
    public int DamageType;
}

public interface IDamaged
{
    GameObject GameObject { get; }
    HitType HitType { get; set; }
    void GetHit(DamageState state);
}
