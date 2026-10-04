using UnityEngine;
using UnityEngine.SceneManagement;

public class PuertaTransicionSinFade : MonoBehaviour
{
    [SerializeField] private string nombreEscenaDestino;
    [SerializeField] private string idPuertaDestino;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.idPuertaEntrada = idPuertaDestino;
        }

        SceneManager.LoadScene(nombreEscenaDestino);
    }
}