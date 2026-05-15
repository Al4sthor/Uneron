using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class MenuInicio : MonoBehaviour
{
    UIDocument menu;
    


    Button nueva_Partida;
    Button continuar_Partida;
    Button opciones;
    Button cerrar;



    private void OnEnable()
    {


        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        menu = GetComponent<UIDocument>();

        VisualElement root = menu.rootVisualElement;
        nueva_Partida = root.Q<Button>("Nueva_Partida");
        continuar_Partida = root.Q<Button>("Continuar_Partida");
        opciones = root.Q<Button>("Opciones");
        cerrar = root.Q<Button>("Cerrar");


        nueva_Partida.clicked += () => iniciarPartida();
        cerrar.clicked += () => salir();
        


    }

    void iniciarPartida()
    {
        Debug.Log("Click en Nueva Partida");
        SceneManager.LoadScene(1);
    }

    void salir()
    {
        Application.Quit();
    }


}
