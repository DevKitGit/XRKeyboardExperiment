using System.Linq;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;
using UnityEngine;
public class KeyboardCharacterPressInvoker : MonoBehaviour
{ }
public struct Command
{
    public string Name;
    public KeyType Type;
    public Command(string name, KeyType type)
    {
        Name = name;
        Type = type;
    }
    public enum KeyType
    {
        Character,
        Backspace,
        Enter,
        Space,
    }
}


