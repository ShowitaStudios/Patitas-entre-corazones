using UnityEngine;
using UnityEngine.Events;

public class LevelProgressManager : MonoBehaviour
{
    [Header("Configuración del Nivel")]
    [Tooltip("Cantidad de puzzles necesarios para activar el evento del nivel")]
    [SerializeField] private int puzzlesRequeridos = 2;

    [Header("Eventos al Completar")]
    [Tooltip("Arrastra aquí lo que quieras que suceda (PlayableDirector para cinemática, SetActive para objetos, etc.)")]
    [SerializeField] private UnityEvent alCompletarTodosLosPuzzles;

    [Header("Estado Actual (Solo Lectura)")]
    [SerializeField] private int puzzlesCompletados = 0;
    public static LevelProgressManager Instance { get; private set; }

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
    
    public void RegistrarPuzzleResuelto()
    {
        puzzlesCompletados++;

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