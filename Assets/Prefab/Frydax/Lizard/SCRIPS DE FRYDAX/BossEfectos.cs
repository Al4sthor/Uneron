using UnityEngine;

public class BossEfectos : MonoBehaviour
{
    public Renderer modeloBoss;

    public Color colorFase1 = Color.green;
    public Color colorFase2 = new Color(1f, 0.5f, 0f);
    public Color colorFase3 = Color.red;

    public AudioSource audioSource;
    public AudioClip sonidoFase2;
    public AudioClip sonidoFase3;

    void Start()
    {
        if (modeloBoss != null)
            CambiarColor(colorFase1);
    }

    public void CambiarAFase2()
    {
        CambiarColor(colorFase2);
    }

    public void CambiarAFase3()
    {
        CambiarColor(colorFase3);
    }

    void CambiarColor(Color color)
    {
        if (modeloBoss == null) return;

        // URP usa _BaseColor en lugar de color directo
        Material mat = modeloBoss.material;
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);
        else
            mat.color = color;
    }

    public void SonidoFase2()
    {
        if (audioSource != null && sonidoFase2 != null)
            audioSource.PlayOneShot(sonidoFase2);
    }

    public void SonidoFase3()
    {
        if (audioSource != null && sonidoFase3 != null)
            audioSource.PlayOneShot(sonidoFase3);
    }
}