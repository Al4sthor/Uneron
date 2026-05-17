using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;
public class MenuInicio : MonoBehaviour
{
    UIDocument menu;

    Button nueva_Partida;
    Button continuar_Partida;
    Button opciones;
    Button cerrar;
    Button cerrar_Panel;

    VisualElement panel_Partidas;
    ScrollView lista_Partidas;

    private void OnEnable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        menu = GetComponent<UIDocument>();
        VisualElement root = menu.rootVisualElement;

        // Botones principales
        nueva_Partida = root.Q<Button>("Nueva_Partida");
        continuar_Partida = root.Q<Button>("Continuar_Partida");
        opciones = root.Q<Button>("Opciones");
        cerrar = root.Q<Button>("Cerrar");

        // Panel de partidas
        panel_Partidas = root.Q<VisualElement>("Panel_Partidas");
        lista_Partidas = root.Q<ScrollView>("Lista_Partidas");
        cerrar_Panel = root.Q<Button>("Cerrar_Panel");

        // Asegurar que el panel empiece oculto
        panel_Partidas.style.display = DisplayStyle.None;

        // Eventos
        nueva_Partida.clicked += OnNuevaPartida;
        continuar_Partida.clicked += OnContinuarPartida;
        cerrar.clicked += () => Application.Quit();
        cerrar_Panel.clicked += () => panel_Partidas.style.display = DisplayStyle.None;
    }

    // ─── NUEVA PARTIDA ───
    void OnNuevaPartida()
    {
        Debug.Log("[MENU] Click en Nueva Partida. Conectando a la API...");

        // Verifica si ya existe el ApiManager, si no lo crea
        if (ApiManager.Instance == null)
        {
            GameObject obj = new GameObject("ApiManager");
            obj.AddComponent<ApiManager>();
        }

        ApiManager.Instance.CrearJugador();
        // La escena la carga el ApiManager cuando termina de crear la partida
    }

    // ─── CONTINUAR PARTIDA ───
    void OnContinuarPartida()
    {
        Debug.Log("[MENU] Click en Continuar Partida. Cargando partidas...");

        if (ApiManager.Instance == null)
        {
            GameObject obj = new GameObject("ApiManager");
            obj.AddComponent<ApiManager>();
        }

        // Muestra el panel y carga las partidas
        panel_Partidas.style.display = DisplayStyle.Flex;
        lista_Partidas.Clear();

        // Mensaje de carga
        Label cargando = new Label("Cargando partidas...");
        cargando.style.color = new StyleColor(Color.white);
        cargando.style.fontSize = 16;
        cargando.style.unityTextAlign = TextAnchor.MiddleCenter;
        lista_Partidas.Add(cargando);

        // Llama a la API
        ApiManager.Instance.ObtenerPartidas(partidas =>
        {
            lista_Partidas.Clear();

            if (partidas == null || partidas.Count == 0)
            {
                Label vacio = new Label("No hay partidas guardadas.");
                vacio.style.color = new StyleColor(Color.white);
                vacio.style.fontSize = 16;
                vacio.style.unityTextAlign = TextAnchor.MiddleCenter;
                lista_Partidas.Add(vacio);
                Debug.Log("[MENU] No se encontraron partidas.");
                return;
            }

            Debug.Log("[MENU] Partidas encontradas: " + partidas.Count);

            foreach (var partida in partidas)
            {
                // Contenedor de cada partida
                VisualElement fila = new VisualElement();
                fila.style.flexDirection = FlexDirection.Row;
                fila.style.justifyContent = Justify.SpaceBetween;
                fila.style.alignItems = Align.Center;
                fila.style.marginBottom = 8;
                fila.style.paddingLeft = 10;
                fila.style.paddingRight = 10;
                fila.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0.5f));

                // Nombre de la partida
                Label nombre = new Label(partida.nombre);
                nombre.style.color = new StyleColor(Color.white);
                nombre.style.fontSize = 14;

                // Botón cargar
                Button btnCargar = new Button();
                btnCargar.text = "Cargar";
                btnCargar.style.backgroundColor = new StyleColor(new Color(0.2f, 0.6f, 0.2f));
                btnCargar.style.color = new StyleColor(Color.white);

                int idPartida = partida.id;
                string nombrePartida = partida.nombre;

                btnCargar.clicked += () =>
                {
                    Debug.Log("[MENU] Cargando partida: " + nombrePartida + " ID: " + idPartida);
                    ApiManager.partidaId = idPartida;
                    panel_Partidas.style.display = DisplayStyle.None;
                    SceneManager.LoadScene("Inicio");
                };

                fila.Add(nombre);
                fila.Add(btnCargar);
                lista_Partidas.Add(fila);
            }
        });
    }
}