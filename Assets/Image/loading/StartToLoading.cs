using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartToLoading : MonoBehaviour
{
    // audio del boton start
    public AudioSource audioSource;

    // tiempo que quiero esperar antes de cambiar de escena
    public float tiempoEspera = 1.2f;

    public void IrACargando()
    {
        // inicio corrutina pa que el sonido alcance a sonar bien
        StartCoroutine(CambiarEscena());
    }

    IEnumerator CambiarEscena()
    {
        // primero suena el click del boton
        audioSource.Play();

        // espero un momento pa que no se corte
        yield return new WaitForSeconds(tiempoEspera);

        // ahora si entro a la pantalla de cargando
        SceneManager.LoadScene("LoadingScene");
    }
}