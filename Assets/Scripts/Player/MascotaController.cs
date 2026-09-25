using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MascotaController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movimiento Base")]
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float runSpeed = 6.5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -19.62f;

    private CharacterController controller;
    private MascotaInteract interactScript;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeedMultiplier = 1f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        interactScript = GetComponent<MascotaInteract>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) moveX -= 1f;
            if (Keyboard.current.dKey.isPressed) moveX += 1f;
            if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
            if (Keyboard.current.wKey.isPressed) moveZ += 1f;
        }

        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = camRight * inputDir.x + camForward * inputDir.z;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            bool isCarrying = interactScript != null && interactScript.IsCarryingObject;
            bool isSprinting = !isCarrying && Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
            
            float baseSpeed = isSprinting ? runSpeed : walkSpeed;
            float finalSpeed = baseSpeed * currentSpeedMultiplier;

            controller.Move(moveDirection * finalSpeed * Time.deltaTime);
        }

        bool canJump = interactScript == null || !interactScript.IsCarryingObject;
        if (canJump && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void SetSpeedMultiplier(float factor)
    {
        currentSpeedMultiplier = Mathf.Clamp(factor, 0.1f, 1f);
    }

    public void ResetSpeedMultiplier()
    {
        currentSpeedMultiplier = 1f;
    }
}