using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Devkit.Modularis.References;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public class ExperimentManager : MonoBehaviour
{
    [SerializeField] private UIntReference currentIndex;
    [SerializeField] private  StringReference promptString;
    private LoggingManager _loggingManager;
    private string LogName => $"scenario.{currentCondition}";
    private LatinSquareRandomizer _latinSquareRandomizer;
    private bool _testStarted;
    private float _testStartTime;
    public int secondsToWait;
    private int _previousIndex;
    private int currentCondition = 1;
    private Dictionary<string, object> loggedData = new()
    {
        {"Character",null},
        {"KeyType",null},
        {"Time",null},
        {"Index",null},
        {"Handedness",null},
        {"IndexDepth",null},
        {"EventType",null}
    };
    public void OnButtonPressBegin(Command command, Handedness handedness, Transform backplateTransform)
    {
        GenerateLog(command,"PressBegin",handedness,backplateTransform);
        _loggingManager.Log(LogName, loggedData);
    }
    public void OnButtonPressEnd(Command command, Handedness handedness, Transform backplateTransform)
    {
        GenerateLog(command,"PressEnd",handedness,backplateTransform);
        _loggingManager.Log(LogName, loggedData);
    }
    public void OnbuttonPressThisFrame(Command command, Handedness handedness, Transform backplateTransform)
    {
        GenerateLog(command,"PressThisFrame",handedness,backplateTransform);
        _loggingManager.Log(LogName, loggedData);
    }

    private float CalculateIndexDepth(Handedness handedness, Transform backplateTransform)
    {
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, handedness, out MixedRealityPose pose);
        var planeNormal = backplateTransform.rotation * Vector3.forward;
        Plane plane = new Plane(planeNormal, backplateTransform.position);

        var distance = plane.GetDistanceToPoint(pose.Position);
        return distance;
    }

    private bool previouslyWrong = false;
    public void OnButtonClicked(Command command, Handedness handedness, Transform backplateTransform)
    {
        bool success = false;
        var conditionShouldChange = false;
        switch (command.Type)
        {
            case Command.KeyType.Backspace:
                if (currentIndex.Value > 0)
                {
                    currentIndex.Value--;
                    success = true;
                }
                break;
            case Command.KeyType.Space:
                if (previouslyWrong)
                {
                    break;
                }
                if (currentIndex.Value == promptString.Value.Length-1)
                {
                    OnConditionShouldChange();
                    break;
                }
                success = promptString.Value[(int)currentIndex.Value].ToString() == " ";
                currentIndex.Value += 1;
                break;
            case Command.KeyType.Character:
                if (previouslyWrong)
                {
                    break;
                }

                if (currentIndex.Value == promptString.Value.Length-1)
                {
                    success = promptString.Value[(int)currentIndex.Value].ToString().ToUpper() == command.Name;
                    conditionShouldChange = success;
                    break;
                }
                success = promptString.Value[(int)currentIndex.Value].ToString().ToUpper() == command.Name;
                currentIndex.Value += 1;
                break;
        }

        previouslyWrong = !success;
        GenerateLog(command, "Clicked", handedness, backplateTransform);
        _loggingManager.Log(LogName, loggedData);
        if (conditionShouldChange)
        {
            previouslyWrong = false;
            OnConditionShouldChange();
            FindObjectOfType<TextUpdater>().ResetHighlight();
        }
        else
        {
            FindObjectOfType<TextUpdater>().UpdateText(command, success);

        }
    }

  
    private void OnConditionShouldChange()
    {
        currentIndex.Value = 0;

        var index =  _latinSquareRandomizer.GetNextConditionID();
        if (index.HasValue)
        {
            currentCondition = index.Value;
            FindObjectOfType<KeyboardGenerator>()?.BuildKeyboard(index.Value);
        }
        else
        {
            SaveAllData();
        }
    }
    private void Start()
    {
        _loggingManager = GetComponent<LoggingManager>();
        _latinSquareRandomizer = GetComponent<LatinSquareRandomizer>();
        OnConditionShouldChange();
    }
    private void DelayedApplicationQuit()
    {
        Application.Quit();
    }
    private bool savingHasBegun = false;
    private bool isRecording;

    public bool SaveAllData()
    {
        if (savingHasBegun)
        {
            return false;
        }
        savingHasBegun = true;
        string root = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\XRKeyboardExperimentData";
        
        GetOrCreateDirectory($"{root}");

        var currLargSubDirNr = new DirectoryInfo($"{root}").GetDirectories().Where(e => int.TryParse(e.Name, out _))
                .Select(e => int.Parse(e.Name)).OrderByDescending(e => e).ToArray();
        DirectoryInfo partDirInf = GetOrCreateDirectory(currLargSubDirNr.Length == 0 ? $"{root}\\{0}" : $"{root}\\{int.Parse(currLargSubDirNr.First().ToString())+1}");
        _loggingManager.SetSavePath(partDirInf.FullName);
        _loggingManager.SaveAllLogs(true,TargetType.CSV);
        Invoke(nameof(DelayedApplicationQuit), secondsToWait);
        return false;
    }
    private DirectoryInfo GetOrCreateDirectory(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        return directoryInfo;
    }
    
    public void GenerateLog(Command command,string eventtype, Handedness handedness, Transform backplateTransform)
    {
        loggedData["Character"] = command.Name;
        loggedData["KeyType"] = command.Type.ToString();
        loggedData["Time"] = Time.time - _testStartTime;
        loggedData["Index"] = currentIndex.Value;
        loggedData["Handedness"] = handedness.ToString();
        loggedData["IndexDepth"] = CalculateIndexDepth(handedness, backplateTransform);
        loggedData["EventType"] = eventtype;
    }
}
