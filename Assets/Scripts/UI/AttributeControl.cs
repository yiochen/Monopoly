using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AttributeControl : MonoBehaviour
{
    public Sprite EditorIconSprite;
    public string EditorAttributeName;
    public string EditorAttributeValue;
    [SerializeField] private Image IconImage;
    [SerializeField] private TextMeshProUGUI AttributeNameLabel;
    [SerializeField] private TextMeshProUGUI AttributeValueLabel;

    public Sprite IconSprite
    {
        get
        {
            return IconImage.sprite;
        }
        set
        {
            IconImage.sprite = value;
            IconImage.RecordPrefab();
        }
    }

    public string attributeName
    {
        get
        {
            return AttributeNameLabel.text;
        }
        set
        {
            AttributeNameLabel.text = value;
            AttributeNameLabel.RecordPrefab();
        }
    }

    public string attributeValue
    {
        get
        {
            return AttributeValueLabel.text;
        }
        set
        {
            AttributeValueLabel.text = value;
            AttributeValueLabel.RecordPrefab();
        }
    }
}
