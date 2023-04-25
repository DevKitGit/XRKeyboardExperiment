using System.Linq;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class KeyboardGenerator : MonoBehaviour
{
    
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject spaceBarPrefab;
    [SerializeField] private GameObject backspacePrefab;
    
    [SerializeField] private GameObject keyPrefab16mm;
    [SerializeField] private GameObject spaceBarPrefab16mm;
    [SerializeField] private GameObject backspacePrefab16mm;

    
    [SerializeField] private GameObject keyPrefab30mm;
    [SerializeField] private GameObject spaceBarPrefab30mm;
    [SerializeField] private GameObject backspacePrefab30mm;

    
    [SerializeField] private float keySize;
    [SerializeField] private float HorizontalSkewAmount;
    
    private string[][] _keyLayout = {
        new[] {"1","2","3","4","5","6","7","8","9","0","?","","Backspace"},
        new[] {"Q","W","E","R","T","Y","U","I","O","P","Å"},
        new[] {"A","S","D","F","G","H","J","K","L","Æ","Ø"},
        new[] {"Z","X","C","V","B","N","M",",",".","-"},
        new[] {"","","","","Space"}
    };
    
    public void BuildKeyboard(int i)
    {
        ClearCurrentKeyboard();
        var startXOffset = keySize * _keyLayout[0].Length / 2;
        var startYOffset = keySize * _keyLayout.Length / 2;
        for (var y = 0; y < _keyLayout.Length; y++)
        {
            var horizontalXSkew = keySize * y * HorizontalSkewAmount;
            for (var x = 0; x < _keyLayout[y].Length; x++)
            {
                var col = _keyLayout[y][x];
                if (col == "")
                {
                    continue;
                }
                var worldSpacePos = new Vector3(x * keySize - startXOffset + horizontalXSkew,
                    -y * keySize + startYOffset);
                var local = transform.TransformPoint(worldSpacePos);
                var localPrefab = col switch
                {
                    "Backspace" => i == 1 ? backspacePrefab : i == 2 ? backspacePrefab16mm : backspacePrefab30mm,
                    "Space" => i == 1 ? spaceBarPrefab : i == 2 ? spaceBarPrefab16mm : spaceBarPrefab30mm,
                    _ => i == 1 ? keyPrefab : i == 2 ? keyPrefab16mm : keyPrefab30mm,
                };
                var instance = Instantiate(localPrefab, local + localPrefab.transform.position, Quaternion.identity);
                instance.transform.SetParent(transform);
                instance.transform.localRotation = Quaternion.identity;
                instance.GetComponentInChildren<TextMeshPro>(true).SetText(col);
                instance.GetComponentInChildren<TextMeshPro>(true).enabled = col switch
                {
                    "Backspace" => false,
                    "Space" => false,
                    _ => instance.GetComponentInChildren<TextMeshPro>(true).enabled
                };
                instance.name = col;
            }
        }
    }
    
    [ContextMenu("Clear keyboard")]
    private void ClearCurrentKeyboard()
    {
        var children = transform.GetComponentsInChildren<Transform>(true).Skip(1).ToArray();
        for (var i = 0; i < children.Length; i++)
        {
            if (children[i] != null && children[i].parent == transform)
            {
#if UNITY_EDITOR
                Undo.DestroyObjectImmediate(children[i].gameObject);
#else
                Destroy(children[i].gameObject);
#endif
            }
        }
    }
}