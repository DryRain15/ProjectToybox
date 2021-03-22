using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using UnityEditor;
using UnityEngine;

public class Phase3 : MonoBehaviour
{

    public Transform respawnLocation;

    private void Start()
    {
        GlobalInputController.Instance.AssignKey(KeyType.Dash, KeyCode.None);
    }

    private void OnGUI()
    {
        var e = Event.current;
        if (e.type != EventType.KeyDown) return;

        var key = e.keyCode;
        if (!Input.GetKeyDown(key))
            return;
        var pressedType = GlobalInputController.FindInputType(key);

        if (GlobalInputController.Instance.CheckKeyAssigned(KeyType.Dash)) return;
        if (pressedType != InputType.None) return; //누른키가 none(이동키가 아니어야만)
        GlobalInputController.Instance.AssignKey(KeyType.Dash, key);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!GlobalInputController.Instance.CheckKeyAssigned(KeyType.Dash))
        {
            PlayerBehaviour.Instance.Position = respawnLocation.position;
        }
    }

}
