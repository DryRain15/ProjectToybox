using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using UnityEditor;
using UnityEngine;

public class Phase3 : MonoBehaviour
{

    public Transform respawnLocation;
    private BoxCollider2D triggerArea;

    private void Start()
    {
        triggerArea = GetComponent<BoxCollider2D>();
        GlobalInputController.Instance.AssignKey(KeyType.Dash, KeyCode.None);
        Debug.Log("start start");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collide enter");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("collide exit");
        if (!GlobalInputController.Instance.CheckKeyAssigned(KeyType.Dash))
        {
            PlayerBehaviour.Instance.Position = respawnLocation.position;
        }
    }

}
