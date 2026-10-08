using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RompecabezasUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PuzzleManager puzzleManager;

    [Header("Identificador para Persistencia")]
    [Tooltip("ID único para recordar que este puzzle de UI ya se resolvió (Ej: 'Puzzle_Cuadro_Habitacion1')")]
    [SerializeField] private string idUnicoPuzzle;

    [Header("Configuración del Puzzle")]
    [SerializeField] private int totalPiezas = 4;
    [SerializeField] private float tiempoParaCerrar = 1.5f;

    [Header("Progreso del Nivel")]
    [Tooltip("Marca si completar este puzzle UI suma al progreso general del nivel")]
    [SerializeField] private bool cuentaComoObjetivoNivel = true;

    [Header("Eventos")]
    [Tooltip("Acciones que suceden cuando el puzzle se completa")]
    [SerializeField] private UnityEvent alCompletarPuzzle;
    
    // Aca lo mismo natito, copias las lineas de codigo sin las barritas y se crean las referencias de los sonidos, un abrazo.
    // [SerializeField] private FMODUnity.EventReference sonidoPiezaEncajadaFMOD;
    // [SerializeField] private FMODUnity.EventReference sonidoPuzzleResueltoFMOD;

    private int piezasCorrectas = 0;
    private bool yaCompletado = false;

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
            piezasCorrectas = 0;
        }
    }
    public void PiezaEncajada()
    {
        if (yaCompletado) return;

        piezasCorrectas++;
        
        //Ya le sabes, borras las barritas y se crea el evento, un beso...
        // FMODUnity.RuntimeManager.PlayOneShot(sonidoPiezaEncajadaFMOD);

        if (piezasCorrectas >= totalPiezas)
        {
            CompletarPuzzle();
        }
    }
    private void CompletarPuzzle()
    {
        yaCompletado = true;
        
        //Lo mismo de arriba, te mando otro beso...
        // FMODUnity.RuntimeManager.PlayOneShot(sonidoPuzzleResueltoFMOD);
        
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
    private IEnumerator CerrarConRetraso()
    {
        yield return new WaitForSeconds(tiempoParaCerrar);
        if (puzzleManager != null)
        {
            puzzleManager.CerrarPuzzleActual();
        }
    }
}