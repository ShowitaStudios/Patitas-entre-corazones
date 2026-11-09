using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class EfectoTypewriter : MonoBehaviour
{
    [Header("Configuración de Texto")]
    [TextArea(3, 10)]
    [SerializeField] private string textoAImprimir = "Test Texto Alex XDDD";
    [SerializeField] private float velocidadEscritura = 0.05f;

    private TextMeshProUGUI componenteTexto;

    private void Awake()
    {
        componenteTexto = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        IniciarEfecto();
    }

    public void IniciarEfecto()
    {
        StopAllCoroutines();
        StartCoroutine(MostrarTextoLetraPorLetra());
    }

    private IEnumerator MostrarTextoLetraPorLetra()
    {
        componenteTexto.text = "";

        foreach (char letra in textoAImprimir.ToCharArray())
        {
            componenteTexto.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }
}