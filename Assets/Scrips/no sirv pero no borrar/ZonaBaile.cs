using UnityEngine;

public class ZonaBaile : MonoBehaviour
{
    public EnemigoBailarin enemigo; // arrastro aqui el enemigo bailarín

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemigo.activado = true; // apenas entro a la zona el man empieza su show de baile
        }
    }
}