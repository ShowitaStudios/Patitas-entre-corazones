using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ZonaVolumenPorProgreso : MonoBehaviour
{
    [System.Serializable]
    public struct VolumenZona
    {
        public string nombreZona;
        public Volume volumen;
    }

    [Header("Configuración de Volúmenes por Nombre")]
    [SerializeField] private List<VolumenZona> listaVolumenes = new List<VolumenZona>();
    [SerializeField] private float velocidadTransicion = 2.0f;

    private Dictionary<string, Volume> diccionarioVolumenes = new Dictionary<string, Volume>();
    private Dictionary<string, Coroutine> corrutinasActivas = new Dictionary<string, Coroutine>();

    private void Awake()
    {
        foreach (var item in listaVolumenes)
        {
            if (!string.IsNullOrEmpty(item.nombreZona) && item.volumen != null)
            {
                if (!diccionarioVolumenes.ContainsKey(item.nombreZona))
                {
                    diccionarioVolumenes.Add(item.nombreZona, item.volumen);
                    item.volumen.weight = 0f;
                    if (item.volumen.priority <= 0)
                    {
                        item.volumen.priority = 1;
                    }
                }
            }
        }
    }
    public void ActivarVolumenPorNombre(string nombreZona)
    {
        if (diccionarioVolumenes.ContainsKey(nombreZona))
        {
            Volume vol = diccionarioVolumenes[nombreZona];

            if (corrutinasActivas.ContainsKey(nombreZona) && corrutinasActivas[nombreZona] != null)
            {
                StopCoroutine(corrutinasActivas[nombreZona]);
            }

            corrutinasActivas[nombreZona] = StartCoroutine(InterpolarPeso(vol, 1f, nombreZona));
        }
        else
        {
            Debug.LogWarning($"[ZonaVolumenPorProgreso] No se encontró ningún volumen registrado con el nombre: '{nombreZona}'");
        }
    }

    private IEnumerator InterpolarPeso(Volume volumen, float pesoFinal, string clave)
    {
        float pesoInicial = volumen.weight;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < 1f)
        {
            tiempoTranscurrido += Time.deltaTime * velocidadTransicion;
            volumen.weight = Mathf.Lerp(pesoInicial, pesoFinal, tiempoTranscurrido);
            yield return null;
        }

        volumen.weight = pesoFinal;
        corrutinasActivas[clave] = null;
    }
}