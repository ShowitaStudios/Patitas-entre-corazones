using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PuzzlePostItUI : MonoBehaviour
{
    [Header("Referencias Generales")]
    [SerializeField] private PuzzleManager puzzleManager;
    [SerializeField] private PuzzleZonaTrigger zonaTrigger;

    [Header("Configuración del Puzzle Post-It")]
    [SerializeField] private int papelesRequeridos = 10;
    [SerializeField] private float velocidadCaida = 500f;
    [SerializeField] private float rotacionCaida = 180f;

    [Header("Eventos")]
    [SerializeField] private UnityEvent alCompletarPuzzle;

    private int papelesDespegadosActuales = 0;
    private bool yaCompletado = false;
    private List<Button> listaBotonesPapelitos = new List<Button>();

    private void Awake()
    {
        if (puzzleManager == null)
        {
#if UNITY_2023_1_OR_NEWER
            puzzleManager = Object.FindFirstObjectByType<PuzzleManager>();
#else
            puzzleManager = Object.FindObjectOfType<PuzzleManager>();
#endif
        }

        if (zonaTrigger == null)
        {
#if UNITY_2023_1_OR_NEWER
            zonaTrigger = Object.FindFirstObjectByType<PuzzleZonaTrigger>();
#else
            zonaTrigger = Object.FindObjectOfType<PuzzleZonaTrigger>();
#endif
        }
    }

    private void Start()
    {
        Button[] botonesHijos = GetComponentsInChildren<Button>();
        foreach (var boton in botonesHijos)
        {
            listaBotonesPapelitos.Add(boton);
            Button btnActual = boton;
            btnActual.onClick.AddListener(() => DespegarPapelito(btnActual));
        }

        if (papelesRequeridos > listaBotonesPapelitos.Count && listaBotonesPapelitos.Count > 0)
        {
            papelesRequeridos = listaBotonesPapelitos.Count;
        }
    }

    private void DespegarPapelito(Button botonPapel)
    {
        if (yaCompletado) return;

        botonPapel.interactable = false;
        papelesDespegadosActuales++;

        StartCoroutine(AnimacionCaidaPapel(botonPapel.GetComponent<RectTransform>(), botonPapel.gameObject));

        if (papelesDespegadosActuales >= papelesRequeridos)
        {
            CompletarPuzzle();
        }
    }

    private IEnumerator AnimacionCaidaPapel(RectTransform rectPapel, GameObject objetoPapel)
    {
        if (rectPapel == null) yield break;

        float tiempoTranscurrido = 0f;
        float duracionAnimacion = 3f;
        Vector2 posicionInicial = rectPapel.anchoredPosition;
        Vector3 rotacionInicial = rectPapel.eulerAngles;

        Image img = objetoPapel.GetComponent<Image>();
        Color colorInicial = img != null ? img.color : Color.white;

        while (tiempoTranscurrido < duracionAnimacion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / duracionAnimacion;

            float nuevaY = posicionInicial.y - (Mathf.Pow(t, 2) * velocidadCaida);
            float nuevoGiro = rotacionInicial.z + (t * rotacionCaida);

            rectPapel.anchoredPosition = new Vector2(posicionInicial.x, nuevaY);
            rectPapel.localEulerAngles = new Vector3(0, 0, nuevoGiro);

            if (img != null)
            {
                Color c = colorInicial;
                c.a = Mathf.Lerp(1f, 0f, t);
                img.color = c;
            }

            yield return null;
        }

        Destroy(objetoPapel);
    }

    private void CompletarPuzzle()
    {
        yaCompletado = true;

        if (zonaTrigger != null)
        {
            zonaTrigger.RegistrarPuzzleZona();
        }

        alCompletarPuzzle?.Invoke();
        StartCoroutine(CerrarConRetraso());
    }

    private IEnumerator CerrarConRetraso()
    {
        yield return new WaitForSeconds(1.5f);

        if (puzzleManager != null)
        {
            puzzleManager.CerrarPuzzleActual();
        }
    }
}