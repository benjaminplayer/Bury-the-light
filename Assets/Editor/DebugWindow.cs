using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugWindow : EditorWindow
{

    private bool dinamicUpdate = true;

    public List<IDebugItem> values = DebugItemRegistry.allitems;

    [MenuItem("Window/Custom Debug Window")]
    public static void ShowWindow()
    {
        GetWindow<DebugWindow>("DebugValues");
    }

    private void OnGUI()
    {
        GUILayout.Label("Enable dinamic updating",EditorStyles.boldLabel);
        dinamicUpdate = EditorGUILayout.Toggle(new GUIContent("Dynamic Update", "Automatically refreshes the debug objects"), dinamicUpdate);
        GUILayout.Space(10);

        GUILayout.Label("Debug values", EditorStyles.boldLabel);

        if (values.Count == 0)
        {
            GUILayout.Label("No debug data yet!");
        }


        foreach (IDebugItem item in values)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(item.GetItemTitle(), item.ValueToString());
            GUILayout.EndHorizontal();
        }
    }
    private void Update()
    {
        if (dinamicUpdate)
        { 
            DebugItemRegistry.allitems.Clear(); // clear starih
            values = DebugItemRegistry.allitems; // dinamicly update values
            Repaint();
        }
    }

}
