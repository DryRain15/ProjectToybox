using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HoldState
{
    None,
    StartHold,
    Holding,
    OnAction,
    EndAction,
    EndHold,
    NonHoldable,
}
public interface IHoldable
{
    GameObject GameObject { get; }
    HoldState HoldState { get; set; }
    ICharacterObject Holder { get; set; }
    
    void Hold(ICharacterObject target);
    void Use();
    void Release();

    void HoldableUpdate();
}
