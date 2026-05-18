using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance;

    private string baseUrl = "http://apiuneron.somee.com/api";

    public static int jugadorId = -1;
    public static int partidaId = -1;

    public static List<PartidaInfo> partidasGuardadas = new List<PartidaInfo>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ─── CREAR JUGADOR ───
    public void CrearJugador()
    {
        StartCoroutine(PostJugador());
    }

    IEnumerator PostJugador()
    {
        if (PlayerPrefs.HasKey("jugadorId"))
        {
            jugadorId = PlayerPrefs.GetInt("jugadorId");
            Debug.Log("[API] Jugador existente reutilizado. ID: " + jugadorId);
            CrearPartida();
            yield break;
        }

        string nombrePC = System.Environment.UserName;
        string json = "{\"nombre\":\"" + nombrePC + "\",\"email\":\"" + nombrePC + "@uneron.com\"}";

        using (UnityWebRequest req = new UnityWebRequest(baseUrl + "/Jugador", "POST"))
        {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                JugadorResponse res = JsonUtility.FromJson<JugadorResponse>(req.downloadHandler.text);
                jugadorId = res.id;
                PlayerPrefs.SetInt("jugadorId", jugadorId);
                PlayerPrefs.Save();
                Debug.Log("[API] Jugador creado. ID: " + jugadorId + " Nombre: " + nombrePC);
                CrearPartida();
            }
            else
            {
                Debug.LogWarning("[API] Error al crear jugador: " + req.error);
            }
        }
    }

    // ─── CREAR PARTIDA ───
    public void CrearPartida()
    {
        StartCoroutine(PostPartida());
    }

    IEnumerator PostPartida()
    {
        string nombrePC = System.Environment.UserName;
        string fecha = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        string nombrePartida = nombrePC + " - " + fecha;

        string json = "{\"nombre\":\"" + nombrePartida + "\",\"estado\":\"EnCurso\"}";

        using (UnityWebRequest req = new UnityWebRequest(baseUrl + "/Partida/guardar", "POST"))
        {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                PartidaResponse res = JsonUtility.FromJson<PartidaResponse>(req.downloadHandler.text);
                partidaId = res.partidaId;
                Debug.Log("[API] Partida creada. ID: " + partidaId + " Nombre: " + nombrePartida);
                UnityEngine.SceneManagement.SceneManager.LoadScene("Inicio");
            }
            else
            {
                Debug.LogWarning("[API] Error al crear partida: " + req.error);
            }
        }
    }

    // ─── OBTENER PARTIDAS ───
    public void ObtenerPartidas(System.Action<List<PartidaInfo>> callback)
    {
        StartCoroutine(GetPartidas(callback));
    }

    IEnumerator GetPartidas(System.Action<List<PartidaInfo>> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(baseUrl + "/Partida/todas"))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                PartidaListResponse res = JsonUtility.FromJson<PartidaListResponse>(req.downloadHandler.text);
                partidasGuardadas = res.partidas;
                Debug.Log("[API] Partidas obtenidas: " + partidasGuardadas.Count);
                callback?.Invoke(partidasGuardadas);
            }
            else
            {
                Debug.LogWarning("[API] Error al obtener partidas: " + req.error);
                callback?.Invoke(new List<PartidaInfo>());
            }
        }
    }

    // ─── GUARDAR PROGRESO ───
    public void GuardarProgreso(float vida, float puntos, string escena, Vector3 posicion, string inventario, string objetivos)
    {
        if (partidaId == -1)
        {
            Debug.LogWarning("[API] No hay partida activa.");
            return;
        }
        StartCoroutine(PostProgreso(vida, puntos, escena, posicion, inventario, objetivos));
    }

    IEnumerator PostProgreso(float vida, float puntos, string escena, Vector3 posicion, string inventario, string objetivos)
    {
        // Escapar comillas del JSON interno para que sea JSON válido dentro de un string
        string inventarioEscapado = inventario.Replace("\"", "\\\"");
        string objetivosEscapado = objetivos.Replace("\"", "\\\"");

        // CORRECCIÓN: vida y puntos se convierten a int porque el modelo C# los espera como enteros.
        // Si se envían como decimales (50.00) el servidor falla al deserializar y guarda ceros.
        string json = "{" +
            "\"partidaId\":" + partidaId + "," +
            "\"nivel\":1," +
            "\"puntos\":" + ((int)puntos) + "," +
            "\"posicionX\":" + posicion.x.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "," +
            "\"posicionY\":" + posicion.y.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "," +
            "\"posicionZ\":" + posicion.z.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "," +
            "\"nombreEscena\":\"" + escena + "\"," +
            "\"vida\":" + ((int)vida) + "," +
            "\"inventarioJson\":\"" + inventarioEscapado + "\"," +
            "\"objetivosJson\":\"" + objetivosEscapado + "\"" +
            "}";

        using (UnityWebRequest req = new UnityWebRequest(baseUrl + "/Progreso", "POST"))
        {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                Debug.Log("[API] Progreso guardado. Escena: " + escena + " Vida: " + (int)vida + " Puntos: " + (int)puntos + " Pos: " + posicion);
            else
                Debug.LogWarning("[API] Error al guardar progreso: " + req.error + " Body: " + req.downloadHandler.text);
        }
    }

    // ─── CLASES ───
    [System.Serializable]
    public class JugadorResponse { public int id; }

    [System.Serializable]
    public class PartidaResponse { public int partidaId; }

    [System.Serializable]
    public class PartidaInfo
    {
        public int id;
        public string nombre;
        public string estado;
        public string fechaInicio;
    }

    [System.Serializable]
    private class PartidaListResponse
    {
        public int total;
        public List<PartidaInfo> partidas;
    }
}