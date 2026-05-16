using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BotonAnimado : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform rect;
    private Image img;

    private Vector3 escalaNormal;
    private Vector3 escalaHover;
    private Vector3 escalaClick;
    private Vector3 escalaObjetivo;

    public Color colorNormal = Color.white;
    public Color colorHover = Color.yellow;
    public Color colorClick = Color.gray;

    public float velocidad = 10f; // velocidad de transición

    void Start()
    {
        rect = GetComponent<RectTransform>();
        img = GetComponent<Image>();

        escalaNormal = rect.localScale;
        escalaHover = escalaNormal * 1.1f; // zoom al pasar el mouse
        escalaClick = escalaNormal * 0.9f; // achicar al hacer click

        escalaObjetivo = escalaNormal;
        img.color = colorNormal;
    }

    void Update()
    {
        // transición suave de escala y color
        rect.localScale = Vector3.Lerp(rect.localScale, escalaObjetivo, Time.deltaTime * velocidad);
        img.color = Color.Lerp(img.color, colorObjetivo, Time.deltaTime * velocidad);
    }

    private Color colorObjetivo;

    public void OnPointerEnter(PointerEventData eventData)
    {
        escalaObjetivo = escalaHover;
        colorObjetivo = colorHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        escalaObjetivo = escalaNormal;
        colorObjetivo = colorNormal;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        escalaObjetivo = escalaClick;
        colorObjetivo = colorClick;

        // después de un click, vuelve al hover si el mouse sigue encima
        Invoke(nameof(RestaurarHover), 0.15f);
    }

    private void RestaurarHover()
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition))
        {
            escalaObjetivo = escalaHover;
            colorObjetivo = colorHover;
        }
        else
        {
            escalaObjetivo = escalaNormal;
            colorObjetivo = colorNormal;
        }
    }
}
