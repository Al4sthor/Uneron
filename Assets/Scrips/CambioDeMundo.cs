using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeMundo : MonoBehaviour
{
    public GameObject canvasTransicion;   // Canvas negro con texto
    public string nombreEscenaDestino = "Desarrollo"; // nombre de la escena destino
    public float tiempoEspera = 3f;       // segundos antes de cambiar

    public void ActivarTransicion()
    {
        canvasTransicion.SetActive(true);
        Invoke("CambiarEscena", tiempoEspera);
    }

    void CambiarEscena()
    {
        SceneManager.LoadScene(nombreEscenaDestino);
    }
}
