using UnityEngine;
using UnityEngine.Events;

public class LevelProgressManager : MonoBehaviour
{
    [Header("Configuración del Nivel")]
    [Tooltip("ID único para este nivel (ej: 'Progreso_Nivel_1') para guardarlo en el GameManager")]
    [SerializeField] private string idNivel = "Progreso_Nivel_1";

    [Tooltip("Cantidad de puzzles necesarios para activar el evento del nivel")]
    [SerializeField] private int puzzlesRequeridos = 3;

    [Header("Eventos al Completar")]
    [Tooltip("Arrastra aquí lo que quieras que suceda (destruir puerta, PlayableDirector para cinemática, SetActive, etc.)")]
    [SerializeField] private UnityEvent alCompletarTodosLosPuzzles;

    [Header("Estado Actual (Solo Lectura)")]
    [SerializeField] private int puzzlesCompletados = 0;

    public static LevelProgressManager Instance { get; private set; }

    public int PuzzlesResueltos => puzzlesCompletados;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CargarProgresoPersistente();
    }

    /// <summary>
    /// Consulta al GameManager el progreso guardado al entrar a la escena.
    /// </summary>
    private void CargarProgresoPersistente()
    {
        if (GameManager.Instance != null)
        {
            // Leemos cuántos puzzles llevaba completados este nivel desde el GameManager
            puzzlesCompletados = GameManager.Instance.ObtenerProgresoNivel(idNivel);

            Debug.Log($"[Progreso Cargado] Puzzles resueltos guardados: {puzzlesCompletados}/{puzzlesRequeridos}");

            // Si al entrar a la escena ya se habían completado todos los puzzles, ejecutamos el evento final
            if (puzzlesCompletados >= puzzlesRequeridos)
            {
                DesencadenarEventoFinal();
            }
        }
    }

    public void RegistrarPuzzleResuelto()
    {
        puzzlesCompletados++;

        // Guardamos el progreso acumulado en el GameManager persistente
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GuardarProgresoNivel(idNivel, puzzlesCompletados);
        }

        Debug.Log($"[Progreso] Puzzle completado: {puzzlesCompletados}/{puzzlesRequeridos}");

        if (puzzlesCompletados >= puzzlesRequeridos)
        {
            DesencadenarEventoFinal();
        }
    }

    private void DesencadenarEventoFinal()
    {
        Debug.Log("[Progreso] ¡Todos los puzzles del nivel han sido completados!");
        alCompletarTodosLosPuzzles?.Invoke();
    }
}