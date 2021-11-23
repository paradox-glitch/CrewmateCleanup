using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SawBlade : MonoBehaviour
{
    public Vector3 targetPosition, otherTargetPos;
    public bool _useX, _doT1;
    public float speed;

    [SerializeField] private float _target1, _target2;

    private void Start()
    {
        if (_useX)
        {
            _target1 = targetPosition.x;
            _target2 = otherTargetPos.x;
        }
        else
        {
            _target1 = targetPosition.z;
            _target2 = otherTargetPos.z;
        }

    }

    public void Update()
    {
        if(_doT1)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else
            transform.Translate(-Vector2.right * speed * Time.deltaTime);

        if (_useX)
        {
            if (transform.position.x > _target1 && transform.position.x > _target2)
            {
                _doT1 = false;
            }
            else if (transform.position.x < _target1 && transform.position.x < _target2)
            {
                _doT1 = true;
            }
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(SawBlade))]
public class SawBladeEditor : Editor
{
    protected virtual void OnSceneGUI()
    {

        

        SawBlade example = (SawBlade)target;

        if(example._useX)
        {
            example.targetPosition.z = example.gameObject.transform.position.z;
            example.otherTargetPos.z = example.gameObject.transform.position.z;
        }

        float size = HandleUtility.GetHandleSize(example.targetPosition) * 0.5f;
        float sizeOther = HandleUtility.GetHandleSize(example.otherTargetPos) * 0.5f;
        float snap = 0.1f;

        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.Slider(example.targetPosition, example.transform.right, size, Handles.ConeHandleCap, snap);
        Vector3 otherTargetPosition = Handles.Slider(example.otherTargetPos, -example.transform.right, sizeOther, Handles.ConeHandleCap, snap);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(example, "Change Look At Target Position");
            example.targetPosition = newTargetPosition;
            example.otherTargetPos = otherTargetPosition;
        }

        Handles.DrawDottedLine(example.targetPosition, example.otherTargetPos, 6);
    }
}
#endif