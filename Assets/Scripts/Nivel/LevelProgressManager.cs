using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelProgressManager : MonoBehaviour
{
    [System.Serializable]
    public struct HitoProgreso
    {
        public int puzzlesObjetivo;
        public UnityEvent eventoHito;
        [HideInInspector] public bool yaEjecutado;
    }

    [Header("Configuración del Nivel")]
    [SerializeField] private string idNivel = "Progreso_Nivel_1";
    [SerializeField] private List<HitoProgreso> hitosNivel = new List<HitoProgreso>();
    [SerializeField] private int puzzlesCompletados = 0;

    public static LevelProgressManager Instance { get; private set; }

    public int PuzzlesResueltos => puzzlesCompletados;

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
        CargarProgresoPersistente();
    }

    private void CargarProgresoPersistente()
    {
        if (GameManager.Instance != null)
        {
            puzzlesCompletados = GameManager.Instance.ObtenerProgresoNivel(idNivel);
            EvaluarHitos(true);
        }
    }

    public void RegistrarPuzzleResuelto()
    {
        puzzlesCompletados++;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GuardarProgresoNivel(idNivel, puzzlesCompletados);
        }

        EvaluarHitos(false);
    }

    private void EvaluarHitos(bool esCargaInicial)
    {
        for (int i = 0; i < hitosNivel.Count; i++)
        {
            var hito = hitosNivel[i];

            if (puzzlesCompletados >= hito.puzzlesObjetivo && !hito.yaEjecutado)
            {
                hito.yaEjecutado = true;
                hitosNivel[i] = hito;
                hito.eventoHito?.Invoke();
            }
            else if (puzzlesCompletados >= hito.puzzlesObjetivo && esCargaInicial && !hito.yaEjecutado)
            {
                hito.yaEjecutado = true;
                hitosNivel[i] = hito;
                hito.eventoHito?.Invoke();
            }
        }
    }
}