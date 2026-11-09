using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class MenuPausa : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private TMP_Dropdown dropdownResoluciones;
    [SerializeField] private Slider sliderVolumen;

    [Header("Configuración de Escenas")]
    [SerializeField] private string MainMenuScene = "MenuPrincipal";

    private bool juegoPausado = false;
    private Resolution[] resolucionesDisponibles;

    private void Start()
    {
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }

        InicializarResoluciones();

        if (sliderVolumen != null)
        {
            sliderVolumen.value = AudioListener.volume;
            sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
        }
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            if (UnityEngine.InputSystem.Keyboard.current.tabKey.wasPressedThisFrame)
            {
                if (juegoPausado)
                {
                    Reanudar();
                }
                else
                {
                    Pausar();
                }
            }
        }
    }

    public void Reanudar()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        Time.timeScale = 1f;
        juegoPausado = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Pausar()
    {
        if (panelPausa != null) panelPausa.SetActive(true);
        Time.timeScale = 0f;
        juegoPausado = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void InicializarResoluciones()
    {
        if (dropdownResoluciones == null) return;

        resolucionesDisponibles = Screen.resolutions;
        dropdownResoluciones.ClearOptions();

        List<string> opciones = new List<string>();
        int resolucionActualIndex = 0;

        for (int i = 0; i < resolucionesDisponibles.Length; i++)
        {
            string opcion = resolucionesDisponibles[i].width + " x " + resolucionesDisponibles[i].height;
            opciones.Add(opcion);

            if (resolucionesDisponibles[i].width == Screen.currentResolution.width &&
                resolucionesDisponibles[i].height == Screen.currentResolution.height)
            {
                resolucionActualIndex = i;
            }
        }

        dropdownResoluciones.AddOptions(opciones);
        dropdownResoluciones.value = resolucionActualIndex;
        dropdownResoluciones.RefreshShownValue();

        dropdownResoluciones.onValueChanged.AddListener(CambiarResolucion);
    }

    public void CambiarResolucion(int indiceResolucion)
    {
        Resolution resolucion = resolucionesDisponibles[indiceResolucion];
        Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);
    }

    public void CambiarVolumen(float volumen)
    {
        AudioListener.volume = volumen;
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuScene);
    }
}