using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using ColorUtility = UnityEngine.ColorUtility;

public static class Extensions
{
    public static void Scale(this Vector3[] vector3s, Vector3 scale)
    {
        for (int i = 0; i < vector3s.Length; i++)
        {
            vector3s[i] = Vector3.Scale(vector3s[i], scale);
        }
    }
    public static void Center(this Vector3[] vector3s, Vector3 center)
    {
        for (int i = 0; i < vector3s.Length; i++)
        {
            vector3s[i] += center;
        }
    }
  
}