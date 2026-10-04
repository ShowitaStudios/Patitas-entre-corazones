using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CirculoOsuUI : MonoBehaviour
{
    [Header("Configuración de Animación")]
    [Tooltip("El círculo externo que se va encogiendo hacia el centro")]
    [SerializeField] private RectTransform circuloAproximacion;
    [SerializeField] private float tiempoDeVida = 2.0f;

    private Button botonCirculo;
    private PuzzleOsuUI puzzlePadre;
    private Coroutine rutinaVida;

    private void Awake()
    {
        botonCirculo = GetComponent<Button>();
        botonCirculo.onClick.AddListener(AlHacerClic);
    }

    public void Inicializar(PuzzleOsuUI padre, float tiempo)
    {
        puzzlePadre = padre;
        tiempoDeVida = tiempo;
        rutinaVida = StartCoroutine(AnimarYExpirar());
    }

    private IEnumerator AnimarYExpirar()
    {
        float tiempoTranscurrido = 0f;
        Vector3 escalaInicial = new Vector3(2.5f, 2.5f, 1f);
        Vector3 escalaFinal = Vector3.one;

        if (circuloAproximacion != null)
        {
            circuloAproximacion.localScale = escalaInicial;
        }

        while (tiempoTranscurrido < tiempoDeVida)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = tiempoTranscurrido / tiempoDeVida;

            if (circuloAproximacion != null)
            {
                circuloAproximacion.localScale = Vector3.Lerp(escalaInicial, escalaFinal, progreso);
            }

            yield return null;
        }

        // Si se acaba el tiempo y no hizo clic
        puzzlePadre.RegistrarFallo();
        Destroy(gameObject);
    }

    private void AlHacerClic()
    {
        if (rutinaVida != null) StopCoroutine(rutinaVida);

        puzzlePadre.RegistrarAcierto();
        Destroy(gameObject);
    }
}