using UnityEngine;
using UnityEngine.EventSystems;

public class DetectorClicsMenu : MonoBehaviour, IPointerDownHandler
{
    [Header("Configuración del Efecto")]
    [Tooltip("Arrastra aquí el PREFAB de tu efecto de gota o huellita")]
    [SerializeField] private GameObject prefabEfectoGota;

    [Tooltip("El Canvas principal o el contenedor donde se instanciarán las gotas")]
    [SerializeField] private RectTransform contenedorCanvas;
    
    public void OnPointerDown(PointerEventData eventData)
    {
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
}