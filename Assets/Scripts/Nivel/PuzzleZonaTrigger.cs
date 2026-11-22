using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PuzzleZonaTrigger : MonoBehaviour
{
    [Header("Configuración de Zona")]
    [SerializeField] private string nombreZona = "Living";
    [SerializeField] private int puzzlesActualesEnZona = 0;
    [SerializeField] private int totalPuzzlesEnZona = 3;

    [Header("Aviso Visual de Zona")]
    [SerializeField] private bool mostrarImagenAlEntrar = true;
    [Tooltip("Si está activo, la imagen de esta zona solo se muestra la primera vez que el jugador entra.")]
    [SerializeField] private bool mostrarImagenSoloPrimeraVez = true;
    [Tooltip("Imagen específica que aparecerá al entrar a este trigger.")]
    [SerializeField] private Sprite imagenZona;

    private bool imagenYaMostradaEnZona = false;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MostrarProgresoEnUI();

            bool puedeMostrarImagen = mostrarImagenAlEntrar &&
                                      (!mostrarImagenSoloPrimeraVez || !imagenYaMostradaEnZona);

            if (puedeMostrarImagen && CanvasImagenTrigger.Instance != null)
            {
                imagenYaMostradaEnZona = true;
                CanvasImagenTrigger.Instance.MostrarCanvas(imagenZona);
            }
        }
    }

    public void RegistrarPuzzleZona(bool mostrarEnUI = true)
    {
        if (puzzlesActualesEnZona < totalPuzzlesEnZona)
        {
            puzzlesActualesEnZona = Mathf.Min(puzzlesActualesEnZona + 1, totalPuzzlesEnZona);
        }

        if (mostrarEnUI)
        {
            MostrarProgresoEnUI();
        }
    }

    private void MostrarProgresoEnUI()
    {
        if (PuzzleUIManager.Instance != null)
        {
            PuzzleUIManager.Instance.ActualizarTextoUI(nombreZona, puzzlesActualesEnZona, totalPuzzlesEnZona);
        }
    }
}