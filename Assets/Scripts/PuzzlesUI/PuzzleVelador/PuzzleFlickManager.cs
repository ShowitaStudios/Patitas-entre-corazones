using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PuzzleFlickManager : MonoBehaviour
{
    [Header("Área del Puzzle")]
    [Tooltip("Panel que contiene los elementos. Si queda vacío, se usa el RectTransform de este objeto.")]
    [SerializeField] private RectTransform areaPanel;
    [Tooltip("Margen extra en píxeles respecto al borde del panel para considerar que el objeto ya salió.")]
    [SerializeField] private float margenSalida = 60f;

    [Header("Elementos del Puzzle")]
    [Tooltip("Si queda vacío, busca automáticamente los ElementoFlickPuzzle hijos al iniciar.")]
    [SerializeField] private List<ElementoFlickPuzzle> elementos = new List<ElementoFlickPuzzle>();

    [Header("Cierre Automático")]
    [Tooltip("Canvas u objeto raíz a cerrar al completar. Si queda vacío, busca el Canvas padre automáticamente.")]
    [SerializeField] private GameObject objetoACerrar;
    [SerializeField] private bool cerrarAlCompletar = true;
    [Min(0f)]
    [SerializeField] private float esperaAntesDeCerrar = 1.5f;
    [Tooltip("Usa tiempo real (ignora Time.timeScale = 0). Útil si tu UI pausa el juego.")]
    [SerializeField] private bool usarTiempoReal = true;

    [Header("Restaurar Control al Cerrar")]
    [Tooltip("Restaura Time.timeScale a 1 al cerrar el canvas.")]
    [SerializeField] private bool restaurarTimeScale = true;
    [Tooltip("Bloquea y oculta el cursor para volver al control en tercera persona.")]
    [SerializeField] private bool bloquearYOcultarCursor = true;
    [Tooltip("Limpia el objeto seleccionado en el EventSystem al cerrar.")]
    [SerializeField] private bool limpiarSeleccionUI = true;
    [Tooltip("Llama a PuzzleManager.CerrarPuzzleActual() al cerrarse.")]
    [SerializeField] private bool notificarPuzzleManagerAlCerrar = true;

    [Header("Conexión Opcional con Zona")]
    [SerializeField] private PuzzleZonaTrigger zonaAsociada;
    [SerializeField] private bool registrarEnZonaAlCompletar = true;

    [Header("Eventos")]
    public UnityEvent onPuzzleCompletado;
    [Tooltip("Se ejecuta justo antes de desactivar el canvas al completarse.")]
    public UnityEvent onAlCerrarCanvas;

    private readonly HashSet<ElementoFlickPuzzle> elementosCompletados = new HashSet<ElementoFlickPuzzle>();
    private PuzzleManager puzzleManager;
    private Coroutine rutinaCierre;
    private bool puzzleCompletado = false;

    public RectTransform AreaPanel => areaPanel;
    public float MargenSalida => margenSalida;

    private void Awake()
    {
        AsegurarReferencias();

        for (int i = 0; i < elementos.Count; i++)
        {
            if (elementos[i] != null)
            {
                elementos[i].Inicializar(this);
            }
        }
    }

    private void AsegurarReferencias()
    {
        if (areaPanel == null)
        {
            areaPanel = GetComponent<RectTransform>();
        }

        if (objetoACerrar == null)
        {
            Canvas canvasPadre = GetComponentInParent<Canvas>(true);
            if (canvasPadre != null)
            {
                objetoACerrar = canvasPadre.gameObject;
            }
        }

        if (elementos.Count == 0)
        {
            GetComponentsInChildren(true, elementos);
        }

        if (puzzleManager == null)
        {
#if UNITY_2023_1_OR_NEWER
            puzzleManager = Object.FindFirstObjectByType<PuzzleManager>(FindObjectsInactive.Include);
#else
            puzzleManager = Object.FindObjectOfType<PuzzleManager>(true);
#endif
        }
    }

    private void OnEnable()
    {
        if (rutinaCierre != null)
        {
            StopCoroutine(rutinaCierre);
            rutinaCierre = null;
        }
    }

    public void NotificarElementoFuera(ElementoFlickPuzzle elemento)
    {
        if (puzzleCompletado || elemento == null) return;
        if (!elementosCompletados.Add(elemento)) return;

        int totalObjetivo = elementos.Count > 0 ? elementos.Count : 6;
        if (elementosCompletados.Count >= totalObjetivo)
        {
            CompletarPuzzle();
        }
    }

    private void CompletarPuzzle()
    {
        puzzleCompletado = true;

        if (registrarEnZonaAlCompletar && zonaAsociada != null)
        {
            zonaAsociada.RegistrarPuzzleZona();
        }

        onPuzzleCompletado?.Invoke();

        if (cerrarAlCompletar && objetoACerrar != null)
        {
            if (rutinaCierre != null)
            {
                StopCoroutine(rutinaCierre);
            }

            rutinaCierre = StartCoroutine(RutinaCerrarCanvas());
        }
    }

    private IEnumerator RutinaCerrarCanvas()
    {
        if (esperaAntesDeCerrar > 0f)
        {
            if (usarTiempoReal)
            {
                yield return new WaitForSecondsRealtime(esperaAntesDeCerrar);
            }
            else
            {
                yield return new WaitForSeconds(esperaAntesDeCerrar);
            }
        }

        RestaurarEstadoGameplay();
        onAlCerrarCanvas?.Invoke();

        if (objetoACerrar != null)
        {
            objetoACerrar.SetActive(false);
        }

        rutinaCierre = null;
    }

    private void RestaurarEstadoGameplay()
    {
        if (restaurarTimeScale && Mathf.Approximately(Time.timeScale, 0f))
        {
            Time.timeScale = 1f;
        }

        if (bloquearYOcultarCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (limpiarSeleccionUI && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        if (notificarPuzzleManagerAlCerrar)
        {
            if (puzzleManager == null)
            {
#if UNITY_2023_1_OR_NEWER
                puzzleManager = Object.FindFirstObjectByType<PuzzleManager>(FindObjectsInactive.Include);
#else
                puzzleManager = Object.FindObjectOfType<PuzzleManager>(true);
#endif
            }

            if (puzzleManager != null)
            {
                puzzleManager.CerrarPuzzleActual();
            }
        }
    }

    public void ReiniciarPuzzle()
    {
        if (rutinaCierre != null)
        {
            StopCoroutine(rutinaCierre);
            rutinaCierre = null;
        }

        puzzleCompletado = false;
        elementosCompletados.Clear();

        if (objetoACerrar != null && !objetoACerrar.activeSelf)
        {
            objetoACerrar.SetActive(true);
        }

        for (int i = 0; i < elementos.Count; i++)
        {
            if (elementos[i] != null)
            {
                elementos[i].ReiniciarElemento();
            }
        }
    }
}