using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjetoRompible : MonoBehaviour
{
    [Header("Configuración de Fragilidad")]
    [SerializeField] private bool esRompible = true;
    [SerializeField] private float velocidadMinimaRuptura = 3.0f;

    [Header("Identificador para Persistencia")]
    [Tooltip("Escribe un ID único para este objeto si quieres que no vuelva a aparecer al cambiar de escena (Ej: 'Florero_Sala_01')")]
    [SerializeField] private string idUnicoObjeto;

    [Header("Progreso del Nivel")]
    [Tooltip("Marca esta casilla si romper este objeto cuenta como uno de los objetivos del nivel")]
    [SerializeField] private bool cuentaComoObjetivoNivel = false;

    [Header("Efectos al Romperse")]
    [SerializeField] private GameObject prefabObjetoRoto;
    [SerializeField] private AudioClip sonidoRuptura;
    [SerializeField] private float volumenSonido = 1.0f;

    private Rigidbody rb;
    private bool yaSeRompio = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(idUnicoObjeto) && GameManager.Instance != null)
        {
            if (GameManager.Instance.EsPuzzleCompletado(idUnicoObjeto))
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision colision)
    {
        if (!esRompible || yaSeRompio) return;

        if (colision.relativeVelocity.magnitude >= velocidadMinimaRuptura)
        {
            RomperObjeto();
        }
    }

    public void RomperObjeto()
    {
        if (yaSeRompio) return;
        yaSeRompio = true;
        
        if (!string.IsNullOrEmpty(idUnicoObjeto) && GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarPuzzleCompletado(idUnicoObjeto);
        }
        
        if (cuentaComoObjetivoNivel && LevelProgressManager.Instance != null)
        {
            LevelProgressManager.Instance.RegistrarPuzzleResuelto();
        }
        
        if (sonidoRuptura != null)
        {
            AudioSource.PlayClipAtPoint(sonidoRuptura, transform.position, volumenSonido);
        }
        
        if (prefabObjetoRoto != null)
        {
            Instantiate(prefabObjetoRoto, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}