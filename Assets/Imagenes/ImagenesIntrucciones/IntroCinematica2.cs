using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using static System.Net.Mime.MediaTypeNames;

public class IntroCinematica2 : MonoBehaviour
{
    [Header("Imágenes en orden")]
    public Sprite[] imagenes;
    public Image displayImage;

    [Header("Subtítulos")]
    public TextMeshProUGUI textoSubtitulo;
    [TextArea] public string[] subtitulos;

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
    private CanvasGroup subtituloCanvas;

    void Start()
    {
        canvasGroup = displayImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = displayImage.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0;

        // Setup subtitulo
        if (textoSubtitulo != null)
        {
            subtituloCanvas = textoSubtitulo.GetComponent<CanvasGroup>();
            if (subtituloCanvas == null)
                subtituloCanvas = textoSubtitulo.gameObject.AddComponent<CanvasGroup>();
            subtituloCanvas.alpha = 0;
            textoSubtitulo.text = "";
        }

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
            Avanzar();

        if (Input.GetKeyDown(KeyCode.Escape))
            StartCoroutine(TerminarCinematica());
    }

    void Avanzar()
    {
        puedeAvanzar = false;
        if (indicadorContinuar != null) indicadorContinuar.SetActive(false);

        indiceActual++;
        if (indiceActual >= imagenes.Length)
            StartCoroutine(TerminarCinematica());
        else
            StartCoroutine(MostrarImagen(indiceActual));
    }

    IEnumerator MostrarImagen(int indice)
    {
        // Fade out imagen y subtitulo
        if (indice > 0)
        {
            yield return Fade(canvasGroup, 1, 0, duracionFade);
            if (subtituloCanvas != null)
                yield return Fade(subtituloCanvas, 1, 0, duracionFade * 0.5f);
        }

        // Cambiar imagen
        displayImage.sprite = imagenes[indice];

        // Cambiar subtitulo
        if (textoSubtitulo != null)
        {
            textoSubtitulo.text = (subtitulos != null && indice < subtitulos.Length)
                ? subtitulos[indice]
                : "";
        }

        // Reproducir voz
        if (vozNarrador != null && clipsNarracion != null
            && indice < clipsNarracion.Length && clipsNarracion[indice] != null)
        {
            vozNarrador.Stop();
            vozNarrador.clip = clipsNarracion[indice];
            vozNarrador.Play();
        }

        // Fade in imagen
        yield return Fade(canvasGroup, 0, 1, duracionFade);

        // Fade in subtitulo
        if (subtituloCanvas != null && !string.IsNullOrEmpty(textoSubtitulo.text))
            yield return Fade(subtituloCanvas, 0, 1, duracionFade * 0.5f);

        yield return new WaitForSeconds(esperaMinima);

        // Clip extra
        if (vozNarrador != null && clipsExtraNarracion != null
            && indice < clipsExtraNarracion.Length && clipsExtraNarracion[indice] != null)
        {
            while (vozNarrador.isPlaying) yield return null;
            vozNarrador.clip = clipsExtraNarracion[indice];
            vozNarrador.Play();
        }

        if (indicadorContinuar != null) indicadorContinuar.SetActive(true);
        puedeAvanzar = true;
    }

    IEnumerator Fade(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }

    IEnumerator TerminarCinematica()
    {
        puedeAvanzar = false;

        if (subtituloCanvas != null)
            yield return Fade(subtituloCanvas, subtituloCanvas.alpha, 0, duracionFade * 0.5f);

        yield return Fade(canvasGroup, canvasGroup.alpha, 0, duracionFade);

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