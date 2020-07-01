using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TacticalButton))]
public class LocationRememberer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TacticalButton button = (TacticalButton)target;
        if(GUILayout.Button("Remember position"))
        {
            button.InterfacePosition = button.transform.localPosition;
        }
    }

}
