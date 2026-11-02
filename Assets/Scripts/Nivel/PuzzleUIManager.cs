using UnityEngine;
using TMPro;
using System.Collections;

public class PuzzleUIManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI textoPuzzleUI;
    [SerializeField] private float duracionVisibilidad = 4f;

    public static PuzzleUIManager Instance { get; private set; }

    private Coroutine rutinaOcultar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (textoPuzzleUI != null)
        {
            textoPuzzleUI.gameObject.SetActive(false);
        }
    }

    public void ActualizarTextoUI(string nombreZona, int puzlesActuales, int totalPuzles)
    {
        if (textoPuzzleUI == null) return;

        textoPuzzleUI.text = $"Área: {nombreZona} {puzlesActuales}/{totalPuzles} puzzles completados";
        textoPuzzleUI.gameObject.SetActive(true);

        if (rutinaOcultar != null)
        {
            StopCoroutine(rutinaOcultar);
        }

        rutinaOcultar = StartCoroutine(RutinaOcultarTexto());
    }

    private IEnumerator RutinaOcultarTexto()
    {
        yield return new WaitForSeconds(duracionVisibilidad);
        if (textoPuzzleUI != null)
        {
            textoPuzzleUI.gameObject.SetActive(false);
        }
    }
}