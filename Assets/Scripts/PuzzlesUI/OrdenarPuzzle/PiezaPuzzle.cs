using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;    

public class PiezaPuzzle : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Target u Objetivo")]
    [SerializeField] private RectTransform encajeTarget;
    [SerializeField] private float toleranciaEncaje = 30f;
    [SerializeField] private RompecabezasUI controladorRompecabezas;

    [Header("Sonidos")]
    [SerializeField] private StudioEventEmitter sonidoTomar;
    [SerializeField] private StudioEventEmitter sonidoDejar;
    [SerializeField] private StudioEventEmitter sonidoEncajar;

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
    //Sonidos aqui, natito, para que suene cuando se toma la pieza, un abarso.
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (estaEncajada) return;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
        sonidoTomar.Play();
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
        //sonidoDejar.Play();

        float distancia = Vector2.Distance(rectTransform.anchoredPosition, encajeTarget.anchoredPosition);

        if (distancia <= toleranciaEncaje)
        {
            rectTransform.anchoredPosition = encajeTarget.anchoredPosition;
            estaEncajada = true;

            if (controladorRompecabezas != null)
            {
                controladorRompecabezas.PiezaEncajada();
                sonidoEncajar.Play();
            }
        }
        else
        {
            rectTransform.anchoredPosition = posicionInicial;
            sonidoDejar.Play();
        }
    }
}