using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class ObjetoPuzzle : MonoBehaviour
{
    [Header("Identificador del Puzzle")]
    [Tooltip("El nombre exacto registrado en el PuzzleManager")]
    [SerializeField] private string puzzleID;

    [Header("Referencias y Configuración")]
    [SerializeField] private PuzzleManager puzzleManager;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private float raycastHeightOffset = 0.3f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Mecánica de Forcejeo")]
    [Tooltip("Si está activo, requiere presionar varias veces la tecla E. Si está apagado, se abre con una sola pulsación.")]
    [SerializeField] private bool usarForcejeo = true;
    [SerializeField] private int vecesNecesariasParaAbrir = 5;
    [SerializeField] private float tiempoMaxEntrePulsaciones = 1.0f;

    [Header("Feedback Visual (Shake)")]
    [SerializeField] private float shakeIntensity = 0.05f;
    [SerializeField] private float shakeSpeed = 40f;

    [Header("sfx")]
    [SerializeField] private StudioEventEmitter sfxOpenBox;
    [SerializeField] private StudioEventEmitter sfxRascar;

    private int pulsacionesActuales = 0;
    private float tiempoUltimaPulsacion = 0f;
    private Vector3 posicionOriginal;
    private bool estaTemblando = false;

    private void Awake()
    {
        if (puzzleManager == null)
        {
            BuscarYAsignarPuzzleManager();
        }
        posicionOriginal = transform.localPosition;
    }

    private void Reset()
    {
        BuscarYAsignarPuzzleManager();
    }

    public void BuscarYAsignarPuzzleManager()
    {
#if UNITY_2023_1_OR_NEWER
        puzzleManager = Object.FindFirstObjectByType<PuzzleManager>();
#else
        puzzleManager = Object.FindObjectOfType<PuzzleManager>();
#endif

        if (puzzleManager == null)
        {
            Debug.LogWarning($"[ObjetoPuzzle] No se encontró ningún PuzzleManager en la escena para '{gameObject.name}'.", this);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (IsPlayerNearby() && puzzleManager != null)
        {
            if (usarForcejeo)
            {
                // Controlar el tiempo límite entre pulsaciones
                if (pulsacionesActuales > 0 && Time.time - tiempoUltimaPulsacion > tiempoMaxEntrePulsaciones)
                {
                    ResetearForcejeo();
                }
                // SFX de rascar aqui
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    EjecutarForcejeo();
                    sfxRascar?.Play();
                }
            }
            else
            {
                // Comportamiento clásico de un solo toque
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    puzzleManager.AbrirPuzzlePorNombre(puzzleID);
                }
            }
        }
        else if (pulsacionesActuales > 0 && !IsPlayerNearby())
        {
            // Si el jugador se aleja, se reinicia el progreso del forcejeo
            ResetearForcejeo();
        }

        // Manejar el movimiento de temblor (feedback visual)
        if (estaTemblando)
        {
            float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
            float offsetZ = Mathf.Cos(Time.time * shakeSpeed * 1.2f) * shakeIntensity;
            transform.localPosition = posicionOriginal + new Vector3(offsetX, 0f, offsetZ);
        }
    }

    private void EjecutarForcejeo()
    {
        pulsacionesActuales++;
        tiempoUltimaPulsacion = Time.time;
        estaTemblando = true;

        if (pulsacionesActuales >= vecesNecesariasParaAbrir)
        {
            sfxOpenBox?.Play();
            ResetearForcejeo();
            puzzleManager.AbrirPuzzlePorNombre(puzzleID);
        }
    }

    private void ResetearForcejeo()
    {
        pulsacionesActuales = 0;
        estaTemblando = false;
        transform.localPosition = posicionOriginal;
    }

    private bool IsPlayerNearby()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * raycastHeightOffset;
        Collider[] hitColliders = Physics.OverlapSphere(rayOrigin, interactDistance, playerLayer);
        return hitColliders.Length > 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 rayOrigin = transform.position + Vector3.up * raycastHeightOffset;
        Gizmos.DrawWireSphere(rayOrigin, interactDistance);
    }
}