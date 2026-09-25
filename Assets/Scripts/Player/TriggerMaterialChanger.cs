using UnityEngine;

public class TriggerOutlineChanger : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El Renderer del objeto que tiene los materiales asignados")]
    [SerializeField] private Renderer objectRenderer;

    [Tooltip("El material de Outline que se asignará en el segundo slot (Element 1)")]
    [SerializeField] private Material outlineMaterial;

    [Header("Filtro")]
    [Tooltip("Tag del jugador para activar el Trigger")]
    [SerializeField] private string playerTag = "Player";

    private Material baseMaterial;

    private void Start()
    {
        if (objectRenderer != null)
        {
            baseMaterial = objectRenderer.materials[0];
            SetOutlineActive(false);
        }
        else
        {
            Debug.LogWarning("No se asignó ningún Renderer en " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            SetOutlineActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            SetOutlineActive(false);
        }
    }

    private void SetOutlineActive(bool active)
    {
        if (objectRenderer == null || outlineMaterial == null) return;

        if (active)
        {
            objectRenderer.materials = new Material[] { baseMaterial, outlineMaterial };
        }
        else
        {
            objectRenderer.materials = new Material[] { baseMaterial };
        }
    }
}