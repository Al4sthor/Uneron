using System.Collections;
using UnityEngine;
using TMPro;

public class PantallaCarga : MonoBehaviour
{
    public GameObject interfas;
    public GameObject vida;

    [Header("Referencias")]
    public GameObject canvasCarga;
    public TMP_Text textoCarga;

    [Header("Configuración")]
    public float duracion = 3f;
    public bool requiereClick = false;
    public string textoBase = "Cargando mundo";
    public bool animarPuntos = true;

    void Start()
    {
        // Ocultar interfaz y vida al inicio
        if (interfas != null) interfas.SetActive(false);
        if (vida != null) vida.SetActive(false);

        // Mostrar pantalla de carga
        Time.timeScale = 0f;
        if (canvasCarga != null) canvasCarga.SetActive(true);

        StartCoroutine(MostrarPantalla());
        if (animarPuntos && textoCarga != null) StartCoroutine(AnimarPuntos());
    }

    IEnumerator MostrarPantalla()
    {
        if (requiereClick)
        {
            while (!(Input.GetMouseButtonDown(0)
                  || Input.GetKeyDown(KeyCode.Space)
                  || Input.GetKeyDown(KeyCode.Return)))
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSecondsRealtime(duracion);
        }

        // Esto se ejecuta en AMBOS casos al terminar
        if (canvasCarga != null) canvasCarga.SetActive(false);

        if (interfas != null) interfas.SetActive(true);
        if (vida != null) vida.SetActive(true);

        Time.timeScale = 1f;
    }

    IEnumerator AnimarPuntos()
    {
        int puntos = 0;
        while (canvasCarga != null && canvasCarga.activeSelf)
        {
            puntos = (puntos + 1) % 4;
            textoCarga.text = textoBase + new string('.', puntos);
            yield return new WaitForSecondsRealtime(0.4f);
        }
    }
}