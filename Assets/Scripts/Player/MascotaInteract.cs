using UnityEngine;
using UnityEngine.InputSystem;

public class MascotaInteract : MonoBehaviour
{
    [Header("Detección e Interacción")]
    [SerializeField] private float interactDistance = 1.8f;
    [SerializeField] private float raycastHeightOffset = 0.3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform holdPoint;

    [Header("Empuje Físico")]
    [SerializeField] private float pushPower = 5.0f;

    private Rigidbody grabbedObject;
    private Transform originalParent;
    private MascotaController controller;

    public bool IsCarryingObject => grabbedObject != null;
    public float CurrentObjectMass => grabbedObject != null ? grabbedObject.mass : 0f;

    private void Start()
    {
        controller = GetComponent<MascotaController>();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (grabbedObject == null)
            {
                TryGrabObject();
            }
            else
            {
                ReleaseObject();
            }
        }
    }

    private void TryGrabObject()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * raycastHeightOffset;

        if (Physics.Raycast(rayOrigin, transform.forward, out hit, interactDistance, interactableLayer))
        {
            if (hit.collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                grabbedObject = rb;
                originalParent = grabbedObject.transform.parent;

                grabbedObject.isKinematic = true;

                grabbedObject.transform.SetParent(holdPoint);
                grabbedObject.transform.position = holdPoint.position;
            }
        }
    }

    private void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.transform.SetParent(originalParent);
            grabbedObject.isKinematic = false;
            grabbedObject = null;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic) return;
        if (hit.moveDirection.y < -0.3f) return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        
        float finalPush = pushPower / Mathf.Max(body.mass, 0.1f);
        body.linearVelocity = pushDir * finalPush;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 rayOrigin = transform.position + Vector3.up * raycastHeightOffset;
        Gizmos.DrawRay(rayOrigin, transform.forward * interactDistance);
    }
}