using System;
using Devkit.Modularis.References;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
public class TextUpdater : MonoBehaviour
{
    private TMP_Text _text;
    [SerializeField] private UIntReference currentKeyIndex;
    private void Start()
    {
        _text = GetComponent<TMP_Text>();
    }
    public void UpdateText(Command command, bool success)
    {
        
        HighlightCharacter(_text, (int)currentKeyIndex.Value, success);
    }
    public void HighlightCharacter(TMP_Text tmpText, int index, bool correctKeyPressed) {
        string greenStart = "<mark=#" + ColorUtility.ToHtmlStringRGB(Color.green) + ">";
        string redStart = "<mark=#" + ColorUtility.ToHtmlStringRGB(Color.red) + ">";
        string endTag = "</mark>";
        string mainText = tmpText.GetParsedText();
        if (correctKeyPressed)
        {
            mainText = mainText.Insert(index, endTag);
            mainText = mainText.Insert(0, greenStart);
        }
        else
        {
            mainText = mainText.Insert(index, endTag);
            mainText = mainText.Insert(math.max(index-1,0), redStart);
            mainText = mainText.Insert(math.max(index-1,0), endTag);
            mainText = mainText.Insert(0, greenStart);
        }
        tmpText.SetText(mainText);
        tmpText.ForceMeshUpdate(forceTextReparsing:true);
    }
    public void UnHighlightCharacter(TMP_Text tmpText, int index) {
        tmpText.ForceMeshUpdate();
        
        tmpText.ForceMeshUpdate(forceTextReparsing:true);
    }
}
