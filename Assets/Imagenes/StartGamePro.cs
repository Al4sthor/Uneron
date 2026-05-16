using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartGamePro : MonoBehaviour
{
    // este es el audio que va a sonar cuando le de start
    public AudioSource audioSource;

    // aqui escribo el nombre exacto de la escena a la que quiero ir
    public string nombreEscena;

    // esta funcion la llamo desde el boton
    public void IniciarJuego()
    {
        // inicio una corrutina pa poder esperar mientras suena el audio
        StartCoroutine(CambiarEscena());
    }

    IEnumerator CambiarEscena()
    {
        // primero hago sonar el audio del boton
        audioSource.Play();

        // aqui espero a que el sonido termine completo
        yield return new WaitForSeconds(audioSource.clip.length);

        // despues de que suena todo ya cambio a la otra escena
        SceneManager.LoadScene(nombreEscena);
    }
}