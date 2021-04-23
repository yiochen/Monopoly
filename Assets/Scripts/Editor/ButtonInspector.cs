using UnityEditor;


[CustomEditor(typeof(ButtonControl))]
public class ButtonInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ButtonControl buttonControl = (ButtonControl)target;
        buttonControl.text = buttonControl.EditorText;
        buttonControl.buttonType = buttonControl.EditorType;
    }
}
