using System.Collections;
using UnityEngine;

public class RompecabezasUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PuzzleManager puzzleManager;

    [Header("Configuración del Puzzle")]
    [SerializeField] private int totalPiezas = 4;
    [SerializeField] private float tiempoParaCerrar = 1.5f;

    [Header("Progreso del Nivel")]
    [Tooltip("Marca si completar este puzzle UI suma al progreso general del nivel")]
    [SerializeField] private bool cuentaComoObjetivoNivel = true;

    private int piezasCorrectas = 0;
    private bool yaCompletado = false;

    private void OnEnable()
    {
        if (!yaCompletado)
        {
            piezasCorrectas = 0;
        }
    }

    public void PiezaEncajada()
    {
        if (yaCompletado) return;

        piezasCorrectas++;

        if (piezasCorrectas >= totalPiezas)
        {
            yaCompletado = true;
            if (cuentaComoObjetivoNivel && LevelProgressManager.Instance != null)
            {
                LevelProgressManager.Instance.RegistrarPuzzleResuelto();
            }

            StartCoroutine(CerrarConRetraso());
        }
    }

    private IEnumerator CerrarConRetraso()
    {
        yield return new WaitForSeconds(tiempoParaCerrar);

        if (puzzleManager != null)
        {
            puzzleManager.CerrarPuzzleActual();
        }
    }
}