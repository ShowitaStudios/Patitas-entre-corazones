using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MascotaController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Slider barraCargaSlider; // Arrastra tu Slider aquí desde el Inspector

    [Header("Movimiento Base")]
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float runSpeed = 6.5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -19.62f;

    [Header("Mecánicas Fantasmas (Nuevas)")]
    [SerializeField] private float maxJumpHeight = 3.0f;
    [SerializeField] private float tiempoMaxCargaSalto = 1.0f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.0f;
    [SerializeField] private float gravedadFlote = -5.0f;
    [SerializeField] private float groundFriction = 10f;
    [SerializeField] private float airFriction = 2.5f;

    private CharacterController controller;
    private MascotaInteract interactScript;
    private Vector3 verticalVelocity;
    private Vector3 horizontalVelocity;
    private bool isGrounded;
    private float currentSpeedMultiplier = 1f;

    private float tiempoCargaSalto = 0f;
    private bool cargandoSalto = false;
    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;
    private bool saliendoDeDash = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        interactScript = GetComponent<MascotaInteract>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Asegurarnos de que la barra esté oculta al iniciar
        if (barraCargaSlider != null)
        {
            barraCargaSlider.gameObject.SetActive(false);
            barraCargaSlider.value = 0f;
        }
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;

        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;

        if (isGrounded)
        {
            saliendoDeDash = false;
            if (verticalVelocity.y < 0)
            {
                verticalVelocity.y = -2f;
            }
        }

        // --- MANEJO DEL DASH ---
        if (isDashing)
        {
            horizontalVelocity = dashDirection * dashSpeed;
            controller.Move(horizontalVelocity * Time.deltaTime);

            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
            {
                isDashing = false;
                saliendoDeDash = true;
            }

            verticalVelocity.y += gravity * Time.deltaTime;
            controller.Move(verticalVelocity * Time.deltaTime);
            return;
        }

        // --- ENTRADA DE MOVIMIENTO ---
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) moveX -= 1f;
            if (Keyboard.current.dKey.isPressed) moveX += 1f;
            if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
            if (Keyboard.current.wKey.isPressed) moveZ += 1f;

            if (Keyboard.current.fKey.wasPressedThisFrame && dashCooldownTimer <= 0f)
            {
                IniciarDash(moveX, moveZ);
            }
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

            Vector3 moveDirection = (camRight * inputDir.x + camForward * inputDir.z).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            bool isCarrying = interactScript != null && interactScript.IsCarryingObject;
            bool isSprinting = !isCarrying && Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
            
            float baseSpeed = isSprinting ? runSpeed : walkSpeed;
            float targetSpeed = baseSpeed * currentSpeedMultiplier;

            Vector3 targetVelocity = moveDirection * targetSpeed;
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetVelocity, 15f * Time.deltaTime);
        }
        else
        {
            float currentFriction = isGrounded ? groundFriction : airFriction;
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, currentFriction * Time.deltaTime);
        }

        controller.Move(horizontalVelocity * Time.deltaTime);

        // --- SALTO CARGADO Y ACTUALIZACIÓN DE BARRA ---
        bool canJump = interactScript == null || !interactScript.IsCarryingObject;
        
        if (canJump && isGrounded && Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                cargandoSalto = true;
                tiempoCargaSalto = 0f;

                // Mostramos y encendemos la barra al empezar a mantener la tecla
                if (barraCargaSlider != null)
                {
                    barraCargaSlider.gameObject.SetActive(true);
                    barraCargaSlider.value = 0f;
                }
            }

            if (Keyboard.current.spaceKey.isPressed && cargandoSalto)
            {
                tiempoCargaSalto += Time.deltaTime;
                tiempoCargaSalto = Mathf.Clamp(tiempoCargaSalto, 0f, tiempoMaxCargaSalto);

                // Actualizamos visualmente el valor del Slider de 0 a 1
                if (barraCargaSlider != null)
                {
                    barraCargaSlider.value = tiempoCargaSalto / tiempoMaxCargaSalto;
                }
            }

            if (Keyboard.current.spaceKey.wasReleasedThisFrame && cargandoSalto)
            {
                cargandoSalto = false;
                float porcentajeCarga = tiempoCargaSalto / tiempoMaxCargaSalto;
                float alturaSaltoActual = Mathf.Lerp(jumpHeight, maxJumpHeight, porcentajeCarga);

                verticalVelocity.y = Mathf.Sqrt(alturaSaltoActual * -2f * gravity);

                // Ocultamos la barra al soltar la tecla y saltar
                if (barraCargaSlider != null)
                {
                    barraCargaSlider.gameObject.SetActive(false);
                }
            }
        }

        // --- GRAVEDAD Y FLOTE ---
        float gravedadAplicada = gravity;
        if (!isGrounded && saliendoDeDash && verticalVelocity.y < 0f)
        {
            gravedadAplicada = gravedadFlote;
        }

        verticalVelocity.y += gravedadAplicada * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void IniciarDash(float moveX, float moveZ)
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;

        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;
        if (inputDir.magnitude >= 0.1f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            dashDirection = (camRight * inputDir.x + camForward * inputDir.z).normalized;
        }
        else
        {
            dashDirection = transform.forward;
        }
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