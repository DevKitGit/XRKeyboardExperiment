using System.Linq;
using TMPro;
using UnityEngine;
public class KeyboardCharacterPressInvoker : MonoBehaviour
{
    private TMP_Text _text;
    private ExperimentManager _experimentManager;
    private Command _command;
    private void Start()
    {
        _text = GetComponentsInChildren<TMP_Text>().FirstOrDefault();
        _experimentManager = FindObjectOfType<ExperimentManager>();
        Command.KeyType type = _text.text switch {
            "Enter" => Command.KeyType.Enter,
            "Space" => Command.KeyType.Space,
            "Backspace" => Command.KeyType.Backspace,
            _ => Command.KeyType.Character
        };
        _command = new Command(_text.text, type);
    }
    public void OnButtonTouchBegin()
    {
        _experimentManager.OnButtonTouchBegin(_command);
    }
    public void OnButtonTouchEnd()
    {
        _experimentManager.OnButtonTouchEnd(_command);
    }
    public void OnButtonPressBegin()
    {
        _experimentManager.OnButtonPressBegin(_command);
    }

    public void OnButtonPressEnd()
    {
        _experimentManager.OnButtonPressEnd(_command);
    }
    public void OnClicked()
    {
        _experimentManager.OnButtonClicked(_command);
    }
}
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


