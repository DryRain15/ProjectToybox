using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using UnityEditor;
using UnityEngine;

public class Phase4 : MonoBehaviour
{

    public Transform respawnLocation;

    private void Start()
    {
        GlobalInputController.Instance.AssignKey(KeyType.Cancel, KeyCode.None);
        EventController.Instance.Subscribe("ZoneEnterEvent", OnDashZoneEnter);
    }

    private void OnGUI()
    {
        var e = Event.current;
        if (e.type != EventType.KeyDown) return;

        var key = e.keyCode;
        if (!Input.GetKeyDown(key))
            return;
        var pressedType = GlobalInputController.FindInputType(key);

        if (GlobalInputController.Instance.CheckKeyAssigned(KeyType.Cancel)) return;
        if (pressedType != InputType.None) return; //누른키가 none(이동키가 아니어야만)
        GlobalInputController.Instance.AssignKey(KeyType.Cancel, key);
    }

    private void OnDashZoneEnter(EventParameter param)
    {
        if (param.Get<string>("Name") != "CancelZone") return;
        if (!GlobalInputController.Instance.CheckKeyAssigned(KeyType.Cancel)
            || PlayerBehaviour.Instance.currentHold != null)
        {
            PlayerBehaviour.Instance.Position = respawnLocation.position;
        }
    }

}
