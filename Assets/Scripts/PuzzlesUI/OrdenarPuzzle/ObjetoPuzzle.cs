using UnityEngine;
using UnityEngine.InputSystem;

public class ObjetoPuzzle : MonoBehaviour
{
    [Header("Identificador del Puzzle")]
    [Tooltip("El nombre exacto registrado en el PuzzleManager")]
    [SerializeField] private string puzzleID;

    [Header("Referencias y Configuración")]
    [SerializeField] private PuzzleManager puzzleManager;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private float raycastHeightOffset = 0.3f;
    [SerializeField] private LayerMask playerLayer;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (IsPlayerNearby() && puzzleManager != null)
            {
                puzzleManager.AbrirPuzzlePorNombre(puzzleID);
            }
        }
    }

    private bool IsPlayerNearby()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * raycastHeightOffset;
        Collider[] hitColliders = Physics.OverlapSphere(rayOrigin, interactDistance, playerLayer);
        return hitColliders.Length > 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 rayOrigin = transform.position + Vector3.up * raycastHeightOffset;
        Gizmos.DrawWireSphere(rayOrigin, interactDistance);
    }
}