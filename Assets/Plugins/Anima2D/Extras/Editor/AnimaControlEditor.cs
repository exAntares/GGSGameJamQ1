using Anima2D;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AnimaControlEditor
{
    private static GameObject current;
    private static List<MonoBehaviour> behaviours;
    private static int s_Index = 0;

    static AnimaControlEditor()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();
        MonoBehaviour control = Selection.activeObject as Control;
        if(control == null)
        {
            var gameobject = Selection.activeObject as GameObject;
            if (gameobject != null)
            {
                control = gameobject.GetComponent<Control>();
                if(control == null)
                {
                    control = gameobject.GetComponent<Ik2D>();
                }
            }
        }

        if (AnimationWindowUtils.get_recording())
        {
            GUI.color = new Color(1, 0, 0, 0.8f);
            GUI.Box(new Rect(0, 0, sceneView.position.width, sceneView.position.height), GUIContent.none, EditorStyles.textArea);
        }

        if (control != null)
        {
            GUI.color = Color.black;
            GUI.Box(new Rect(0, 0, sceneView.position.width, 150), GUIContent.none, EditorStyles.textArea);
            var style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 100;
            GUI.color = Color.white;
            var message = string.Format("{0} ({1})", control.gameObject.name, Tools.current);
            GUI.Label(new Rect(0, 0, 100, sceneView.position.height), message, style);
        }

        Handles.EndGUI();
    }

    [MenuItem("Window/Anima2D/FixIks &#f")]
    static void FixIks()
    {
        var allIks = Object.FindObjectsOfType<IkLimb2D>();
        for (int i = 0; i < allIks.Length; i++)
        {
            var ikLimb = allIks[i];
            ikLimb.enabled = !ikLimb.enabled;
            EditorUpdater.SetDirty();
        }
    }

    [MenuItem("Window/Anima2D/SelectControl #&-")]
    static void SelectPrevControl()
    {
        if (Selection.activeObject == current)
        {
            s_Index--;
            s_Index = s_Index < 0 ? int.MaxValue : s_Index;
        }

        SelectCurrent();
    }

    [MenuItem("Window/Anima2D/SelectControl #&+")]
    static void SelectNextControl()
    {
        if (Selection.activeObject == current)
        {
            s_Index++;
        }

        SelectCurrent();
    }

    static void SelectCurrent()
    {
        behaviours = Object.FindObjectsOfType<Control>().ToList().ConvertAll(x => x as MonoBehaviour);
        behaviours.AddRange(Object.FindObjectsOfType<Ik2D>().ToList().ConvertAll(x => x as MonoBehaviour));
        behaviours.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());
        s_Index = s_Index == int.MaxValue ? behaviours.Count-1 : s_Index;
        if (behaviours.Count > 0)
        {
            s_Index %= behaviours.Count;
            current = behaviours[s_Index].gameObject;
        }

        if (current != null)
        {
            Selection.activeObject = current;
        }
    }

    static void LookAt(Vector3 position)
    {
        var sceneViews = SceneView.sceneViews;
        SceneView sceneView = (SceneView)sceneViews[0];
        if (sceneView != null)
        {
            sceneView.LookAtDirect(position, Quaternion.identity);
        }
    }
}
