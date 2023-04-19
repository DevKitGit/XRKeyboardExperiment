using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Devkit.Modularis.References;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

public class ExperimentManager : MonoBehaviour
{
    [SerializeField] private IntReference currentIndex;
    [SerializeField] private  StringReference promptString;
    private LoggingManager _loggingManager;
    private string logName = "KeyboardData";
    private bool _testStarted;
    private float _testStartTime;
    public int secondsToWait;

    private int _previousIndex;

    private Dictionary<string, object> loggedData = new()
    {
        {"Head.Pos.X",null},
        {"Head.Pos.Y",null},
        {"Head.Pos.Z",null},

        {"Head.Rot.X",null},
        {"Head.Rot.Y",null},
        {"Head.Rot.Z",null},
        {"Head.Rot.W",null},

        {"Character",null},
        {"Time",null},
        {"Index",null}, 
        {"EventType",null}
    };
    public void OnButtonTouchBegin(Command command)
    {
        GenerateLog(command);
        loggedData["EventType"]= "TouchBegin";
        
        _loggingManager.Log(logName, loggedData);
    }
    public void OnButtonTouchEnd(Command command)
    {
        GenerateLog(command);
        loggedData["EventType"]= "TouchEnd";
        _loggingManager.Log(logName, loggedData);
    }
    public void OnButtonPressBegin(Command command)
    {
        GenerateLog(command);
        loggedData["EventType"]= "PressBegin";
        _loggingManager.Log(logName, loggedData);
    }
    public void OnButtonPressEnd(Command command)
    {
        GenerateLog(command);
        loggedData["EventType"]= "PressEnd";
        _loggingManager.Log(logName, loggedData);
    }

    private bool previouslyWrong = false;
    public void OnButtonClicked(Command command)
    {
        bool success = false;
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
                success = promptString.Value[currentIndex.Value].ToString() == " ";
                currentIndex.Value += 1;
                break;
            case Command.KeyType.Character:
                if (previouslyWrong)
                {
                    break;
                }
                success = promptString.Value[currentIndex.Value].ToString().ToUpper() == command.Name;
                currentIndex.Value += 1;
                break;
        }
        FindObjectOfType<TextUpdater>().UpdateText(command, success);
        previouslyWrong = !success;
        GenerateLog(command);
        loggedData["EventType"]= "Clicked";
        _loggingManager.Log(logName, loggedData);
        
    }

    private void Start()
    {
        _loggingManager = FindObjectOfType<LoggingManager>();
        Application.wantsToQuit += SaveAllData;
    }
    private void DelayedApplicationQuit()
    {
        Application.wantsToQuit -= SaveAllData;
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
    
    public void GenerateLog(Command command)
    {
        print(command.Name);
        loggedData["Character"] = command.Name;
        loggedData["KeyType"] = command.Type.ToString();
        loggedData["Time"] = Time.time - _testStartTime;
        loggedData["Index"] = currentIndex.Value;
        
        loggedData["Head.Pos.X"] = CameraCache.Main.transform.position.x;
        loggedData["Head.Pos.Y"] = CameraCache.Main.transform.position.y;
        loggedData["Head.Pos.Z"] = CameraCache.Main.transform.position.z;
        
        loggedData["Head.Rot.X"] = CameraCache.Main.transform.rotation.x;
        loggedData["Head.Rot.Y"] = CameraCache.Main.transform.rotation.y;
        loggedData["Head.Rot.Z"] = CameraCache.Main.transform.rotation.z;
        loggedData["Head.Rot.W"] = CameraCache.Main.transform.rotation.w;
    }
}
