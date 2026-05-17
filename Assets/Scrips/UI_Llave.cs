using UnityEngine;
using UnityEngine.UI;

public class UI_Llave : MonoBehaviour
{
    public GameObject panel;
    public Minijuego_2 mN;
    public Vida vid;
    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void Mostrar()
    {
        if (panel != null)
        {
            panel.SetActive(true);
            mN.DesactivarVisibilidad();
            vid.DesactivarVisibilidad();
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            // Busca el boton dentro del panel y le asigna Ocultar al click
            Button btn = panel.GetComponentInChildren<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(Ocultar);
            }
        }
    }

    public void Ocultar()
    {
        if (panel != null)
            panel.SetActive(false);
        mN.ActivarVisibilidad();
        vid.ActivarVisibilidad();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }
}