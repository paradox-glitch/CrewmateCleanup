using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFinish : MonoBehaviour
{
    public GameObject TutManager;
    void TriggerInteraction()
    {
        TutManager.SendMessage("TryFinish");
    }
}