using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
[CanEditMultipleObjects]
public class ed_GameManager : Editor
{
    GameManager _owner;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        _owner = target as GameManager;

        if (GUILayout.Button("Reset Player Resources"))
        {
            Debug.Log("Reset Player Resources.");
            
            _owner.Reset_PlayerResources();
        }

        if (GUILayout.Button("Delete PlayerPrefs-BackHomeIndex"))
        {
            Debug.Log("Delete PlayerPrefs-BackHomeIndex");

            _owner.Ed_Delete_PlayerPrefs_BackHomeIndex();
        }
    }

}
