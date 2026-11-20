using UnityEngine;
using UnityEngine.InputSystem;

public class CamaraTerceraPersona : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;

    [Header("Distancia y Posición")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, -3.5f);
    [SerializeField] private float smoothSpeed = 12f;

    [Header("Rotación con Mouse")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minVerticalAngle = -15f;
    [SerializeField] private float maxVerticalAngle = 60f;

    [Header("Atravesar Paredes / Colisión")]
    [Tooltip("Déjalo desactivado para atravesar paredes sin recortes ni saltos de rotación.")]
    [SerializeField] private bool bloquearConParedes = false;
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private float cameraRadius = 0.2f;
    [SerializeField] private float minDistanceFromTarget = 0.8f;

    private float currentYaw;
    private float currentPitch;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentYaw = angles.y;

        // Convierte ángulos > 180 a rango negativo (-180..180) para que el Clamp no salte
        currentPitch = angles.x > 180f ? angles.x - 360f : angles.x;
        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);

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

        Vector3 finalPosition = bloquearConParedes
            ? CheckCameraCollision(targetPosition, desiredPosition)
            : desiredPosition;

        transform.position = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * smoothSpeed);
        transform.LookAt(targetPosition);
    }

    private Vector3 CheckCameraCollision(Vector3 fromPosition, Vector3 toPosition)
    {
        Vector3 direction = toPosition - fromPosition;
        float distance = direction.magnitude;
        if (distance <= 0.0001f) return toPosition;

        Vector3 dirNormalized = direction / distance;

        if (Physics.SphereCast(fromPosition, cameraRadius, dirNormalized, out RaycastHit hit, distance, collisionLayers, QueryTriggerInteraction.Ignore))
        {
            float safeDistance = Mathf.Clamp(hit.distance - cameraRadius, minDistanceFromTarget, distance);
            return fromPosition + dirNormalized * safeDistance;
        }

        return toPosition;
    }
}