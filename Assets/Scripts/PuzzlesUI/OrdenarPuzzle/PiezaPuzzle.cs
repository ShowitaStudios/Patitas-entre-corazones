using UnityEngine;
using UnityEngine.EventSystems;

public class PiezaPuzzle : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Target u Objetivo")]
    [SerializeField] private RectTransform encajeTarget;
    [SerializeField] private float toleranciaEncaje = 30f;
    [SerializeField] private RompecabezasUI controladorRompecabezas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 posicionInicial;
    private bool estaEncajada = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        posicionInicial = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (estaEncajada) return;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (estaEncajada) return;

        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (estaEncajada) return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        float distancia = Vector2.Distance(rectTransform.anchoredPosition, encajeTarget.anchoredPosition);

        if (distancia <= toleranciaEncaje)
        {
            rectTransform.anchoredPosition = encajeTarget.anchoredPosition;
            estaEncajada = true;

            if (controladorRompecabezas != null)
            {
                controladorRompecabezas.PiezaEncajada();
            }
        }
        else
        {
            rectTransform.anchoredPosition = posicionInicial;
        }
    }
}