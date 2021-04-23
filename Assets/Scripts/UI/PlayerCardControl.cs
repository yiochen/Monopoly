using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCardControl : MonoBehaviour
{
    [SerializeField] private Image Image;
    [SerializeField] private TextMeshProUGUI NameLabel;
    public string playerName
    {
        get
        {
            return NameLabel.text;
        }
        set
        {
            NameLabel.text = value;
        }
    }

    [HideInInspector] public string playerId;

}
