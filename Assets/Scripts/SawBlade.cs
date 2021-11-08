using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SawBlade : MonoBehaviour
{
    public Vector3 targetPosition { get { return m_TargetPosition; } set { m_TargetPosition = value; } }
    [SerializeField]
    private Vector3 m_TargetPosition = new Vector3(1f, 0f, 2f);

    public virtual void Update()
    {
        transform.LookAt(m_TargetPosition);
    }
}

[CustomEditor(typeof(SawBlade))]
public class SawBladeEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        SawBlade example = (SawBlade)target;

        float size = HandleUtility.GetHandleSize(example.targetPosition) * 0.5f;
        float snap = 0.1f;

        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.Slider(example.targetPosition, example.transform.right, size, Handles.ConeHandleCap, snap);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(example, "Change Look At Target Position");
            example.targetPosition = newTargetPosition;
            example.Update();
        }
    }
}
