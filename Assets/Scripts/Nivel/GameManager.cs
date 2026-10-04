using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Transición de Puertas entre Escenas")]
    [Tooltip("ID de la puerta por la que debe aparecer el jugador al cargar la escena")]
    public string idPuertaEntrada;
    private HashSet<string> puzzlesCompletados = new HashSet<string>();
    private Dictionary<string, int> progresoNiveles = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #region Persistencia de Puzles Únicos (HashSet)
    public void RegistrarPuzzleCompletado(string idPuzzle)
    {
        if (string.IsNullOrEmpty(idPuzzle)) return;

        if (!puzzlesCompletados.Contains(idPuzzle))
        {
            puzzlesCompletados.Add(idPuzzle);
        }
    }
    public bool EsPuzzleCompletado(string idPuzzle)
    {
        if (string.IsNullOrEmpty(idPuzzle)) return false;
        return puzzlesCompletados.Contains(idPuzzle);
    }
    #endregion
    #region Persistencia de Progreso Por Nivel (Dictionary)
    public void GuardarProgresoNivel(string idNivel, int cantidad)
    {
        if (string.IsNullOrEmpty(idNivel)) return;

        if (progresoNiveles.ContainsKey(idNivel))
        {
            progresoNiveles[idNivel] = cantidad;
        }
        else
        {
            progresoNiveles.Add(idNivel, cantidad);
        }
    }
    public int ObtenerProgresoNivel(string idNivel)
    {
        if (string.IsNullOrEmpty(idNivel)) return 0;

        if (progresoNiveles.TryGetValue(idNivel, out int cantidad))
        {
            return cantidad;
        }
        return 0;
    }
    #endregion
    #region Gestión de Partida
    public void LimpiarProgresoTotal()
    {
        puzzlesCompletados.Clear();
        progresoNiveles.Clear();
        idPuertaEntrada = string.Empty;
    }
    #endregion
}