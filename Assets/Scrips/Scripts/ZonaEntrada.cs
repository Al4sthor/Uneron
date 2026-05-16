using UnityEngine;

public class ZonaEntrada : MonoBehaviour
{
    public Collider terrenoCollider;
    public Collider personajeCollider;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Physics.IgnoreCollision(personajeCollider, terrenoCollider, true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Physics.IgnoreCollision(personajeCollider, terrenoCollider, false);
        }
    }
}