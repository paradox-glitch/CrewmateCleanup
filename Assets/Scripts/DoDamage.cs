using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    public Vector3 box;
    public LayerMask m_LayerMask;

    public Vector3 direction;
    public float distance;

    private void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, box, Quaternion.identity, m_LayerMask);
        if (hitColliders.Length > 0)
        {
            if(
            Physics.ComputePenetration(gameObject.GetComponent<Collider>(), gameObject.transform.position, gameObject.transform.rotation,
                hitColliders[0], hitColliders[0].gameObject.transform.position, hitColliders[0].gameObject.transform.rotation,
                out direction, out distance)
            )
            {
                GameObject.FindGameObjectWithTag("Player").SendMessage("DoDamage");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, box);
    }
}
