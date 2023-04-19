using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed = 10f;

    private void Update()
    {
        scrollRect.verticalNormalizedPosition += scrollSpeed * Time.deltaTime;
    }
}
