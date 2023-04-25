using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEngine;

public class KeyButtonHandler : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityTouchHandler
{
    private TMP_Text _text;
    private ExperimentManager _experimentManager;
    private Command _command;
    [SerializeField] private Transform BackplateTransform;
    
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
    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        _experimentManager.OnButtonPressBegin(_command, eventData.Handedness,BackplateTransform);
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        _experimentManager.OnbuttonPressThisFrame(_command, eventData.Handedness,BackplateTransform);
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        _experimentManager.OnButtonPressEnd(_command, eventData.Handedness,BackplateTransform);
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        _experimentManager.OnButtonClicked(_command, eventData.Handedness,BackplateTransform);
    }

    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        _experimentManager.OnButtonTouchBegin(_command, eventData.Handedness,BackplateTransform);
    }

    public void OnTouchCompleted(HandTrackingInputEventData eventData)
    {
        _experimentManager.OnButtonTouchEnd(_command, eventData.Handedness,BackplateTransform);
    }

    public void OnTouchUpdated(HandTrackingInputEventData eventData)
    {
    }
}
