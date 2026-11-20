using System.Collections.Generic;
using UnityEngine;

public class DitherObstaculosCamara : MonoBehaviour
{
    [Header("Objetivo")]
    [Tooltip("Transform del jugador o el punto/pivot dentro del Player.")]
    [SerializeField] private Transform objetivoJugador;
    [SerializeField] private Vector3 offsetObjetivo = new Vector3(0f, 1.2f, 0f);

    [Header("Detección de Obstáculos")]
    [Tooltip("Capa(s) de muros o props que pueden quedar entre la cámara y el jugador.")]
    [SerializeField] private LayerMask capasObstaculos;
    [SerializeField] private float radioDeteccion = 0.35f;

    [Header("Dithering (_DitherFade)")]
    [SerializeField] private string nombrePropiedadDither = "_DitherFade";
    [Range(0f, 1f)]
    [SerializeField] private float valorSolido = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float valorDitherBloqueo = 0.25f;
    [SerializeField] private float velocidadTransicion = 10f;

    private readonly Dictionary<Renderer, float> estadosActuales = new Dictionary<Renderer, float>();
    private readonly HashSet<Renderer> bloqueandoEsteFrame = new HashSet<Renderer>();
    private MaterialPropertyBlock propBlock;
    private int propId;

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        propId = Shader.PropertyToID(nombrePropiedadDither);
    }

    private void LateUpdate()
    {
        if (objetivoJugador == null) return;

        bloqueandoEsteFrame.Clear();

        Vector3 inicio = transform.position;
        Vector3 fin = objetivoJugador.position + offsetObjetivo;
        Vector3 direccion = fin - inicio;
        float distancia = direccion.magnitude;

        if (distancia > 0.001f)
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                inicio,
                radioDeteccion,
                direccion / distancia,
                distancia,
                capasObstaculos,
                QueryTriggerInteraction.Ignore
            );

            for (int i = 0; i < hits.Length; i++)
            {
                Renderer rend = hits[i].collider.GetComponentInParent<Renderer>();
                if (rend == null)
                {
                    rend = hits[i].collider.GetComponentInChildren<Renderer>();
                }

                if (rend != null)
                {
                    bloqueandoEsteFrame.Add(rend);
                    if (!estadosActuales.ContainsKey(rend))
                    {
                        estadosActuales[rend] = valorSolido;
                    }
                }
            }
        }

        List<Renderer> renderers = new List<Renderer>(estadosActuales.Keys);
        for (int i = 0; i < renderers.Count; i++)
        {
            Renderer rend = renderers[i];
            if (rend == null)
            {
                estadosActuales.Remove(rend);
                continue;
            }

            float objetivo = bloqueandoEsteFrame.Contains(rend) ? valorDitherBloqueo : valorSolido;
            float nuevoValor = Mathf.MoveTowards(estadosActuales[rend], objetivo, velocidadTransicion * Time.deltaTime);
            estadosActuales[rend] = nuevoValor;

            rend.GetPropertyBlock(propBlock);
            propBlock.SetFloat(propId, nuevoValor);
            rend.SetPropertyBlock(propBlock);
        }
    }
}