
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagerGame : MonoBehaviour
{
    public string sceneName = "Juego";

    public GameObject menuPanel;
    public GameObject optionsPanel;

    // ?? INICIAR JUEGO
    public void StartGame()
    {
        Debug.Log("Boton Play Presionado ...");

        // Oculta y bloquea el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(sceneName);
    }

    // ?? OPCIONES
    public void OpenOptions()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    // ? SALIR
    public void ExitGame()
    {
        Debug.Log("Juego cerrado");
        Application.Quit();
    }

    // ??? MENÚ (cursor visible)
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}