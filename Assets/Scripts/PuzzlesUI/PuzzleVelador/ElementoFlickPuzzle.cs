using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class ElementoFlickPuzzle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Lanzamiento / Flick")]
    [Tooltip("Velocidad mínima (px/seg) al soltar para considerar que hubo un lanzamiento rápido.")]
    [SerializeField] private float velocidadMinimaLanzamiento = 900f;
    [Tooltip("Multiplicador aplicado a la velocidad del gesto al soltar.")]
    [SerializeField] private float multiplicadorImpulso = 1.15f;
    [Tooltip("Velocidad máxima de lanzamiento en px/seg para evitar disparos extremos.")]
    [SerializeField] private float velocidadMaximaLanzamiento = 3800f;
    [Tooltip("Desaceleración gradual al volar (0 = sin frenado, mayor = frena más rápido).")]
    [SerializeField] private float frenadoAire = 1.5f;

    [Header("Al Salir del Área")]
    [SerializeField] private bool desactivarAlSalir = true;

    private RectTransform rectTransform;
    private Canvas canvasPadre;
    private PuzzleFlickManager manager;

    private Vector2 posicionInicial;
    private Vector2 velocidadActual;
    private Vector2 ultimaDireccionValida = Vector2.up;

    private bool referenciasInicializadas = false;
    private bool estaArrastrando = false;
    private bool lanzado = false;
    private bool yaSalio = false;

    private void Awake()
    {
        AsegurarReferencias();
    }

    private void AsegurarReferencias()
    {
        if (referenciasInicializadas && rectTransform != null) return;

        rectTransform = GetComponent<RectTransform>();
        canvasPadre = GetComponentInParent<Canvas>();
        if (rectTransform != null)
        {
            posicionInicial = rectTransform.anchoredPosition;
            referenciasInicializadas = true;
        }
    }

    public void Inicializar(PuzzleFlickManager managerPuzzle)
    {
        AsegurarReferencias();
        manager = managerPuzzle;
        if (rectTransform != null)
        {
            posicionInicial = rectTransform.anchoredPosition;
        }
    }

    public void ReiniciarElemento()
    {
        AsegurarReferencias();
        estaArrastrando = false;
        lanzado = false;
        yaSalio = false;
        velocidadActual = Vector2.zero;
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = posicionInicial;
        }
        gameObject.SetActive(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (yaSalio) return;
        AsegurarReferencias();

        estaArrastrando = true;
        lanzado = false;
        velocidadActual = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (yaSalio || !estaArrastrando) return;
        AsegurarReferencias();

        float escala = canvasPadre != null ? canvasPadre.scaleFactor : 1f;
        Vector2 deltaUI = eventData.delta / escala;
        rectTransform.anchoredPosition += deltaUI;

        float dt = Mathf.Max(Time.unscaledDeltaTime, 0.0001f);
        Vector2 velocidadInstantanea = deltaUI / dt;

        if (deltaUI.sqrMagnitude > 0.0001f)
        {
            ultimaDireccionValida = deltaUI.normalized;
        }

        velocidadActual = Vector2.Lerp(velocidadActual, velocidadInstantanea, 0.65f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (yaSalio || !estaArrastrando) return;

        estaArrastrando = false;

        float rapidez = velocidadActual.magnitude;
        if (rapidez >= velocidadMinimaLanzamiento)
        {
            Vector2 direccion = rapidez > 0.01f ? velocidadActual.normalized : ultimaDireccionValida;
            float rapidezAjustada = Mathf.Clamp(rapidez * multiplicadorImpulso, velocidadMinimaLanzamiento, velocidadMaximaLanzamiento);
            velocidadActual = direccion * rapidezAjustada;
            lanzado = true;
        }
        else
        {
            velocidadActual = Vector2.zero;
            lanzado = false;
            ComprobarSiSalioDelArea();
        }
    }

    private void Update()
    {
        if (yaSalio || estaArrastrando) return;

        if (lanzado)
        {
            AsegurarReferencias();
            float dt = Time.unscaledDeltaTime;
            rectTransform.anchoredPosition += velocidadActual * dt;

            if (frenadoAire > 0f)
            {
                velocidadActual = Vector2.Lerp(velocidadActual, Vector2.zero, frenadoAire * dt);
            }

            ComprobarSiSalioDelArea();

            if (velocidadActual.magnitude < 40f && !yaSalio)
            {
                lanzado = false;
                velocidadActual = Vector2.zero;
            }
        }
    }

    private void ComprobarSiSalioDelArea()
    {
        if (yaSalio || manager == null || manager.AreaPanel == null) return;
        AsegurarReferencias();

        RectTransform panel = manager.AreaPanel;
        float margen = manager.MargenSalida;

        if (EstaFueraDeRect(rectTransform, panel, margen) || EstaFueraDePantalla(rectTransform, margen))
        {
            yaSalio = true;
            lanzado = false;
            velocidadActual = Vector2.zero;

            manager.NotificarElementoFuera(this);

            if (desactivarAlSalir)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private bool EstaFueraDeRect(RectTransform objetivo, RectTransform contenedor, float margen)
    {
        Vector3[] esquinasObj = new Vector3[4];
        Vector3[] esquinasCont = new Vector3[4];

        objetivo.GetWorldCorners(esquinasObj);
        contenedor.GetWorldCorners(esquinasCont);

        float minX = esquinasCont[0].x - margen;
        float maxX = esquinasCont[2].x + margen;
        float minY = esquinasCont[0].y - margen;
        float maxY = esquinasCont[2].y + margen;

        Vector3 centro = objetivo.position;
        bool centroFuera = centro.x < minX || centro.x > maxX || centro.y < minY || centro.y > maxY;

        bool todasEsquinasFuera = true;
        for (int i = 0; i < 4; i++)
        {
            if (esquinasObj[i].x >= minX && esquinasObj[i].x <= maxX &&
                esquinasObj[i].y >= minY && esquinasObj[i].y <= maxY)
            {
                todasEsquinasFuera = false;
                break;
            }
        }

        return centroFuera || todasEsquinasFuera;
    }

    private bool EstaFueraDePantalla(RectTransform objetivo, float margen)
    {
        Vector3 pos = objetivo.position;
        return pos.x < -margen || pos.x > Screen.width + margen ||
               pos.y < -margen || pos.y > Screen.height + margen;
    }
}