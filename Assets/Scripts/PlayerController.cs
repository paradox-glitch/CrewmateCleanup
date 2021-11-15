
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;

public class PlayerController : MonoBehaviour
{
    private CharacterController m_CharacterController;

    [SerializeField]
    private float
        m_PlayerMovementSpeed = 6f,
        m_HandInteractRadius,
        m_PlayerReachRadius;

    private Vector3
        m_InputVector,
        m_LookAtPosition;

    private RaycastHit m_MouseRayHit;

    [SerializeField]
    private LayerMask
        m_PickUpLayerMask,
        m_DirtLayerMask,
        m_DrawRayLayerMask;
    
    [SerializeField]
    private m_HandItems m_CurrentHandItem = m_HandItems.Hand;

    [SerializeField]
    private bool m_LocalizeMovement = false;

    private bool m_InputEnabled = true;

    private int m_PlayerHealth = 3;

    [SerializeField]
    private GameObject
        m_DeadCrewmateLower,
        m_DeadCrewmateUpper,
        m_BloodSplaterLauncher;

    private GameObject
        m_SplatDecalPool,
        m_HandPosition,
        m_LevelManager,
        m_DropDecalPool;

    private ConfigurableJoint m_ItemJoint;

    enum m_HandItems { Hand, Mop, MegaPhone, Brush, CattleProd, Gloves }

    private void Start()
    {
        m_ItemJoint = transform.GetChild(2).transform.GetComponent<ConfigurableJoint>();
        m_HandPosition = transform.GetChild(1).gameObject;
        m_CharacterController = GetComponent<CharacterController>();
        m_DropDecalPool = GameObject.FindGameObjectWithTag("DropDecalPool");
        m_SplatDecalPool = GameObject.FindGameObjectWithTag("SpaltDecalPool");
        m_LevelManager = GameObject.FindGameObjectWithTag("Manager.Level");
    }


    void Update()
    {
        if (m_InputEnabled)
            PlayerCycle();
    }

    void PlayerCycle()
    {
        Quaternion l_TargetRotation = Quaternion.LookRotation(m_LookAtPosition);
        float l_TargetEualarY = Mathf.LerpAngle(transform.eulerAngles.y, l_TargetRotation.eulerAngles.y, Time.deltaTime * 10);
        Vector3 l_TargetEular = new Vector3(0f, l_TargetEualarY, 0f);
        l_TargetRotation = Quaternion.Euler(l_TargetEular);
        transform.rotation = l_TargetRotation;

        PlayerMove();
    }

    void PlayerMove()
    {
        if (m_InputVector.magnitude == 0)
            return;

        Vector3 l_angledDirectionVector;

        if (m_LocalizeMovement)
            l_angledDirectionVector = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * m_InputVector;
        else
            l_angledDirectionVector = Quaternion.AngleAxis(-45, Vector3.up) * m_InputVector;


        m_CharacterController.Move(l_angledDirectionVector * m_PlayerMovementSpeed * Time.deltaTime);

        
    }

    //* Called by UnityEngine.InputSystem when the mouse is moved
    void OnLook()
    {
        Ray l_MouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(l_MouseRay, out m_MouseRayHit, 50f, m_DrawRayLayerMask))
        {
            Vector3 l_MouseRayHitFixed = new Vector3(m_MouseRayHit.point.x, transform.position.y, m_MouseRayHit.point.z);

            Debug.DrawLine(transform.position, l_MouseRayHitFixed, Color.green);

            float l_Distance = Vector3.Distance(transform.position, l_MouseRayHitFixed);

            if (l_Distance > 0.1f)
                m_LookAtPosition = l_MouseRayHitFixed;
                m_LookAtPosition = m_LookAtPosition - transform.position;
        }
    }

    //* Called by UnityEngine.InputSystem when the LMB is pressed
    void OnFire()
    {
        switch (m_CurrentHandItem)
        {
            case m_HandItems.Hand:
                Pickup();
                break;
            case m_HandItems.Mop:
                DoMop();
                break;
            case m_HandItems.Brush:
                try { InteractionObject(m_DirtLayerMask, "Dirt.SmallDirt").SendMessage("CleanUp"); }
                catch(Exception e) { Debug.Log("NoItemFound"); }
                break;
            case m_HandItems.CattleProd:
                try { InteractionObject(m_DirtLayerMask, "Dirt.Rat").SendMessage("CleanUp"); }
                catch (Exception e) { Debug.Log("NoItemFound"); }
                break;
            case m_HandItems.MegaPhone:
                try { InteractionObject(m_DirtLayerMask, "Dirt.Kids").SendMessage("CleanUp"); }
                catch (Exception e) { Debug.Log("NoItemFound"); }
                break;
            case m_HandItems.Gloves:
                DoBodyDrag();
                break;
            default:
                break;
        }

        m_LevelManager.GetComponent<LevelManager>().SendMessage("DoCheck");
    }

    void DoBodyDrag(bool a_ForceDrop = false)
    {
        if((a_ForceDrop && m_ItemJoint.connectedBody != null) || m_ItemJoint.connectedBody != null)
        {
            m_ItemJoint.connectedBody.gameObject.layer = 7;
            m_ItemJoint.connectedBody = null;
        }
        else if (m_ItemJoint.connectedBody == null)
        {
            GameObject l_Body = InteractionObject(m_DirtLayerMask, "Dirt.DeadBody");
            if (l_Body == null)
                return;
            m_ItemJoint.connectedBody = l_Body.transform.GetComponent<Rigidbody>();
            l_Body.gameObject.transform.position = m_ItemJoint.gameObject.transform.position;
            l_Body.layer = 9;
        }

    }

    void Pickup()
    {
        GameObject l_InteractionObject = InteractionObject(m_PickUpLayerMask);

        if (l_InteractionObject == null)
            return;

        if (l_InteractionObject.CompareTag("Tool.Brush"))
        {
            m_CurrentHandItem = m_HandItems.Brush;
        }
        else if (l_InteractionObject.CompareTag("Tool.CattleProd"))
        {

            m_CurrentHandItem = m_HandItems.CattleProd;
        }
        else if (l_InteractionObject.CompareTag("Tool.MegaPhone"))
        {
            m_CurrentHandItem = m_HandItems.MegaPhone;
        }
        else if (l_InteractionObject.CompareTag("Pickup.Mop"))
        {
            m_CurrentHandItem = m_HandItems.Mop;
        }
        else if (l_InteractionObject.CompareTag("Tool.Gloves"))
        {
            m_CurrentHandItem = m_HandItems.Gloves;
        }




        Destroy(l_InteractionObject.GetComponent<Rigidbody>());
        l_InteractionObject.GetComponent<Collider>().enabled = false;

        l_InteractionObject.transform.parent = m_HandPosition.transform;
        l_InteractionObject.transform.position = m_HandPosition.transform.position;
        l_InteractionObject.transform.rotation = m_HandPosition.transform.rotation;
    }

    void OnDrop()
    {
        DoDrop();
    }

    void DoDrop()
    {
        if (m_CurrentHandItem == m_HandItems.Hand)
            return;
        else if (m_CurrentHandItem == m_HandItems.Gloves)
        {
            DoBodyDrag(true);
        }
            

        m_HandPosition.transform.GetChild(0).gameObject.AddComponent<Rigidbody>();
        m_HandPosition.transform.GetChild(0).GetComponent<Collider>().enabled = true;

        m_HandPosition.transform.DetachChildren();
        m_CurrentHandItem = m_HandItems.Hand;
    }

    //* Gets the closets item that is reachable by the player and near the mouse position
    GameObject InteractionObject(LayerMask a_Layer, string a_tag = null)
    {
        Collider[] l_ItemsNearHand = Physics.OverlapSphere(m_MouseRayHit.point, m_HandInteractRadius, a_Layer);
        Collider[] l_ItemsNearBody = Physics.OverlapSphere(transform.position, m_PlayerReachRadius, a_Layer);

        Collider[] l_ItemsNearBoth = CheckBoth(l_ItemsNearBody, l_ItemsNearHand);

        GameObject l_ClosestItem = GetClosest(l_ItemsNearBoth);

        return l_ClosestItem;
    }

    void DoMop()
    {
        float l_Distance = Vector3.Distance(transform.position, m_MouseRayHit.point);

        if (l_Distance > 2)
            return;

        m_SplatDecalPool.SendMessage("TryClean", m_MouseRayHit.point);
        m_DropDecalPool.SendMessage("TryClean", m_MouseRayHit.point);

        return;
    }

    //* Checks that the collider is in both arrays
    Collider[] CheckBoth(Collider[] a_Array1, Collider[] a_Array2)
    {
        var l_Intersect = a_Array1.Intersect(a_Array2);

        return l_Intersect.ToArray();
    }

    //* Gets the closets item to the player
    GameObject GetClosest(Collider[] a_Objects, string a_tag = null)
    {
        GameObject l_BestTarget = null;
        float l_ClosestDistance = Mathf.Infinity;
        foreach (Collider a_PotentialTarget in a_Objects)
        {
            if (a_tag == null || a_tag == a_PotentialTarget.tag)
            {
                float l_Distance = Vector3.Distance(a_PotentialTarget.transform.position, m_MouseRayHit.point);
                if (l_Distance < l_ClosestDistance)
                {
                    l_ClosestDistance = l_Distance;
                    l_BestTarget = a_PotentialTarget.gameObject;
                }
            }
        }
        return l_BestTarget;
    }

    void CleanDirt()
    {

    }

    void DoDamage()
    {
        m_PlayerHealth--;
        m_CharacterController.enabled = false;
        m_InputEnabled = false;
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        DoDrop();
        Instantiate(m_DeadCrewmateLower, gameObject.transform.position, transform.rotation * Quaternion.Euler(m_DeadCrewmateLower.transform.rotation.eulerAngles));
        Instantiate(m_DeadCrewmateUpper, gameObject.transform.position, transform.rotation * Quaternion.Euler(m_DeadCrewmateUpper.transform.rotation.eulerAngles));
        Instantiate(m_BloodSplaterLauncher, gameObject.transform.position, gameObject.transform.rotation);

        if (m_PlayerHealth > 0)
        {
            StartCoroutine(WaitToRespawn());
        }
        else if (m_PlayerHealth <= 0)
        {

        }
    }

    IEnumerator WaitToRespawn()
    {
        yield return new WaitForSeconds(1.2f);
        transform.position = m_LevelManager.transform.GetChild(0).position;

        m_CharacterController.enabled = true;
        m_InputEnabled = true;
        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    void OnMove(InputValue a_InputValue)
    {
        Vector2 l_InputVector = a_InputValue.Get<Vector2>();
        m_InputVector = new Vector3(l_InputVector.x, 0, l_InputVector.y).normalized;
    }
}