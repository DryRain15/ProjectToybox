using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AutoState
{
    None,
    Wait,
    Follow,
    Action,
    Custom1,
    Custom2,
    Custom3,
    Custom4,
}

public interface IAutoBehaviour
{
    string BattleGroup { get; set; }
    string NoticeGroup { get; set; }
    
    AutoState AutoState { get; set; }

    void OnStateTransition(AutoState to);

    void AutoUpdate();
}
