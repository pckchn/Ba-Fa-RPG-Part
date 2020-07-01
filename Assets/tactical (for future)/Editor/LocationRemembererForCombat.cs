using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CombatButton))]
public class LocationRemembererForCombat : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CombatButton button = (CombatButton)target;
        if (GUILayout.Button("Remember position"))
        {
            button.InterfacePosition = button.transform.localPosition;
        }

    }
}
