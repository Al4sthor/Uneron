using UnityEngine;

public class ActivarCinematica : MonoBehaviour
{
    public GameObject camaraCinematica;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            camaraCinematica.SetActive(true);
        }
    }
}