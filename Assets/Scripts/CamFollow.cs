using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform _playerTransform;
    private Vector3 _offset;

    void Start()
    {
        _offset = _playerTransform.position + this.transform.position;
    }

    private void LateUpdate()
    {
        this.transform.position = _playerTransform.position + _offset;
    }
}
