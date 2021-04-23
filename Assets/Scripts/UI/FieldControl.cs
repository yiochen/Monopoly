using UnityEngine;
using TMPro;

public class FieldControl : MonoBehaviour
{
    public string EditorLabel;
    public string EditorPlaceholder;
    public string EditorText;

    [SerializeField] private TextMeshProUGUI LabelChild;
    [SerializeField] private InputControl InputControl;

    public string label
    {
        get
        {
            return LabelChild.text;
        }
        set
        {
            LabelChild.text = value;
        }
    }

    public string placeholder
    {
        get
        {
            return InputControl.placeholder;
        }
        set
        {
            InputControl.placeholder = value;
        }
    }

    public string text
    {
        get
        {
            return InputControl.text;
        }
        set
        {
            InputControl.text = value;
        }
    }

}
