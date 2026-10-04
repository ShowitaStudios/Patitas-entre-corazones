using UnityEngine;

public class PuertaDestruiblePorProgreso : MonoBehaviour
{
    [Header("Configuración de Progreso")]
    [Tooltip("Cantidad de puzzles necesarios para que esta puerta permanezca destruida")]
    [SerializeField] private int puzzlesRequeridos = 3;

    [Tooltip("ID único opcional si prefieres guardarlo directamente en el GameManager")]
    [SerializeField] private string idUnicoPuerta = "Puerta_Nivel1_Destruida";

    private void Start()
    {
        VerificarEstadoPuerta();
    }

    public void VerificarEstadoPuerta()
    {
        if (LevelProgressManager.Instance != null)
        {
            if (LevelProgressManager.Instance.PuzzlesResueltos >= puzzlesRequeridos)
            {
                DestruirPuertaSilenciosamente();
                return;
            }
        }
        if (!string.IsNullOrEmpty(idUnicoPuerta) && GameManager.Instance != null)
        {
            if (GameManager.Instance.EsPuzzleCompletado(idUnicoPuerta))
            {
                DestruirPuertaSilenciosamente();
            }
        }
    }
    public void DestruirPuerta()
    {
        if (!string.IsNullOrEmpty(idUnicoPuerta) && GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarPuzzleCompletado(idUnicoPuerta);
        }

        Destroy(gameObject);
    }

    private void DestruirPuertaSilenciosamente()
    {
        Destroy(gameObject);
    }
}