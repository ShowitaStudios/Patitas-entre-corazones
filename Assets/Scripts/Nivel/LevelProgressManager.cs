using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelProgressManager : MonoBehaviour
{
    [System.Serializable]
    public struct AccionSecuencial
    {
        [Tooltip("Tiempo de espera en segundos antes de ejecutar esta acción (desde que inició el hito o desde la acción anterior)")]
        public float delayAntesDeAccion;
        public UnityEvent eventoAccion;
    }

    [System.Serializable]
    public struct HitoProgreso
    {
        public int puzzlesObjetivo;
        [Tooltip("Lista de eventos que se ejecutarán en secuencia con su propio delay")]
        public List<AccionSecuencial> secuenciaAcciones;
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

                if (esCargaInicial)
                {
                    EjecutarSecuenciaInstantanea(hito.secuenciaAcciones);
                }
                else
                {
                    StartCoroutine(EjecutarSecuenciaConDelays(hito.secuenciaAcciones));
                }
            }
            else if (puzzlesCompletados >= hito.puzzlesObjetivo && esCargaInicial && !hito.yaEjecutado)
            {
                hito.yaEjecutado = true;
                hitosNivel[i] = hito;
                EjecutarSecuenciaInstantanea(hito.secuenciaAcciones);
            }
        }
    }

    private IEnumerator EjecutarSecuenciaConDelays(List<AccionSecuencial> secuencia)
    {
        foreach (var accion in secuencia)
        {
            if (accion.delayAntesDeAccion > 0f)
            {
                yield return new WaitForSeconds(accion.delayAntesDeAccion);
            }
            accion.eventoAccion?.Invoke();
        }
    }

    private void EjecutarSecuenciaInstantanea(List<AccionSecuencial> secuencia)
    {
        foreach (var accion in secuencia)
        {
            accion.eventoAccion?.Invoke();
        }
    }
}