using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverPro : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // guardo la escala normal del boton apenas inicia
    Vector3 escalaOriginal;

    // esta es la escala a la que quiero que crezca cuando paso el mouse
    Vector3 escalaHover;

    // velocidad de la animacion (si lo subo se mueve mas rapido)
    public float velocidad = 8f;

    // esta variable es como un interruptor pa saber si el mouse esta encima
    bool estaEncima = false;

    // aqui voy a poner el audio del click del boton
    public AudioSource audioSource;

    void Start()
    {
        // apenas empieza el juego guardo el tamaño original
        escalaOriginal = transform.localScale;

        // aqui defino cuanto quiero que crezca el boton (modo pro elegante)
        escalaHover = escalaOriginal * 1.15f;
    }

    void Update()
    {
        // esto se ejecuta cada frame entonces aqui hago la animacion suave
        if (estaEncima)
        {
            // cuando paso el mouse el boton crece suavecito
            transform.localScale = Vector3.Lerp(transform.localScale, escalaHover, Time.deltaTime * velocidad);
        }
        else
        {
            // cuando quito el mouse vuelve normal tambien suave
            transform.localScale = Vector3.Lerp(transform.localScale, escalaOriginal, Time.deltaTime * velocidad);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // activo el modo hover osea que el mouse ya esta encima
        estaEncima = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // aqui ya el mouse se fue entonces toca volver al tamaño normal
        estaEncima = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // aqui hago sonar el audio cuando le doy click al boton
        audioSource.Play();
    }
}