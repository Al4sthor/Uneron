using UnityEngine;

// Pon este script en la CAMARA, no en el player.
// La camara NO debe ser hija del player en la jerarquia.
// Arrastra el Player al campo "jugador" desde el Inspector.

public class CamaraJugador : MonoBehaviour
{
    [Header("Referencia")]
    public Transform jugador;

    [Header("Posicion")]
    public float distancia = 6f;        // Distancia atras del jugador
    public float alturaOffset = 2.2f;   // Altura desde los pies del jugador
    public float offsetLateral = 0f;    // 0 = centrado, positivo = derecha

    [Header("Rotacion")]
    public float sensibilidad = 2f;
    public float limiteVerticalMin = -15f;
    public float limiteVerticalMax = 70f;

    [Header("Colisiones")]
    public float radioColision = 0.3f;
    public LayerMask capaColision;      // Pon aqui tus layers de paredes y suelo

    // Suavizado de posicion cuando la camara rebota por colision
    private float distanciaActual;
    private float velocidadDistancia;
    public float suavizadoColision = 8f;

    private float rotacionX = 0f;
    private Player1 playerScript;

    // Punto al que mira la camara (cabeza del jugador aprox)
    private Vector3 puntoMira => jugador.position + Vector3.up * 1.5f;

    void Start()
    {
        if (jugador != null)
            playerScript = jugador.GetComponent<Player1>();

        distanciaActual = distancia;
    }

    void LateUpdate()
    {
        if (jugador == null || playerScript == null) return;

        // --- Rotacion vertical ---
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidad;
        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, limiteVerticalMin, limiteVerticalMax);

        // --- Direccion de la camara en base a rotacion del jugador + mouse ---
        Quaternion rotacion = Quaternion.Euler(rotacionX, playerScript.targetRotationY, 0f);

        // Pivot: desde donde se mide la distancia (altura media del jugador)
        Vector3 pivot = jugador.position + Vector3.up * alturaOffset;

        // Offset lateral para que el jugador no tape el centro de pantalla
        Vector3 offsetLocal = new Vector3(offsetLateral, 0f, -distancia);
        Vector3 posicionDeseada = pivot + rotacion * offsetLocal;

        // --- Colision: SphereCast desde el pivot hasta la posicion deseada ---
        // Esto evita que atraviese paredes Y que atraviese al jugador mismo
        Vector3 direccion = posicionDeseada - pivot;
        float distanciaDeseada = direccion.magnitude;
        float distanciaSegura = distanciaDeseada;

        if (Physics.SphereCast(
                pivot,
                radioColision,
                direccion.normalized,
                out RaycastHit hit,
                distanciaDeseada,
                capaColision))
        {
            // La camara se queda justo antes del obstaculo
            distanciaSegura = hit.distance - radioColision;
            distanciaSegura = Mathf.Max(distanciaSegura, 0.5f); // minimo para no meterse al jugador
        }

        // Suavizado: si hay colision se acerca rapido, si se libera vuelve suave
        float velocidadSuavizado = (distanciaSegura < distanciaActual)
            ? suavizadoColision * 3f  // acercarse rapido al chocar
            : suavizadoColision;      // alejarse suave al liberar

        distanciaActual = Mathf.SmoothDamp(
            distanciaActual,
            distanciaSegura,
            ref velocidadDistancia,
            1f / velocidadSuavizado
        );

        // Posicion final de la camara
        transform.position = pivot + direccion.normalized * distanciaActual;

        // La camara siempre mira hacia la cabeza del jugador
        transform.LookAt(puntoMira);
    }
}