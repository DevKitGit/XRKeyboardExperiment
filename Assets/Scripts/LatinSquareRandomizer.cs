using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LatinSquareRandomizer : MonoBehaviour
{
    private List<int> _orderThisRun;
    private int currentOrderIndex = 0;
    // Start is called before the first frame update
    private void Awake()
    {
        _orderThisRun = GenerateLatinSquareConditionIDs(GetCurrentParticipantID());
    }
    public int? GetNextConditionID()
    {
        return currentOrderIndex >= _orderThisRun.Count ? null : _orderThisRun[currentOrderIndex++];
    }
    public int GetCurrentParticipantID()
    {
        var currLargSubDirNr = new DirectoryInfo($"{root}").GetDirectories().Where(e => int.TryParse(e.Name, out _))
            .Select(e => int.Parse(e.Name)).OrderByDescending(e => e).ToArray();
        return currLargSubDirNr.Length == 0 ? 0 : int.Parse(currLargSubDirNr.First().ToString()) + 1;
    }
    string root = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\XRKeyboardExperimentData";
    
    // Based on "Bradley, J. V. Complete counterbalancing of immediate sequential effects in a Latin square design. J. Amer. Statist. Ass.,.1958, 53, 525-528. "
    public List<int> GenerateLatinSquareConditionIDs(int participantId)
    {
        var result = new List<int>();
        var conditions = new List<int> {1,2,3};
        
        for (int i = 0, j = 0, h = 0; i < conditions.Count; ++i) {
            int val = 0;
            if (i < 2 || i % 2 != 0) {
                val = j++;
            } else {
                val = conditions.Count - h - 1;
                ++h;
            }
            var idx = (val + participantId) % conditions.Count;
            result.Add(conditions[idx]);
        }
        if (conditions.Count % 2 != 0 && participantId % 2 != 0) {
            result.Reverse();
        }
        return result;
    }
}