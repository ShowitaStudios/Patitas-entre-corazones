using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    [Header("Conexión Opcional con Zona")]
    [SerializeField] private PuzzleZonaTrigger zonaAsociada;
    [SerializeField] private bool registrarEnZonaAlCompletar = true;

    [Header("Eventos al Completar")]
    public UnityEvent onPuzzleCompletado;

    private readonly HashSet<ElementoFlickPuzzle> elementosCompletados = new HashSet<ElementoFlickPuzzle>();
    private Coroutine rutinaCierre;
    private bool puzzleCompletado = false;

    public RectTransform AreaPanel => areaPanel;
    public float MargenSalida => margenSalida;

    private void Awake()
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

        for (int i = 0; i < elementos.Count; i++)
        {
            if (elementos[i] != null)
            {
                elementos[i].Inicializar(this);
            }
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

            // Corremos la espera en este MonoBehaviour o en el objeto a cerrar si sigue activo
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

        if (objetoACerrar != null)
        {
            objetoACerrar.SetActive(false);
        }

        rutinaCierre = null;
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