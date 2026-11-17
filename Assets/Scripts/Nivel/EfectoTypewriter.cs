using System.Collections;
using UnityEngine;
using TMPro;

public class EfectoTypewriter : MonoBehaviour
{
    [SerializeField] private TMP_Text textoMeshPro;
    [SerializeField] private float velocidadEscritura = 0.05f;
    [SerializeField, TextArea] private string textoCompleto;

    private Coroutine corrutinaEscritura;

    public void IniciarEfecto()
    {
        // Si el objeto está desactivado, lo activamos para que no rompa la corrutina
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        if (textoMeshPro == null)
        {
            textoMeshPro = GetComponent<TMP_Text>();
        }

        if (textoMeshPro != null)
        {
            if (corrutinaEscritura != null)
            {
                StopCoroutine(corrutinaEscritura);
            }
            corrutinaEscritura = StartCoroutine(EscribirTexto());
        }
    }

    private IEnumerator EscribirTexto()
    {
        textoMeshPro.text = "";
        foreach (char letra in textoCompleto.ToCharArray())
        {
            textoMeshPro.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }
}