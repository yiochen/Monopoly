using UnityEditor;

[CustomEditor(typeof(AttributeControl))]
public class AttributeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AttributeControl attributeControl = (AttributeControl)target;

        attributeControl.IconSprite = attributeControl.EditorIconSprite;
        attributeControl.attributeName = attributeControl.EditorAttributeName;
        attributeControl.attributeValue = attributeControl.EditorAttributeValue;
    }
}