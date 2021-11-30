using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAmDirt : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_DirtLayerMask;


    void CleanUp(GameObject a_Player)
    {

        Collider[] l_ItemsNearDirt = Physics.OverlapSphere(transform.position, 0.8f, m_DirtLayerMask);
        foreach(Collider a_dirt in l_ItemsNearDirt)
        {
            if (a_dirt.gameObject.tag == "Dirt.SmallJunk")
            {
                a_Player.SendMessage("SmallDirtCleaned");
                a_dirt.SendMessage("CleanMe");
            }
        }


        a_Player.SendMessage("SmallDirtCleaned");
        Destroy(this.gameObject);
    }

    void CleanMe()
    {
        Destroy(this.gameObject);
    }
}
