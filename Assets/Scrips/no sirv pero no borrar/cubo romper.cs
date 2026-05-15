using UnityEngine;

public class CaerYRomper : MonoBehaviour
{
    private Rigidbody rb;
    private bool jugadorCerca = false;
    private bool activado = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Empieza flotando
        rb.isKinematic = true;
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.Y) && !activado)
        {
            activado = true;
            rb.isKinematic = false; // Ahora cae
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (activado && collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // Se rompe al tocar el suelo
        }
    }
}