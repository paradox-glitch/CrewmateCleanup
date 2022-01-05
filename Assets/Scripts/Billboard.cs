using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    public Camera m_MainCamera;

    // Start is called before the first frame update
    void Start()
    {
        m_MainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //this.gameObject.transform.LookAt(new Vector3(-m_MainCamera.transform.position.x, this.gameObject.transform.position.y, -m_MainCamera.transform.position.z));
        
        Vector3 relativePos = m_MainCamera.transform.position - transform.position;

        relativePos = new Vector3(-relativePos.x, 0, -relativePos.z);

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = rotation;
    }
}
