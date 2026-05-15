using UnityEngine;
using UnityEngine.SceneManagement;

public class PausaManager : MonoBehaviour
{
    public Minijuego_2 mN;
    public Vida vid;
    [Header("Referencias UI")]
    [SerializeField] private GameObject panelBotonPrincipal; // panel que contiene el BotonPausa
    [SerializeField] private GameObject panelMenu;           // menú desplegable (PanelPausa)
    [SerializeField] private GameObject panelOpciones;
    [SerializeField] private MonoBehaviour playerController; // script de control del jugador

    [Header("Estado del menú")]
    public bool menuVisible = false;
    public bool EnOpciones { get; private set; } = false;

    [Header("Nombre de la escena del menú principal")]
    [SerializeField] private string escenaMenuPrincipal = "Menu";

    private void Start()
    {
        if (panelBotonPrincipal != null) panelBotonPrincipal.SetActive(true);
        if (panelMenu != null) panelMenu.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuVisible) OcultarMenuPausa();
            else MostrarMenuPausa();
        }
    }

    public void MostrarMenuPausa()
    {
        if (panelMenu != null) panelMenu.SetActive(true);
        if (panelBotonPrincipal != null) panelBotonPrincipal.SetActive(false);

        menuVisible = true;
        EnOpciones = false;

        // 🔹 Pausar juego
        Time.timeScale = 0f;
        if (playerController != null) playerController.enabled = false;

        mN.DesactivarVisibilidad();
        vid.DesactivarVisibilidad()
;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OcultarMenuPausa()
    {
        if (panelMenu != null) panelMenu.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (panelBotonPrincipal != null) panelBotonPrincipal.SetActive(true);

        menuVisible = false;
        EnOpciones = false;

        // 🔹 Reanudar juego
        Time.timeScale = 1f;
        if (playerController != null) playerController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        mN.ActivarVisibilidad();
        vid.ActivarVisibilidad();
    }

    public void AbrirOpciones()
    {
        if (panelOpciones != null) panelOpciones.SetActive(true);
        if (panelMenu != null) panelMenu.SetActive(false);
        EnOpciones = true;
    }

    public void CerrarOpciones()
    {
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (panelMenu != null) panelMenu.SetActive(true);
        EnOpciones = false;
    }

    public void SalirJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaMenuPrincipal);
    }
}
