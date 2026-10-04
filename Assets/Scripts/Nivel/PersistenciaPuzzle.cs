using UnityEngine;
using UnityEngine.Events;

public class PersistenciaPuzzle : MonoBehaviour
{
    [Header("Identificador Único")]
    [Tooltip("Escribe un ID único para este puzle o objeto (Ej: 'Florero_Cocina_01', 'Puzzle_CajaFuerte')")]
    [SerializeField] private string idUnico;

    [Header("Comportamiento al Cargar Escena")]
    [Tooltip("Si está activo, el objeto se destruirá automáticamente si ya fue completado.")]
    [SerializeField] private bool destruirSiCompletado = true;

    [Tooltip("Eventos a ejecutar si ya está completado (útil para desactivar colliders, abrir puertas, etc.)")]
    [SerializeField] private UnityEvent alEstarCompletado;

    private void Start()
    {
        VerificarEstado();
    }
    public void VerificarEstado()
    {
        if (GameManager.Instance != null && GameManager.Instance.EsPuzzleCompletado(idUnico))
        {
            alEstarCompletado?.Invoke();

            if (destruirSiCompletado)
            {
                Destroy(gameObject);
            }
        }
    }
    public void GuardarComoCompletado()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarPuzzleCompletado(idUnico);
        }
    }
}