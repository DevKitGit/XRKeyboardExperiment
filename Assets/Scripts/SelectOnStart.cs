using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectOnStart : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<UnityEngine.UI.Selectable>().Select();
    }

}
