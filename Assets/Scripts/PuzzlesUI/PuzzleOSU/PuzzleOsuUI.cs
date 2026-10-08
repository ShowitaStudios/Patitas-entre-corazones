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
    [SerializeField] private RectTransform areaAparicion;
    [SerializeField] private int aciertosRequeridos = 10;
    [SerializeField] private float tiempoEntreCirculos = 0.8f;
    [SerializeField] private float duracionCirculo = 1.8f;

    [Header("Progreso del Nivel")]
    [SerializeField] private bool cuentaComoObjetivoNivel = true;

    [Header("Eventos")]
    [SerializeField] private UnityEvent alCompletarPuzzle;
    // Referencias para los eventos de audio del puzzle Osu, aqui tienes que poner tal cual pero sin los slash para crear las referencias para eventos del Fmod, saludos nato si rompes algo te mato
    // [SerializeField] private FMODUnity.EventReference eventoAciertoFMOD;
    // [SerializeField] private FMODUnity.EventReference eventoFalloFMOD;
    // [SerializeField] private FMODUnity.EventReference eventoVictoriaFMOD;

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
        // Aca pones lo mismo , copias el codigo que te deje abajito y se crea el evento, un saludo natanael
        // FMODUnity.RuntimeManager.PlayOneShot(eventoAciertoFMOD);

        if (aciertosActuales >= aciertosRequeridos)
        {
            CompletarPuzzle();
        }
    }
    public void RegistrarFallo()
    {
        // Aqui puedo meter weas por si pierde el player, no olvidar att sehita
    }

    private void CompletarPuzzle()
    {
        yaCompletado = true;

        if (rutinaGenerador != null) StopCoroutine(rutinaGenerador);
        LimpiarCirculosRestantes();
        
        //Lo mismo de antes, copias el codigo sin las barritas y se crea el evento.
        // FMODUnity.RuntimeManager.PlayOneShot(eventoVictoriaFMOD);
        
        if (!string.IsNullOrEmpty(idUnicoPuzzle) && GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarPuzzleCompletado(idUnicoPuzzle);
        }
        
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