
using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

public class GameDataSender : MonoBehaviour
{
    [Header("Configuración del Servidor")]
    [Tooltip("URL de tu API publicada en Somee")]
    public string apiUrl = "http://tudominio.somee.com/api/partida/guardar";

    [Header("Datos del Jugador")]
    public int nivelActual = 1;
    public int puntosActuales = 0;

    [Header("UI Feedback")]
    public UnityEngine.UI.Text feedbackText; // Opcional: para mostrar mensaje  en pantalla

    private void Start()
    {
        Debug.Log(" Sistema de envío de datos iniciado");
        Debug.Log($" URL del API: {apiUrl}");
    }

    // Método que se llama desde el triger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Debug.Log(" TRIGGER ACTIVADO - Enviando datos...");
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            EnviarDatosCompletos();
        }
    }

    // Método público que puedes llamar desde un botko
    public void EnviarDatosCompletos()
    {
        StartCoroutine(EnviarDatosAlServidor());
    }

    IEnumerator EnviarDatosAlServidor()
    {
        // Obtener posición del jugador
        Transform jugador = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 posicion = jugador.position;

        // Crear inventario de ejemplo (JEISON)
        string inventario = CrearInventarioJSON();

        // Crear objeto de datos completo
        GameData datos = new GameData
        {
            Estado = "Checkpoint alcanzado",
            Nivel = nivelActual,
            Puntos = puntosActuales,
            PosicionX = posicion.x,
            PosicionY = posicion.z, // Usamos Z como Y para 3D
            InventarioJson = inventario
        };

        // Convertir a JSON
        string jsonData = JsonUtility.ToJson(datos);

        Debug.Log(" Datos a enviar:");
        Debug.Log(jsonData);

        // Configurar petición POST
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Mostrar "Enviando..." en UI
        if (feedbackText != null)
            feedbackText.text = "📡 Enviando datos al servidor...";

        // Enviar petición
        float startTime = Time.time;
        yield return request.SendWebRequest();
        float elapsedTime = Time.time - startTime;

        // Procesar respuesta
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Debug.Log(" ¡DATOS ENVIADOS EXITOSAMENTE!");
            Debug.Log($" Tiempo de respuesta: {elapsedTime:F2} segundos");
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Debug.Log(" Respuesta del servidor:");
            Debug.Log(request.downloadHandler.text);
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            if (feedbackText != null)
                feedbackText.text = " Datos guardados en la nube";

            // Parsear respuesta
            try
            {
                ServerResponse response = JsonUtility.FromJson<ServerResponse>(request.downloadHandler.text);
                Debug.Log($" Partida ID: {response.partida.id}");
                Debug.Log($" Progreso ID: {response.progreso.id}");
            }
            catch (Exception e)
            {
                Debug.Log(" No se pudo parsear la respuesta: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Debug.LogError(" ERROR AL ENVIAR DATOS");
            Debug.LogError($"Código de error: {request.responseCode}");
            Debug.LogError($"Mensaje: {request.error}");
            Debug.LogError($"Detalles: {request.downloadHandler.text}");
            Debug.LogError("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            if (feedbackText != null)
                feedbackText.text = "❌ Error al enviar datos";
        }
    }

    // Crear un inventario de ejemplo en formato JSON
    private string CrearInventarioJSON()
    {
        // Aqui nos toca Simularc items del inventario
        string[] items = { "Espada", "Escudo", "Poción x3", "Llave dorada" };
        return "{\"items\":[\"" + string.Join("\",\"", items) + "\"]}";
    }

    // Método para probar conexión
    public void ProbarConexion()
    {
        StartCoroutine(TestConexion());
    }

    IEnumerator TestConexion()
    {
        string testUrl = apiUrl.Replace("/guardar", "/test");

        UnityWebRequest request = UnityWebRequest.PostWwwForm(testUrl, "");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(" Conexión con servidor exitosa!");
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("No se pudo conectar con el servidor: " + request.error);
        }
    }
}

// Clases para serializar datos
[Serializable]
public class GameData
{
    public string Estado;
    public int Nivel;
    public int Puntos;
    public float PosicionX;
    public float PosicionY;
    public string InventarioJson;
}

[Serializable]
public class ServerResponse
{
    public bool success;
    public string mensaje;
    public PartidaData partida;
    public ProgresoData progreso;
}

[Serializable]
public class PartidaData
{
    public int id;
    public string estado;
    public string fechaInicio;
}

[Serializable]
public class ProgresoData
{
    public int id;
    public int nivel;
    public int puntos;
    public string posicion;
}