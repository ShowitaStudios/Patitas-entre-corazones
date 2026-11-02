using UnityEngine;

public class PuzzleZonaTrigger : MonoBehaviour
{
    [Header("Configuración de Zona")]
    [SerializeField] private string nombreZona = "Living";
    [SerializeField] private int puzzlesActualesEnZona = 0;
    [SerializeField] private int totalPuzzlesEnZona = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MostrarProgresoEnUI();
        }
    }

    public void RegistrarPuzzleZona()
    {
        // Usamos Mathf.Min para asegurarnos de que jamás sobrepase el total
        if (puzzlesActualesEnZona < totalPuzzlesEnZona)
        {
            puzzlesActualesEnZona++;
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