using UnityEngine;
using UnityEngine.EventSystems;

public class DetectorClicsMenu : MonoBehaviour, IPointerDownHandler
{
    [Header("Configuración del Efecto")]
    [Tooltip("Arrastra aquí el PREFAB de tu efecto de gota o huellita")]
    [SerializeField] private GameObject prefabEfectoGota;

    [Tooltip("El Canvas principal o el contenedor donde se instanciarán las gotas")]
    [SerializeField] private RectTransform contenedorCanvas;

    [Header("Easter Egg (Menú Secreto)")]
    [SerializeField] private int clicksRequeridos = 67;
    [SerializeField] private string nombreEscenaSecreta;
    private int clicksActuales = 0;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        clicksActuales++;
        if (clicksActuales >= clicksRequeridos)
        {
            AbrirEscenaSecreta();
        }

        if (prefabEfectoGota == null || contenedorCanvas == null)
        {
            Debug.LogWarning("Faltan referencias en el DetectorClicsMenu.");
            return;
        }

        Vector2 posicionEnCanvas;
        bool exitoConversion = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            contenedorCanvas,
            eventData.position,
            eventData.pressEventCamera,
            out posicionEnCanvas
        );
        
        if (exitoConversion)
        {
            GameObject nuevaGota = Instantiate(prefabEfectoGota, contenedorCanvas);
            RectTransform rectTransformGota = nuevaGota.GetComponent<RectTransform>();
            
            if (rectTransformGota != null)
            {
                rectTransformGota.anchoredPosition = posicionEnCanvas;
            }
        }
    }

    private void AbrirEscenaSecreta()
    {
        if (!string.IsNullOrEmpty(nombreEscenaSecreta))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscenaSecreta);
        }
        else
        {
            Debug.LogWarning("El nombre de la escena secreta no está asignado en el Inspector.");
        }
    }
}