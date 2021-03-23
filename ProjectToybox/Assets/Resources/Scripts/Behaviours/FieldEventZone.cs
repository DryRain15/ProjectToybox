using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FieldEventZone : MonoBehaviour
{
    [SerializeField] private Vector2Int _size;
    [SerializeField]
    private PolygonCollider2D _col;
    
    private void OnValidate()
    {
        _col.points = new []
        {
            new Vector2(
            0.5f * (_size.x - _size.y), 
            -0.25f * (_size.x + _size.y)),
            new Vector2(
                -0.5f * (_size.x + _size.y), 
                0.25f * (_size.x - _size.y)),
            new Vector2(
                -0.5f * (_size.x - _size.y), 
                0.25f * (_size.x + _size.y)),
            new Vector2(
                0.5f * (_size.x + _size.y), 
                -0.25f * (_size.x - _size.y))
        };
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        IFieldObject fo = other.gameObject.GetComponent<IFieldObject>();
        if (fo == null) return;
        EventParameter param = new EventParameter(
            "Name".EventParameterPairing(gameObject.name),
            "Target".EventParameterPairing(fo)
        );
        EventController.Instance.EventCall("ZoneEnterEvent", param);
    }


    private void OnCollisionExit2D(Collision2D other)
    {
        IFieldObject fo = other.gameObject.GetComponent<IFieldObject>();
        if (fo == null) return;
        EventParameter param = new EventParameter(
            "Name".EventParameterPairing(gameObject.name),
            "Target".EventParameterPairing(fo)
        );
        EventController.Instance.EventCall("ZoneExitEvent", param);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var pos = (Vector2)transform.position;
        var points = _col.points;
        for (int i = 1; i < points.Length; i++)
        {
            Gizmos.DrawLine(pos + points[i - 1], pos + points[i]);
        }
        Gizmos.DrawLine(pos + points[3], pos + points[0]);
        Handles.color = Color.black;
        Handles.Label(transform.position, gameObject.name);
    }
}
