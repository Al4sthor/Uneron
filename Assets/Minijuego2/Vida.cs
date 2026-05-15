using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
public class Vida : MonoBehaviour
{
    UIDocument vida;
    public Player1 player;

    ProgressBar barraVida;


    private void OnEnable()
    {
        vida = GetComponent<UIDocument>();
        VisualElement root = vida.rootVisualElement;
        barraVida = root.Q<ProgressBar>("Vida");

    }

    public void ActualizarVida()
    {
        barraVida.value = player.currentHealth;
    }

    public void ActivarVisibilidad()
    {
        vida.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    public void DesactivarVisibilidad()
    {
        vida.rootVisualElement.style.display = DisplayStyle.None;
    }
}
