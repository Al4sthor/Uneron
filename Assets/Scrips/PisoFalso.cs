using UnityEngine;

public class PisoFalso : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
          Destroy(gameObject);
        }
    }
}
