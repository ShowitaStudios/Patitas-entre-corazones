using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleOsuUI : MonoBehaviour
{
    [Header("Referencias Generales")]
    [SerializeField] private PuzzleManager puzzleManager;

    [Header("Identificador para Persistencia")]
    [Tooltip("ID único para recordar que este puzzle de Osu ya se resolvió")]
    [SerializeField] private string idUnicoPuzzle;

    [Header("Configuración del Puzzle Osu")]
    [SerializeField] private GameObject prefabCirculoOsu;
    [SerializeField] private RectTransform areaAparicion; // El Panel donde aparecerán los círculos
    [SerializeField] private int aciertosRequeridos = 10;
    [SerializeField] private float tiempoEntreCirculos = 0.8f;
    [SerializeField] private float duracionCirculo = 1.8f;

    [Header("Progreso del Nivel")]
    [SerializeField] private bool cuentaComoObjetivoNivel = true;

    [Header("Eventos")]
    [SerializeField] private UnityEvent alCompletarPuzzle;

    private int aciertosActuales = 0;
    private bool yaCompletado = false;
    private Coroutine rutinaGenerador;

    private void Awake()
    {
        if (puzzleManager == null)
        {
#if UNITY_2023_1_OR_NEWER
            puzzleManager = Object.FindFirstObjectByType<PuzzleManager>();
#else
            puzzleManager = Object.FindObjectOfType<PuzzleManager>();
#endif
        }
    }

    private void Start()
    {
        // Verificar persistencia si ya fue resuelto en el GameManager
        if (!string.IsNullOrEmpty(idUnicoPuzzle) && GameManager.Instance != null)
        {
            if (GameManager.Instance.EsPuzzleCompletado(idUnicoPuzzle))
            {
                yaCompletado = true;
                alCompletarPuzzle?.Invoke();
            }
        }
    }

    private void OnEnable()
    {
        if (!yaCompletado)
        {
            aciertosActuales = 0;
            rutinaGenerador = StartCoroutine(GenerarCirculos());
        }
    }

    private void OnDisable()
    {
        if (rutinaGenerador != null)
        {
            StopCoroutine(rutinaGenerador);
        }
        LimpiarCirculosRestantes();
    }

    private IEnumerator GenerarCirculos()
    {
        while (aciertosActuales < aciertosRequeridos && !yaCompletado)
        {
            CrearNuevoCirculo();
            yield return new WaitForSeconds(tiempoEntreCirculos);
        }
    }

    private void CrearNuevoCirculo()
    {
        if (prefabCirculoOsu == null || areaAparicion == null) return;

        GameObject nuevoCirculo = Instantiate(prefabCirculoOsu, areaAparicion);
        RectTransform rect = nuevoCirculo.GetComponent<RectTransform>();

        if (rect != null)
        {
            // Generar posición aleatoria dentro del área asignada
            float xRandom = Random.Range(-areaAparicion.rect.width / 2f + 50f, areaAparicion.rect.width / 2f - 50f);
            float yRandom = Random.Range(-areaAparicion.rect.height / 2f + 50f, areaAparicion.rect.height / 2f - 50f);
            rect.anchoredPosition = new Vector2(xRandom, yRandom);
        }

        CirculoOsuUI componenteCirculo = nuevoCirculo.GetComponent<CirculoOsuUI>();
        if (componenteCirculo != null)
        {
            componenteCirculo.Inicializar(this, duracionCirculo);
        }
    }

    public void RegistrarAcierto()
    {
        if (yaCompletado) return;

        aciertosActuales++;

        if (aciertosActuales >= aciertosRequeridos)
        {
            CompletarPuzzle();
        }
    }

    public void RegistrarFallo()
    {
        // Puedes agregar lógica aquí si quieres restar puntos o reiniciar el contador al fallar
    }

    private void CompletarPuzzle()
    {
        yaCompletado = true;

        if (rutinaGenerador != null) StopCoroutine(rutinaGenerador);
        LimpiarCirculosRestantes();

        // Registrar en GameManager
        if (!string.IsNullOrEmpty(idUnicoPuzzle) && GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarPuzzleCompletado(idUnicoPuzzle);
        }

        // Registrar en LevelProgressManager
        if (cuentaComoObjetivoNivel && LevelProgressManager.Instance != null)
        {
            LevelProgressManager.Instance.RegistrarPuzzleResuelto();
        }

        alCompletarPuzzle?.Invoke();

        StartCoroutine(CerrarConRetraso());
    }

    private void LimpiarCirculosRestantes()
    {
        foreach (Transform child in areaAparicion)
        {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator CerrarConRetraso()
    {
        yield return new WaitForSeconds(1.0f);

        if (puzzleManager != null)
        {
            puzzleManager.CerrarPuzzleActual();
        }
    }
}