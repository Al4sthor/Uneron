
using UnityEngine;

public class Puerta : MonoBehaviour
{

    public Minijuego_2 mN;
    bool jugadorDentro;
    bool activada;
    [Header("Configuración de la llave")]
    [Tooltip("Tipo de llave que abre esta puerta. Debe coincidir con el del cubo.")]
    public string tipoLlaveRequerida = "LlaveDorada";
    [Tooltip("Si está marcado, la llave se gasta al abrir. Si no, sirve para más puertas.")]
    public bool consumirLlave = false;

    [Header("Detección del jugador")]
    [Tooltip("Distancia a la que el jugador puede interactuar con la puerta.")]
    public float distanciaInteraccion = 3f;
    [Tooltip("Tecla para abrir la puerta.")]
    public KeyCode teclaAbrir = KeyCode.E;
    public Transform player;

    [Header("Modo de apertura")]
    [Tooltip("Cómo se abre la puerta cuando se desbloquea.")]
    public ModoApertura modoApertura = ModoApertura.Rotar;
    public float velocidadApertura = 2f;

    [Header("Mensajes opcionales en consola")]
    public bool mostrarMensajes = true;

    public enum ModoApertura { Destruir, Rotar, DeslizarArriba }

    private bool abierta = false;
    private bool abriendo = false;
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    void Start()
    {
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;

        // Si no asignaste el player, intenta encontrarlo por tag
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        // Si ya está abierta o abriendo, animar y salir
        if (abriendo)
        {
            AnimarApertura();
            return;
        }
        if (abierta) return;

        // Verificar distancia al jugador
        if (player == null) return;
        float distancia = Vector3.Distance(transform.position, player.position);

        if (distancia <= distanciaInteraccion && Input.GetKeyDown(teclaAbrir))
        {
            IntentarAbrir();
        }
    }

    void IntentarAbrir()
    {
        if (Inventario.TieneLlave(tipoLlaveRequerida))
        {
            if (consumirLlave)
            {
                Inventario.UsarLlave(tipoLlaveRequerida);
            }
            Abrir();
        }
        else
        {
            if (mostrarMensajes)
            {
                Debug.Log("Necesitas una " + tipoLlaveRequerida + " para abrir esta puerta.");
            }
        }
    }

    void Abrir()
    {
        if (mostrarMensajes) Debug.Log("Puerta abierta: " + gameObject.name);

        if (modoApertura == ModoApertura.Destruir)
        {
            Destroy(gameObject);
        }
        else
        {
            abriendo = true;
        }
        activada = true;
    }

    void AnimarApertura()
    {
        if (modoApertura == ModoApertura.Rotar)
        {
            // Rota 90 grados sobre el eje Y
            Quaternion objetivo = rotacionInicial * Quaternion.Euler(0, 90, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, objetivo, Time.deltaTime * velocidadApertura);

            if (Quaternion.Angle(transform.rotation, objetivo) < 0.5f)
            {
                transform.rotation = objetivo;
                abierta = true;
                abriendo = false;
                // Desactivar collider para que el jugador pase
                Collider col = GetComponent<Collider>();
                if (col != null) col.enabled = false;
            }
        }
        else if (modoApertura == ModoApertura.DeslizarArriba)
        {
            Vector3 objetivo = posicionInicial + Vector3.up * 3f;
            transform.position = Vector3.Lerp(transform.position, objetivo, Time.deltaTime * velocidadApertura);

            if (Vector3.Distance(transform.position, objetivo) < 0.05f)
            {
                transform.position = objetivo;
                abierta = true;
                abriendo = false;
                Collider col = GetComponent<Collider>();
                if (col != null) col.enabled = false;
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
            if (!activada)
            {
                mN.interacciónBasica();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
            mN.DesactivarInteraccion();

        }
    }
}