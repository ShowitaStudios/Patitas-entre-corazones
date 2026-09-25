using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleManager : MonoBehaviour
{
    [System.Serializable]
    public struct PuzzlePanelData
    {
        public string puzzleID;
        public GameObject panelObject;
    }

    [Header("Referencias")]
    [SerializeField] private MascotaController jugadorController;

    [Header("Registro de Puzzles")]
    [SerializeField] private List<PuzzlePanelData> listaPuzzles;

    private Dictionary<string, GameObject> diccionarioPuzzles = new Dictionary<string, GameObject>();
    private GameObject puzzleActual;
    private bool puzzleActivo = false;

    // Propiedad pública para que la cámara pueda consultar si hay un puzzle activo
    public bool PuzzleActivo => puzzleActivo;

    private void Awake()
    {
        foreach (var data in listaPuzzles)
        {
            if (!string.IsNullOrEmpty(data.puzzleID) && data.panelObject != null)
            {
                if (!diccionarioPuzzles.ContainsKey(data.puzzleID))
                {
                    diccionarioPuzzles.Add(data.puzzleID, data.panelObject);
                    data.panelObject.SetActive(false);
                }
            }
        }
    }

    private void Update()
    {
        if (puzzleActivo && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CerrarPuzzleActual();
        }
    }

    public void AbrirPuzzlePorNombre(string nombrePuzzle)
    {
        if (diccionarioPuzzles.TryGetValue(nombrePuzzle, out GameObject panel))
        {
            puzzleActual = panel;
            puzzleActual.SetActive(true);
            puzzleActivo = true;

            ConfigurarEstadoJuego(true);
        }
        else
        {
            Debug.LogWarning($"El puzzle con ID '{nombrePuzzle}' no está registrado en el PuzzleManager.");
        }
    }

    public void CerrarPuzzleActual()
    {
        if (puzzleActual != null)
        {
            puzzleActual.SetActive(false);
            puzzleActual = null;
        }

        puzzleActivo = false;
        ConfigurarEstadoJuego(false);
    }

    private void ConfigurarEstadoJuego(bool enPuzzle)
    {
        if (jugadorController != null)
        {
            jugadorController.enabled = !enPuzzle;
        }

        // El Manager solo se encarga del cursor
        if (enPuzzle)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}