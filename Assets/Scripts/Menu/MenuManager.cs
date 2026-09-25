using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [SerializeField] private string sceneToLoad;

    public void PlayGame()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el nombre de la escena");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Cerrando el juego");
        Application.Quit();
    }
}