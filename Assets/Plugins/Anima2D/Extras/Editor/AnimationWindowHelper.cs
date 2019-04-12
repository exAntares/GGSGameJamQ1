using UnityEngine;
using UnityEditor;

public class AnimationWindowHelper : EditorWindow
{
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Animation/Helper")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        var window = (AnimationWindowHelper)EditorWindow.GetWindow(typeof(AnimationWindowHelper));
        window.Show();
    }

    void OnGUI()
    {
        if(GUILayout.Button("PrintMethods"))
            AnimationWindowUtils.PrintMethods();

        if (GUILayout.Button("ToggleRecord"))
            AnimationWindowUtils.ToggleRecord();

        if (GUILayout.Button("get_previewing"))
            Debug.Log(AnimationWindowUtils.get_previewing());

        if (GUILayout.Button("get_recording"))
            Debug.Log(AnimationWindowUtils.get_recording());
    }
}
