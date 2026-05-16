using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    // nombre de la escena real del gameplay
    public string escenaACargar = "juego";

    // barra visual de progreso
    public Slider barra;

    // tiempo minimo que quiero que dure el loading pa que se vea pro
    public float tiempoMinimo = 3f;

    void Start()
    {
        // apenas entro a la escena empiezo a cargar todo
        StartCoroutine(CargarEscena());
    }

    IEnumerator CargarEscena()
    {
        // empiezo a cargar la escena en segundo plano
        AsyncOperation operacion = SceneManager.LoadSceneAsync(escenaACargar);

        // evito que entre inmediatamente
        operacion.allowSceneActivation = false;

        float tiempo = 0f;

        while (!operacion.isDone)
        {
            // progreso real de carga
            float progresoReal = Mathf.Clamp01(operacion.progress / 0.9f);

            // sumo tiempo para forzar que dure
            tiempo += Time.deltaTime;

            // progreso visual mezclando tiempo y carga real
            float progresoVisual = Mathf.Clamp01(tiempo / tiempoMinimo);

            // actualizo la barra
            barra.value = Mathf.Min(progresoReal, progresoVisual);

            // cuando ya cargo todo y paso el tiempo minimo
            if (operacion.progress >= 0.9f && tiempo >= tiempoMinimo)
            {
                // ahora si dejo entrar al gameplay
                operacion.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}