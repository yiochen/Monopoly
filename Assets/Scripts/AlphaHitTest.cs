using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Enable object with Image to do hit test with alpha information
/// The texture needs to have 
/// Mesh type = Full Rect
/// Read/Write Enabled = checked
/// </summary>
public class AlphaHitTest : MonoBehaviour
{
    public float AlphaThreadhold = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaThreadhold;
    }

}
