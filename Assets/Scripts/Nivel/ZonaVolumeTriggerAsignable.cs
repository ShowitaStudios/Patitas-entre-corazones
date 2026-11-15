using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(BoxCollider))]
public class ZonaVolumenTriggerAsignable : MonoBehaviour
{
    [Header("Configuración de Zona")]
    [SerializeField] private Volume volumenHabitacion;
    [SerializeField] private float velocidadTransicion = 2.0f;

    private Coroutine transicionActiva;
    private bool estaEncendido = false;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        if (volumenHabitacion != null)
        {
            volumenHabitacion.weight = 0f;
            if (volumenHabitacion.priority <= 0)
            {
                volumenHabitacion.priority = 10; // Prioridad alta para dominar sobre la base
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && volumenHabitacion != null)
        {
            // Alternamos el estado (Toggle) al cruzar el trigger
            estaEncendido = !estaEncendido;
            float pesoObjetivo = estaEncendido ? 1f : 0f;

            if (transicionActiva != null)
            {
                StopCoroutine(transicionActiva);
            }

            transicionActiva = StartCoroutine(InterpolarPeso(pesoObjetivo));
        }
    }

    private IEnumerator InterpolarPeso(float pesoFinal)
    {
        float pesoInicial = volumenHabitacion.weight;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < 1f)
        {
            tiempoTranscurrido += Time.deltaTime * velocidadTransicion;
            volumenHabitacion.weight = Mathf.Lerp(pesoInicial, pesoFinal, tiempoTranscurrido);
            yield return null;
        }

        volumenHabitacion.weight = pesoFinal;
    }
}