using UnityEngine;

public class InteraccionLuz : MonoBehaviour
{
    public Light luz;
    private bool jugadorCerca = false;

    void Start()
    {
        // Apagar la luz al inicio
        luz.enabled = false;
    }

    void Update()
    {
        // Presionar R para alternar luz solo si estás cerca
        if (jugadorCerca && Input.GetKeyDown(KeyCode.R))
        {
            luz.enabled = !luz.enabled;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}