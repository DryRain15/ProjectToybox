using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using UnityEditor;
using UnityEngine;

public class Phase2 : MonoBehaviour
{

    //처음으로 누른 이동키/사용키를 설정

    private void OnGUI()
    {
        var e = Event.current;
        if (e.type != EventType.KeyDown) return;

        var key = e.keyCode;
        if (!Input.GetKeyDown(key))
            return;
        var pressedType = GlobalInputController.FindInputType(key);

        if (GlobalInputController.Instance.InputType == InputType.None) //이동 키가 설정이 안되었다면
        {
            if (pressedType != InputType.None)
            {
                GlobalInputController.Instance.InputType = pressedType;
                Debug.Log("최초 입력 방식 설정됨: " + pressedType);
                return;
            }
        }

        if (GlobalInputController.Instance.CheckKeyAssigned(KeyType.Use)) return;
        if (pressedType != InputType.None) return; //누른키가 none(이동키가 아니어야만)
        GlobalInputController.Instance.AssignKey(KeyType.Use, key);
    }

}
