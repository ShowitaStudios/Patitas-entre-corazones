using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BillboardTextoZona : MonoBehaviour
{
    [Header("Referencias (Opcionales - Auto-asignación)")]
    [Tooltip("Texto TMP (3D o en Canvas World Space). Si queda vacío, se busca en hijos, objeto o escena.")]
    [SerializeField] private TMP_Text textoBillboard;
    [Tooltip("Transform donde se posicionará el texto flotante. Si queda vacío, se busca automáticamente.")]
    [SerializeField] private Transform puntoPosicionTexto;
    [Tooltip("Cámara principal o de tercera persona. Si queda vacío, se busca automáticamente.")]
    [SerializeField] private Camera camara;

    [Header("Contenido y Visibilidad")]
    [TextArea(2, 4)]
    [SerializeField] private string mensaje = "Interactuar [E]";
    [SerializeField] private bool ocultarAlIniciar = true;

    [Header("Detección y Seguimiento")]
    [SerializeField] private string tagJugador = "Player";
    [Tooltip("Suavizado de posición (0 = instantáneo).")]
    [SerializeField] private float suavizadoPosicion = 0f;
    [Tooltip("Si está activo, el texto copia la rotación de la cámara para mantenerse siempre de frente sin espejo.")]
    [SerializeField] private bool alinearConCamara = true;

    private bool jugadorDentro = false;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }

        ResolverReferencias();

        if (textoBillboard != null)
        {
            textoBillboard.text = mensaje;
            textoBillboard.gameObject.SetActive(!ocultarAlIniciar);
        }
        else
        {
            Debug.LogWarning($"[BillboardTextoZona] No se encontró ningún TMP_Text en '{name}' ni en la escena.");
        }
    }

    private void OnValidate()
    {
        TMP_Text texto = textoBillboard != null ? textoBillboard : GetComponentInChildren<TMP_Text>(true);
        if (texto != null && !Application.isPlaying)
        {
            texto.text = mensaje;
        }
    }

    private void ResolverReferencias()
    {
        if (textoBillboard == null)
        {
            textoBillboard = GetComponentInChildren<TMP_Text>(true);
        }

        if (textoBillboard == null)
        {
            textoBillboard = GetComponent<TMP_Text>();
        }

        if (textoBillboard == null)
        {
#if UNITY_2023_1_OR_NEWER
            textoBillboard = Object.FindFirstObjectByType<TMP_Text>(FindObjectsInactive.Include);
#else
            textoBillboard = Object.FindObjectOfType<TMP_Text>(true);
#endif
        }

        if (camara == null || !camara.isActiveAndEnabled)
        {
            if (Camera.main != null && Camera.main.isActiveAndEnabled)
            {
                camara = Camera.main;
            }
            else
            {
#if UNITY_2023_1_OR_NEWER
                camara = Object.FindFirstObjectByType<Camera>();
#else
                camara = Object.FindObjectOfType<Camera>();
#endif
            }
        }

        if (puntoPosicionTexto == null)
        {
            Transform[] hijos = GetComponentsInChildren<Transform>(true);
            foreach (Transform hijo in hijos)
            {
                if (hijo == transform) continue;
                if (textoBillboard != null && hijo == textoBillboard.transform) continue;

                string nombreMinuscula = hijo.name.ToLowerInvariant();
                if (nombreMinuscula.Contains("punto") ||
                    nombreMinuscula.Contains("texto") ||
                    nombreMinuscula.Contains("billboard") ||
                    nombreMinuscula.Contains("ancla") ||
                    nombreMinuscula.Contains("spawn"))
                {
                    puntoPosicionTexto = hijo;
                    break;
                }
            }

            if (puntoPosicionTexto == null)
            {
                puntoPosicionTexto = textoBillboard != null ? textoBillboard.transform : transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (!jugadorDentro || textoBillboard == null || !textoBillboard.gameObject.activeInHierarchy)
        {
            return;
        }

        if (camara == null || !camara.isActiveAndEnabled || puntoPosicionTexto == null)
        {
            ResolverReferencias();
            if (camara == null) return;
        }

        Transform tTexto = textoBillboard.transform;
        Transform tCam = camara.transform;

        // 1. Posicionar en el Transform asignado o encontrado automáticamente
        if (puntoPosicionTexto != null)
        {
            if (suavizadoPosicion > 0f)
            {
                tTexto.position = Vector3.Lerp(tTexto.position, puntoPosicionTexto.position, suavizadoPosicion * Time.deltaTime);
            }
            else
            {
                tTexto.position = puntoPosicionTexto.position;
            }
        }

        // 2. Orientar para mantener siempre la vista frontal de la cámara sin espejo
        if (alinearConCamara)
        {
            tTexto.rotation = tCam.rotation;
        }
        else
        {
            Vector3 haciaCamara = tCam.position - tTexto.position;
            if (haciaCamara.sqrMagnitude > 0.0001f)
            {
                tTexto.rotation = Quaternion.LookRotation((-haciaCamara).normalized, tCam.up);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        ResolverReferencias();
        jugadorDentro = true;

        if (textoBillboard != null)
        {
            if (!string.IsNullOrEmpty(mensaje))
            {
                textoBillboard.text = mensaje;
            }
            textoBillboard.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        jugadorDentro = false;

        if (textoBillboard != null)
        {
            textoBillboard.gameObject.SetActive(false);
        }
    }
}