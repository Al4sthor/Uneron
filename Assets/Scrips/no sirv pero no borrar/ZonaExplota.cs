using UnityEngine;

public class ZonaExplota : MonoBehaviour
{
    public static bool jugadorEnZona = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = false;
        }
    }
}