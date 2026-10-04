using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class EfectoGotaUI : MonoBehaviour
{
    [Header("Configuración de Animación")]
    [SerializeField] private float duracionTotal = 1.0f;
    
    [Header("Escala (Tamaño)")]
    [SerializeField] private Vector3 escalaInicial = new Vector3(0.3f, 0.3f, 1f);
    [SerializeField] private Vector3 escalaFinal = new Vector3(1.5f, 1.5f, 1f);

    [Header("Transparencia (Fade)")]
    [SerializeField] private float alphaInicial = 1.0f;
    [SerializeField] private float alphaFinal = 0.0f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        StartCoroutine(AnimarGota());
    }

    private IEnumerator AnimarGota()
    {
        float tiempoTranscurrido = 0f;
        transform.localScale = escalaInicial;
        canvasGroup.alpha = alphaInicial;

        while (tiempoTranscurrido < duracionTotal)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = tiempoTranscurrido / duracionTotal;
            transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, progreso);
            canvasGroup.alpha = Mathf.Lerp(alphaInicial, alphaFinal, progreso);
            yield return null;
        }
        Destroy(gameObject);
    }
}