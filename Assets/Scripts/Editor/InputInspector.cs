using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InputControl))]
public class InputInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        InputControl inputControl = (InputControl)target;

        inputControl.placeholder = inputControl.EditorPlaceholder;
        inputControl.text = inputControl.EditorText;
    }
}
