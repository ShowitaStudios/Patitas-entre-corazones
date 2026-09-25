using UnityEngine;
using UnityEngine.InputSystem;

public class CamaraTerceraPersona : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;

    [Header("Distancia y Posición")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, -3.5f);
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Rotación con Mouse")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minVerticalAngle = -15f;
    [SerializeField] private float maxVerticalAngle = 60f;

    [Header("Colisión con Paredes")]
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private float cameraRadius = 0.2f;

    private float currentYaw;
    private float currentPitch;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentYaw = angles.y;
        currentPitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            currentYaw += mouseDelta.x * mouseSensitivity * 0.1f;
            currentPitch -= mouseDelta.y * mouseSensitivity * 0.1f;
            currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);
        }

        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 targetPosition = target.position + Vector3.up * offset.y;
        Vector3 desiredPosition = targetPosition + rotation * new Vector3(offset.x, 0f, offset.z);

        Vector3 finalPosition = CheckCameraCollision(targetPosition, desiredPosition);

        transform.position = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * smoothSpeed);
        transform.LookAt(targetPosition);
    }

    private Vector3 CheckCameraCollision(Vector3 fromPosition, Vector3 toPosition)
    {
        RaycastHit hit;
        Vector3 direction = toPosition - fromPosition;
        float distance = direction.magnitude;

        if (Physics.SphereCast(fromPosition, cameraRadius, direction.normalized, out hit, distance, collisionLayers))
        {
            return fromPosition + direction.normalized * (hit.distance - cameraRadius);
        }

        return toPosition;
    }
}