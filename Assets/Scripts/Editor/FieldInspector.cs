using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldControl))]
public class FieldInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        FieldControl fieldControl = (FieldControl)target;
        fieldControl.label = fieldControl.EditorLabel;
        fieldControl.placeholder = fieldControl.EditorPlaceholder;
        fieldControl.text = fieldControl.EditorText;
    }
}
