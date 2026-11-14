using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SlideshowManager : MonoBehaviour
{
    [Header("Configuración de Imágenes")]
    [SerializeField] private Image imagenDisplay;
    [SerializeField] private List<Sprite> listaSlides = new List<Sprite>();
    [SerializeField] private float tiempoPorSlide = 3.0f;

    [Header("Escena de Destino")]
    [SerializeField] private string nombreEscenaJuego = "CasaCompletaTestScene";

    private int indiceActual = 0;
    private bool reproduciendo = false;

    private void Start()
    {
        if (listaSlides.Count > 0 && imagenDisplay != null)
        {
            StartCoroutine(EjecutarSlideshow());
        }
        else
        {
            Debug.LogWarning("[SlideshowManager] Faltan imágenes o el componente Image de UI no está asignado.");
            ContinuarAlJuego();
        }
    }

    private IEnumerator EjecutarSlideshow()
    {
        reproduciendo = true;

        for (int i = 0; i < listaSlides.Count; i++)
        {
            indiceActual = i;
            imagenDisplay.sprite = listaSlides[indiceActual];

            yield return new WaitForSeconds(tiempoPorSlide);
        }

        reproduciendo = false;
        ContinuarAlJuego();
    }

    private void ContinuarAlJuego()
    {
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    private void Update()
    {
        if (reproduciendo && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            // Aqui puedo poner uhh mas cosas
        }
    }
}