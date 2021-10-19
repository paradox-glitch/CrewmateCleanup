
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public CharacterController _characterController;

    public float _speed = 6f, _handInteractRadius, _playerInteractRadius;

    public Vector3 _directionVector, _angledDirectionVector, handOffset;

    public Collider[] _hitColliders;

    private Ray _mouseRay;

    private RaycastHit _mouseRayHit;

    public LayerMask _itemInteractLayerMask;
    enum HandItem { Hand, MegaPhone, Brush, CattleProd }

    HandItem myHandItem = HandItem.Hand;

    public GameObject brushPrefab, cattleProdPrefab, megaPhonePrefab;

    // Update is called once per frame
    void Update()
    {
        if (_directionVector.magnitude != 0)
        {
            _angledDirectionVector = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * _directionVector;
            _characterController.Move(_angledDirectionVector * _speed * Time.deltaTime);
        }
    }

    void OnLook()
    {
        _mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(_mouseRay, out _mouseRayHit, 50f))
        {
            Vector3 _mouseRayHitFixed = new Vector3(_mouseRayHit.point.x, transform.position.y, _mouseRayHit.point.z);
            transform.LookAt(_mouseRayHitFixed);
        }
    }

    void OnFire()
    {
        GameObject interactionObjet = GetClosest(CheckBoth(Physics.OverlapSphere(_mouseRayHit.point, _handInteractRadius, _itemInteractLayerMask), Physics.OverlapSphere(transform.position, _playerInteractRadius, _itemInteractLayerMask)));

        if (!interactionObjet)
            return;

        if (interactionObjet.layer == 6)
            Pickup(interactionObjet);
        else if (interactionObjet.layer == 7)
            CleanDirt(interactionObjet);
    }


    //* Checks that the collider is in both arrays
    Collider[] CheckBoth(Collider[] array1, Collider[] array2)
    {
        var intersect = array1.Intersect(array2);

        return intersect.ToArray();
    }


    GameObject GetClosest(Collider[] objects)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Collider potentialTarget in objects)
        {
            Vector3 directionToTarget = potentialTarget.gameObject.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.gameObject;
            }
        }
        return bestTarget;
    }

    void Pickup(GameObject interaction)
    {
        //if (myHandItem != HandItem.Hand)
        //    return;
        //else
        //{
            if (interaction.CompareTag("Brush"))
            {
                myHandItem = HandItem.Brush;
            }
            else if (interaction.CompareTag("CattleProd"))
            {
                myHandItem = HandItem.CattleProd;
            }
            else if (interaction.CompareTag("MegaPhone"))
            {
                myHandItem = HandItem.MegaPhone;
            }

            RenderHandObject(brushPrefab);
            Destroy(interaction);
        //}
    }

    void RenderHandObject(GameObject item)
    {
        GameObject renderedHandItem = Instantiate(item, transform.position + handOffset, transform.rotation);

        Destroy(renderedHandItem.GetComponent<Rigidbody>());
        Destroy(renderedHandItem.GetComponent<BoxCollider>());

        Debug.Log("renderhand" + myHandItem);
    }

    void CleanDirt(GameObject interaction)
    {
        if(interaction.CompareTag("Packets") && myHandItem == HandItem.Hand)
        {

        }
        else if(interaction.CompareTag("Kids") && myHandItem == HandItem.MegaPhone)
        {

        }
        else if (interaction.CompareTag("Dirt") && myHandItem == HandItem.Brush)
        {

        }
        else if (interaction.CompareTag("Rat") && myHandItem == HandItem.CattleProd)
        {

        }
    }

    void OnMove(InputValue _inputValue)
    {
        Vector2 _inputVector = _inputValue.Get<Vector2>();
        _directionVector = new Vector3(_inputVector.x, 0, _inputVector.y).normalized;
    }
}