using UnityEngine;

public class ZonaEnemigo : MonoBehaviour
{
    public enemigoSeguimiento enemigo; // aqui arrastro el enemigo

    void OnTriggerEnter(Collider other)
    {
        // si entra el jugador
        if (other.CompareTag("Player"))
        {
            enemigo.activado = true; // despertamos al enemigo
        }
    }
}