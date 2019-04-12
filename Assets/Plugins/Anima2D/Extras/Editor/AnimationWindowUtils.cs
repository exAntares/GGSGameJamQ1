using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public static class AnimationWindowUtils 
{
    static readonly BindingFlags _flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
    static readonly Type animationWindowType = Type.GetType("UnityEditor.AnimationWindow, UnityEditor");
    static readonly FieldInfo _animEditorFieldInfo = animationWindowType.GetField("m_AnimEditor", _flags);
    static readonly Type _animEditorType = _animEditorFieldInfo.FieldType;
    static readonly FieldInfo _animWindowStateFieldInfo = _animEditorType.GetField("m_State", _flags);
    static readonly Type _windowStateType = _animWindowStateFieldInfo.FieldType;

    static UnityEngine.Object _window { get { return GetOpenAnimationWindow(); } }
    static System.Object _animEditorObject { get { return _animEditorFieldInfo.GetValue(_window); } }

    public static UnityEngine.Object GetOpenAnimationWindow()
    {
        UnityEngine.Object[] openAnimationWindows = Resources.FindObjectsOfTypeAll(animationWindowType);
        if (openAnimationWindows.Length > 0)
        {
            return openAnimationWindows[0];
        }
        return null;
    }
 
    public static void RepaintOpenAnimationWindow()
    {
        if (_window != null)
        {
            (_window as EditorWindow).Repaint();
        }
    }

    public static void PrintMethods()
    {
        if (_window != null)
        {
            Debug.Log("Methods");
            MethodInfo[] methods = _windowStateType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            Debug.Log("Methods : " + methods.Length);
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo currentInfo = methods[i];
                Debug.Log(currentInfo.ToString());
            }
        }
    }

    static void StartRecording()
    {
        if (_window != null)
        {
            var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
            _windowStateType.InvokeMember("StartRecording", flags, null, _animWindowStateFieldInfo.GetValue(_animEditorObject), new object[0] { });
        }
    }

    static void StopRecording()
    {
        if (_window != null)
        {
            var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
            _windowStateType.InvokeMember("StopRecording", flags, null, _animWindowStateFieldInfo.GetValue(_animEditorObject), new object[0] { });
        }
    }

    public static bool get_previewing()
    {
        if (_window != null)
        {
            var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
            return (bool)_windowStateType.InvokeMember("get_previewing", flags, null, _animWindowStateFieldInfo.GetValue(_animEditorObject), new object[0] { });
        }

        return false;
    }

    public static bool get_recording()
    {
        if (_window != null)
        {
            var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
            return (bool)_windowStateType.InvokeMember("get_recording", flags, null, _animWindowStateFieldInfo.GetValue(_animEditorObject), new object[0] { });
        }

        return false;
    }

    public static void ToggleRecord()
    {
        if (EditorApplication.isPlaying)
            return;

        if (get_recording())
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }
}
