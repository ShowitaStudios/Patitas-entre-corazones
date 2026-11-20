using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LamparaPuzzleTrigger : MonoBehaviour
{
    [Header("Detección")]
    [SerializeField] private string tagLampara = "Lampara";
    [Tooltip("Si está activo, cada lámpara solo se cuenta una vez.")]
    [SerializeField] private bool contarSoloUnaVezPorLampara = true;
    [Tooltip("Si está activo, el trigger se desactiva tras contar una lámpara.")]
    [SerializeField] private bool desactivarAlContar = false;

    [Header("Progreso de Puzzles")]
    [Tooltip("Zona de puzzles a sumar. Si queda vacío, busca un PuzzleZonaTrigger en la escena.")]
    [SerializeField] private PuzzleZonaTrigger zonaObjetivo;
    [SerializeField] private bool mostrarProgresoEnUI = true;

    private readonly System.Collections.Generic.HashSet<int> lamparasContadas = new System.Collections.Generic.HashSet<int>();

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }

        if (zonaObjetivo == null)
        {
#if UNITY_2023_1_OR_NEWER
            zonaObjetivo = Object.FindFirstObjectByType<PuzzleZonaTrigger>(FindObjectsInactive.Include);
#else
            zonaObjetivo = Object.FindObjectOfType<PuzzleZonaTrigger>(true);
#endif
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagLampara)) return;

        int idObjeto = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject.GetInstanceID()
            : other.gameObject.GetInstanceID();

        if (contarSoloUnaVezPorLampara && !lamparasContadas.Add(idObjeto))
        {
            return;
        }

        if (zonaObjetivo == null)
        {
#if UNITY_2023_1_OR_NEWER
            zonaObjetivo = Object.FindFirstObjectByType<PuzzleZonaTrigger>(FindObjectsInactive.Include);
#else
            zonaObjetivo = Object.FindObjectOfType<PuzzleZonaTrigger>(true);
#endif
        }

        if (zonaObjetivo != null)
        {
            zonaObjetivo.RegistrarPuzzleZona(mostrarProgresoEnUI);
        }

        if (desactivarAlContar)
        {
            gameObject.SetActive(false);
        }
    }
}