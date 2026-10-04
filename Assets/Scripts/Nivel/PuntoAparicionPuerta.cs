using UnityEngine;

public class PuntoAparicionPuerta : MonoBehaviour
{
    [SerializeField] private string idPuerta;

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.idPuertaEntrada == idPuerta)
        {
            GameObject jugador = GameObject.FindWithTag("Player");

            if (jugador != null)
            {
                CharacterController cc = jugador.GetComponent<CharacterController>();
                Rigidbody rb = jugador.GetComponent<Rigidbody>();
                
                if (cc != null) cc.enabled = false;
                if (rb != null) rb.isKinematic = true;

                jugador.transform.position = transform.position;
                jugador.transform.rotation = transform.rotation;

                if (cc != null) cc.enabled = true;
                if (rb != null) rb.isKinematic = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 1.5f);
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}