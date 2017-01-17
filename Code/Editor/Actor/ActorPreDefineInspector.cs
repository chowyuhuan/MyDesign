using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ActorPreDefine))]
public class ActorPreDefineInspector : Editor
{
    ActorPreDefine _preDefine;
    void OnEnable()
    {
        _preDefine = target as ActorPreDefine;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    public void OnSceneGUI()
    {
        Handles.color = Color.green;

        Handles.DrawLine(new Vector3(_preDefine.XMin, _preDefine.transform.position.y - 10, 0), new Vector3(_preDefine.XMin, _preDefine.transform.position.y + 10, 0));
        Handles.DrawLine(new Vector3(_preDefine.XMax, _preDefine.transform.position.y - 10, 0), new Vector3(_preDefine.XMax, _preDefine.transform.position.y + 10, 0));
    }
}
