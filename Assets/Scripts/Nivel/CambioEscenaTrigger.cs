using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscenaTrigger : MonoBehaviour
{
    [SerializeField] private string nombreEscenaDestino;
    [SerializeField] private bool soloJugador = true;

    private void OnTriggerEnter(Collider other)
    {
        if (soloJugador && !other.CompareTag("Player"))
        {
            return;
        }

        if (!string.IsNullOrEmpty(nombreEscenaDestino))
        {
            SceneManager.LoadScene(nombreEscenaDestino);
        }
        else
        {
            Debug.LogError("No se ha asignado un nombre de escena en el inspector de " + gameObject.name);
        }
    }
}