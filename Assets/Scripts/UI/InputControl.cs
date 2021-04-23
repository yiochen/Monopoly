using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class InputControl : MonoBehaviour
{
    public string EditorPlaceholder = "";
    public string EditorText = "";
    [SerializeField] private TextMeshProUGUI PlaceholderChild;
    [SerializeField] private TMP_InputField TextComp;

    public string text
    {
        get
        {
            return TextComp.text;
        }
        set
        {
            TextComp.text = value;
        }
    }


    public string placeholder
    {
        get
        {
            return PlaceholderChild.text;
        }
        set
        {
            PlaceholderChild.text = value;
        }
    }
}
