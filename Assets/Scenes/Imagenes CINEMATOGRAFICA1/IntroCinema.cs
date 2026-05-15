using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class IntroCinematica : MonoBehaviour
{
    [Header("Imágenes en orden")]
    public Sprite[] imagenes;
    public Image displayImage;

    [Header("Audio")]
    public AudioSource musicaFondo;
    public AudioSource vozNarrador;
    public AudioClip[] clipsNarracion;
    public AudioClip[] clipsExtraNarracion;

    [Header("Configuración")]
    public float duracionFade = 1f;
    public float esperaMinima = 0.5f;
    public string escenaSiguiente = "Nivel1";

    [Header("Indicador en pantalla")]
    public GameObject indicadorContinuar;

    private int indiceActual = 0;
    private bool puedeAvanzar = false;
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = displayImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = displayImage.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0;
        if (indicadorContinuar != null) indicadorContinuar.SetActive(false);

        if (musicaFondo != null) musicaFondo.Play();
        StartCoroutine(MostrarImagen(0));
    }

    void Update()
    {
        bool inputDetectado = Input.GetMouseButtonDown(0)
                            || Input.GetKeyDown(KeyCode.Space)
                            || Input.GetKeyDown(KeyCode.Return);

        if (puedeAvanzar && inputDetectado)
        {
            Avanzar();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(TerminarCinematica());
        }
    }

    void Avanzar()
    {
        puedeAvanzar = false;
        if (indicadorContinuar != null) indicadorContinuar.SetActive(false);

        indiceActual++;
        if (indiceActual >= imagenes.Length)
        {
            StartCoroutine(TerminarCinematica());
        }
        else
        {
            StartCoroutine(MostrarImagen(indiceActual));
        }
    }

    IEnumerator MostrarImagen(int indice)
    {
        // Fade out de la imagen anterior (si no es la primera)
        if (indice > 0)
        {
            yield return Fade(1, 0, duracionFade);
        }

        // Cambiar imagen
        displayImage.sprite = imagenes[indice];

        // Reproducir voz principal de esta imagen (si existe)
        if (vozNarrador != null && clipsNarracion != null
            && indice < clipsNarracion.Length && clipsNarracion[indice] != null)
        {
            vozNarrador.Stop();
            vozNarrador.clip = clipsNarracion[indice];
            vozNarrador.Play();
        }

        // Fade in
        yield return Fade(0, 1, duracionFade);

        // Esperar un momento para que no se pase sin querer
        yield return new WaitForSeconds(esperaMinima);

        // Si hay un clip extra para esta imagen, esperar al principal y reproducirlo
        if (vozNarrador != null && clipsExtraNarracion != null
            && indice < clipsExtraNarracion.Length && clipsExtraNarracion[indice] != null)
        {
            while (vozNarrador.isPlaying)
            {
                yield return null;
            }
            vozNarrador.clip = clipsExtraNarracion[indice];
            vozNarrador.Play();
        }

        if (indicadorContinuar != null) indicadorContinuar.SetActive(true);
        puedeAvanzar = true;
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    IEnumerator TerminarCinematica()
    {
        puedeAvanzar = false;
        yield return Fade(canvasGroup.alpha, 0, duracionFade);

        if (musicaFondo != null)
        {
            float volInicial = musicaFondo.volume;
            float t = 0f;
            while (t < 1.5f)
            {
                t += Time.deltaTime;
                musicaFondo.volume = Mathf.Lerp(volInicial, 0, t / 1.5f);
                yield return null;
            }
            musicaFondo.Stop();
        }

        SceneManager.LoadScene(escenaSiguiente);
    }
}