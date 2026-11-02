using System.Collections;
using UnityEngine;

public class CamaraCinematicaMultiples : MonoBehaviour
{
    [Header("Configuración de Cámaras")]
    [SerializeField] private Camera camaraJugador;
    [SerializeField] private float duracionTransicion = 1.0f;
    [SerializeField] private float duracionEnDestino = 2.0f;

    private bool enCinematica = false;

    private void Start()
    {
        if (camaraJugador == null && Camera.main != null)
        {
            camaraJugador = Camera.main;
        }
    }

    public void ActivarCamaraCinematica(Camera camaraCinematicaObjetivo)
    {
        if (enCinematica || camaraCinematicaObjetivo == null || camaraJugador == null) return;

        StartCoroutine(RutinaTransicionSuave(camaraCinematicaObjetivo));
    }

    private IEnumerator RutinaTransicionSuave(Camera camaraObjetivo)
    {
        enCinematica = true;

        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador != null)
        {
            MonoBehaviour controladorJugador = jugador.GetComponent<MonoBehaviour>();
            if (controladorJugador != null) controladorJugador.enabled = false;
        }

        Transform tJugador = camaraJugador.transform;
        Transform tDestino = camaraObjetivo.transform;

        Vector3 posInicial = tJugador.position;
        Quaternion rotInicial = tJugador.rotation;

        Vector3 posFinal = tDestino.position;
        Quaternion rotFinal = tDestino.rotation;

        camaraJugador.enabled = false;
        camaraObjetivo.gameObject.SetActive(true);

        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionTransicion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = Mathf.Clamp01(tiempoTranscurrido / duracionTransicion);
            float curvaT = Mathf.SmoothStep(0f, 1f, t);

            tDestino.position = Vector3.Lerp(posInicial, posFinal, curvaT);
            tDestino.rotation = Quaternion.Slerp(rotInicial, rotFinal, curvaT);

            yield return null;
        }

        tDestino.position = posFinal;
        tDestino.rotation = rotFinal;

        yield return new WaitForSeconds(duracionEnDestino);

        tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionTransicion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = Mathf.Clamp01(tiempoTranscurrido / duracionTransicion);
            float curvaT = Mathf.SmoothStep(0f, 1f, t);

            tDestino.position = Vector3.Lerp(posFinal, posInicial, curvaT);
            tDestino.rotation = Quaternion.Slerp(rotFinal, rotInicial, curvaT);

            yield return null;
        }

        tDestino.position = posFinal;
        tDestino.rotation = rotFinal;
        camaraObjetivo.gameObject.SetActive(false);
        camaraJugador.enabled = true;

        if (jugador != null)
        {
            MonoBehaviour controladorJugador = jugador.GetComponent<MonoBehaviour>();
            if (controladorJugador != null) controladorJugador.enabled = true;
        }

        enCinematica = false;
    }
}