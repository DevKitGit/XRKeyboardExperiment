using System.Linq;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEngine;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;

public class KeyButtonHandler : MonoBehaviour, IMixedRealityTouchHandler
{
    private TMP_Text _text;
    private ExperimentManager _experimentManager;
    private Command _command;
    [SerializeField] private Transform BackplateTransform;
    private Handedness currentTouchingFinger;
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

    public void OnClicked()
    {
        _experimentManager.OnButtonClicked(_command,currentTouchingFinger,BackplateTransform);
    }

    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        _experimentManager.OnButtonPressBegin(_command, eventData.Handedness,BackplateTransform);
    }
    public void OnTouchCompleted(HandTrackingInputEventData eventData)
    {
        _experimentManager.OnButtonPressEnd(_command, eventData.Handedness,BackplateTransform);
    }
    public void OnTouchUpdated(HandTrackingInputEventData eventData)
    {
        _experimentManager.OnbuttonPressThisFrame(_command,eventData.Handedness,BackplateTransform);
        currentTouchingFinger = eventData.Handedness;
    }
}
