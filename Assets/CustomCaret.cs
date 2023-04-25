using System.Collections;
using Devkit.Modularis.References;
using TMPro;
using UnityEngine;

public class CustomCaret : MonoBehaviour
{
    [SerializeField] private UIntReference characterIndex;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI caret;
    [SerializeField,Range(0,1f)] private float height = 0.5f;
    
    [SerializeField,Range(0.1f,2f)] private float blinkInterval = 0.7f;

    private void Start()
    {
        StartCoroutine(nameof(Blink));
    }

    private void Update()
    {
        SetCaretLeftFromSpecificCharacter(characterIndex.Value);
    }

    private IEnumerator Blink()
    {
        var color = caret.color;
        while (true)
        {
            caret.color = Color.clear;
            yield return new WaitForSeconds(blinkInterval);
            caret.color = color;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
    
    public void SetCaretLeftFromSpecificCharacter(uint index)
    {
        text.ForceMeshUpdate();
        TMP_CharacterInfo characterInfo = text.textInfo.characterInfo[index];
        Vector3 localPosition = characterInfo.bottomLeft;
        Vector3 worldPosition = text.transform.TransformPoint(localPosition);
        transform.position = worldPosition + Vector3.up * height;
        
    }
}
