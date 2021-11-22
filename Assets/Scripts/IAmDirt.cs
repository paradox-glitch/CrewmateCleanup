using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAmDirt : MonoBehaviour
{
    void CleanUp(GameObject a_Player)
    {
        a_Player.SendMessage("SmallDirtCleaned");
        Destroy(this.gameObject);
    }
}
