using UnityEngine;

public class PesoZone : MonoBehaviour
{
    [Range(0.1f, 1f)]
    [SerializeField] private float speedFactor = 0.4f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MascotaController>(out MascotaController controller))
        {
            controller.SetSpeedMultiplier(speedFactor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MascotaController>(out MascotaController controller))
        {
            controller.ResetSpeedMultiplier();
        }
    }
}