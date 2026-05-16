using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class ControlVideo : MonoBehaviour
{
    [Header("Referencias")]
    public VideoPlayer videoPlayer;
    public CanvasGroup canvasVideo;
    public float velocidadFade = 1.5f;

    [Header("Canvas a ocultar durante el video")]
    public GameObject[] canvasOcultar;

    [Header("Boss")]
    public GameObject bossObject;

    [Header("Eventos")]
    public UnityEngine.Events.UnityEvent onVideoTerminado;

    private bool videoActivo = false;

    public void ReproducirVideo()
    {
        StartCoroutine(IniciarVideo());
    }

    IEnumerator IniciarVideo()
    {
        videoActivo = true;

        // Ocultar canvas
        foreach (GameObject c in canvasOcultar)
            if (c != null) c.SetActive(false);

        // Activar canvas video
        if (canvasVideo != null)
        {
            canvasVideo.gameObject.SetActive(true);
            canvasVideo.alpha = 0f;
        }

        // Subir Sort Order para que quede encima
        Canvas canvas = canvasVideo.GetComponent<Canvas>();
        if (canvas != null) canvas.sortingOrder = 10;

        // Preparar el video
        videoPlayer.Prepare();

        // Esperar que este listo
        while (!videoPlayer.isPrepared)
            yield return new WaitForSecondsRealtime(0.1f);

        // Congelar el juego
        Time.timeScale = 0f;

        // Fade in
        yield return StartCoroutine(Fade(0f, 1f));

        // Reproducir video
        videoPlayer.Play();

        // Esperar que termine
        while (videoPlayer.isPlaying)
            yield return new WaitForSecondsRealtime(0.1f);

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f));

        // Desactivar canvas video
        if (canvasVideo != null)
            canvasVideo.gameObject.SetActive(false);

        // Volver a mostrar canvas
        foreach (GameObject c in canvasOcultar)
            if (c != null) c.SetActive(true);

        // Activar el boss
        if (bossObject != null)
            bossObject.SetActive(true);

        // Descongelar
        Time.timeScale = 1f;
        videoActivo = false;

        onVideoTerminado?.Invoke();
    }

    IEnumerator Fade(float desde, float hasta)
    {
        float timer = 0f;
        while (timer < velocidadFade)
        {
            timer += Time.unscaledDeltaTime;
            canvasVideo.alpha = Mathf.Lerp(desde, hasta, timer / velocidadFade);
            yield return null;
        }
        canvasVideo.alpha = hasta;
    }
}