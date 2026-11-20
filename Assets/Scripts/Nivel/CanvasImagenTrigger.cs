using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class CanvasImagenTrigger : MonoBehaviour
{
    public static CanvasImagenTrigger Instance { get; private set; }

    [Header("Referencias")]
    [Tooltip("Imagen dentro del Canvas. Si queda vacía, se busca automáticamente en los hijos.")]
    [SerializeField] private Image imagenUI;
    [Tooltip("CanvasGroup del Canvas. Si queda vacío, se obtiene o agrega automáticamente.")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool ocultarAlIniciar = true;

    [Header("Tiempos (Segundos)")]
    [Min(0.01f)]
    [SerializeField] private float duracionFadeIn = 0.5f;
    [Min(0f)]
    [SerializeField] private float tiempoEnPantalla = 2.5f;
    [Min(0.01f)]
    [SerializeField] private float duracionFadeOut = 0.5f;

    [Header("Opciones")]
    [Tooltip("Desactiva el GameObject del Canvas al terminar el fade out.")]
    [SerializeField] private bool desactivarAlTerminar = true;
    [Tooltip("Si está activo, solo muestra la imagen la primera vez que se activa.")]
    [SerializeField] private bool mostrarSoloUnaVez = false;

    private Coroutine rutinaFade;
    private bool yaMostrado = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (imagenUI == null)
        {
            imagenUI = GetComponentInChildren<Image>(true);
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (ocultarAlIniciar)
        {
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
        else
        {
            canvasGroup.alpha = 1f;
        }
    }

    public void MostrarCanvas()
    {
        MostrarCanvas(null);
    }

    public void MostrarCanvas(Sprite spriteZona)
    {
        if (mostrarSoloUnaVez && yaMostrado) return;

        if (imagenUI != null && spriteZona != null)
        {
            imagenUI.sprite = spriteZona;
        }

        if (imagenUI == null || imagenUI.sprite == null) return;

        yaMostrado = true;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (rutinaFade != null)
        {
            StopCoroutine(rutinaFade);
        }

        rutinaFade = StartCoroutine(SecuenciaFade());
    }

    private IEnumerator SecuenciaFade()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float alphaInicial = canvasGroup.alpha;
        float tiempo = 0f;
        while (tiempo < duracionFadeIn)
        {
            tiempo += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(alphaInicial, 1f, Mathf.Clamp01(tiempo / duracionFadeIn));
            yield return null;
        }
        canvasGroup.alpha = 1f;

        if (tiempoEnPantalla > 0f)
        {
            yield return new WaitForSeconds(tiempoEnPantalla);
        }

        tiempo = 0f;
        while (tiempo < duracionFadeOut)
        {
            tiempo += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp01(tiempo / duracionFadeOut));
            yield return null;
        }
        canvasGroup.alpha = 0f;

        if (desactivarAlTerminar)
        {
            gameObject.SetActive(false);
        }

        rutinaFade = null;
    }
}