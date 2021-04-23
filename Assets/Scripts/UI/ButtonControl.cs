using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using TMPro;

[RequireComponent(typeof(Button))]
public class ButtonControl : MonoBehaviour
{
    public string EditorText;

    public Type EditorType;

    [SerializeField] private TextMeshProUGUI Text;
    [SerializeField] private Color PrimaryColor;
    [SerializeField] private Color SecondaryColor;

    public enum Type
    {
        Primary,
        Secondary,
    }

    private Button Button;

    public string text
    {
        get
        {
            return Text.text;
        }
        set
        {

            Text.text = value;
            Text.RecordPrefab();
        }
    }

    public Type buttonType
    {
        get
        {
            var color = GetComponent<Image>().color;
            if (color.Equals(PrimaryColor))
            {
                return Type.Primary;
            }
            if (color.Equals(SecondaryColor))
            {
                return Type.Secondary;
            }
            return Type.Primary;
        }
        set
        {
            var image = GetComponent<Image>();
            switch (value)
            {
                case Type.Primary:
                    image.color = PrimaryColor;
                    break;
                case Type.Secondary:
                    image.color = SecondaryColor;
                    break;
            }
            image.RecordPrefab();
        }
    }

    void Awake()
    {
        Button = GetComponent<Button>();
    }

    public void SetCallback(UnityAction callback)
    {
        if (Button == null)
        {
            Button = GetComponent<Button>();
        }
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(callback);
    }

    public void ClearCallback()
    {
        if (Button == null)
        {
            Button = GetComponent<Button>();
        }
        Button.onClick.RemoveAllListeners();
    }
}
