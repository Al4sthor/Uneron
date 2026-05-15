using UnityEngine;
using UnityEngine.SceneManagement;

// Pon este script en un Empty GameObject.
// Agregale un Collider (Box Collider esta bien) y marca "Is Trigger" en true.

public class CambiarEscena : MonoBehaviour
{
    [Header("Escena destino")]
    public string nombreEscena = "NombreDeTuEscena"; // Escribe aqui el nombre exacto de tu escena

    [Header("Solo el jugador activa el trigger?")]
    public bool soloJugador = true; // Si es true, solo el tag "Player" lo activa

    void OnTriggerEnter(Collider other)
    {
        if (soloJugador && !other.CompareTag("Player")) return;

        SceneManager.LoadScene(nombreEscena);
    }
}