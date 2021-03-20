using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Stats))]
public class StatsPropertyDrawer : PropertyDrawer
{
    private float contentHeight;
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        var indent = EditorGUI.indentLevel;
        var objField = new Rect(position.x, position.y, position.width, 20f);
        EditorGUI.ObjectField(objField, property);

        if (property.objectReferenceValue != null)
        {
            contentHeight = 120f;
            SerializedObject statProperty = new SerializedObject( property.objectReferenceValue );
        
            // Don't make child fields be indented
            EditorGUI.indentLevel = 2;

            // Calculate rects
            var hpRect = new Rect(position.x, position.y + 24f, position.width, 20f);
            var hpGenRect = new Rect(position.x, position.y + 44f, position.width, 20f);
            var atkRect = new Rect(position.x, position.y + 64f, position.width, 20f);
            var feverRect = new Rect(position.x, position.y + 84f, position.width, 20f);
            var speedRect = new Rect(position.x, position.y + 104f, position.width, 20f);
            
            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(hpRect, statProperty.FindProperty("hp"), new GUIContent("HP"), true);
            EditorGUI.PropertyField(hpGenRect, statProperty.FindProperty("hpGen"), new GUIContent("HPGen"), true);
            EditorGUI.PropertyField(atkRect, statProperty.FindProperty("atk"), new GUIContent("Atk"), true);
            EditorGUI.PropertyField(feverRect, statProperty.FindProperty("fever"), new GUIContent("FeverMult"), true);
            EditorGUI.PropertyField(speedRect, statProperty.FindProperty("moveSpeed"), new GUIContent("MoveSpeed"), true);
        }
        else
        {
            contentHeight = 20f;
        }
        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //set the height of the drawer by the field size and padding
        return contentHeight;
    }
}
