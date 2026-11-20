using UnityEngine;

public class PropFlotanteRotacion : MonoBehaviour
{
    public enum EjeRotacion
    {
        EjeY,
        EjeX,
        EjeZ,
        Personalizado
    }

    [Header("Flotación (Arriba y Abajo)")]
    [Tooltip("Activa o desactiva el movimiento vertical suave.")]
    [SerializeField] private bool flotar = true;
    [Tooltip("Distancia máxima que sube y baja desde su posición inicial (en metros).")]
    [SerializeField] private float amplitudFlote = 0.15f;
    [Tooltip("Velocidad del ciclo de flotación.")]
    [SerializeField] private float velocidadFlote = 1.5f;
    [Tooltip("Desfase inicial para que varios props iguales no floten sincronizados.")]
    [SerializeField] private float desfaseFase = 0f;
    [Tooltip("Si está activo, aleatoriza el desfase al iniciar para dar variedad natural.")]
    [SerializeField] private bool desfaseAleatorioAlIniciar = true;

    [Header("Rotación sobre su Propio Eje")]
    [Tooltip("Activa o desactiva la rotación continua.")]
    [SerializeField] private bool rotar = true;
    [SerializeField] private EjeRotacion ejeRotacion = EjeRotacion.EjeY;
    [Tooltip("Eje usado solo si 'Eje Rotacion' está en Personalizado.")]
    [SerializeField] private Vector3 ejePersonalizado = Vector3.up;
    [Tooltip("Velocidad de giro en grados por segundo.")]
    [SerializeField] private float velocidadRotacion = 20f;
    [SerializeField] private Space espacioRotacion = Space.Self;

    private Vector3 posicionInicial;
    private float tiempoFlote;

    private void Awake()
    {
        posicionInicial = transform.position;

        if (desfaseAleatorioAlIniciar)
        {
            desfaseFase = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    private void Update()
    {
        if (flotar)
        {
            tiempoFlote += Time.deltaTime * velocidadFlote;
            float offsetY = Mathf.Sin(tiempoFlote + desfaseFase) * amplitudFlote;
            transform.position = new Vector3(
                posicionInicial.x,
                posicionInicial.y + offsetY,
                posicionInicial.z
            );
        }

        if (rotar)
        {
            Vector3 eje = ObtenerEje();
            transform.Rotate(eje, velocidadRotacion * Time.deltaTime, espacioRotacion);
        }
    }

    private Vector3 ObtenerEje()
    {
        return ejeRotacion switch
        {
            EjeRotacion.EjeX => Vector3.right,
            EjeRotacion.EjeY => Vector3.up,
            EjeRotacion.EjeZ => Vector3.forward,
            EjeRotacion.Personalizado => ejePersonalizado.sqrMagnitude > 0.0001f ? ejePersonalizado.normalized : Vector3.up,
            _ => Vector3.up
        };
    }
    public void ActualizarPosicionBase(Vector3 nuevaPosicionBase)
    {
        posicionInicial = nuevaPosicionBase;
    }
}