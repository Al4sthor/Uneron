
using UnityEngine;

public class PuertaBunker : MonoBehaviour
{
    [Header("Configuracion de la llave")]
    [Tooltip("Debe coincidir exactamente con el nombre de la llave")]
    public string tipoLlaveRequerida = "LlaveBunker";
    [Tooltip("Si esta marcado la llave se gasta al abrir")]
    public bool consumirLlave = false;

    [Header("Deteccion del jugador")]
    public float distanciaInteraccion = 3f;
    public KeyCode teclaAbrir = KeyCode.E;
    public Transform player;

    [Header("Animacion")]
    [Tooltip("Velocidad a la que se desliza la puerta")]
    public float velocidadApertura = 2f;
    [Tooltip("Cuantas unidades se desliza hacia la derecha")]
    public float distanciaDesplazamiento = 8f;

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoApertura;

    [Header("Testing")]
    [Tooltip("Marca esto para poder abrir con E sin necesitar la llave")]
    public bool modoTest = false;

    [Header("Mensajes")]
    public bool mostrarMensajes = true;

    private bool abierta = false;
    private bool abriendo = false;
    private Vector3 posicionInicial;
    private Vector3 posicionObjetivo;

    void Start()
    {
        posicionInicial = transform.position;
        posicionObjetivo = transform.position + transform.right * distanciaDesplazamiento;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (modoTest && Input.GetKeyDown(KeyCode.E))
        {
            Inventario.AgregarLlave(tipoLlaveRequerida);
            Abrir();
            Debug.Log("TEST: Llave agregada y puerta abierta");
        }

        if (abriendo)
        {
            AnimarApertura();
            return;
        }

        if (abierta) return;
        if (player == null) return;

        float distancia = Vector3.Distance(transform.position, player.position);

        if (!modoTest && distancia <= distanciaInteraccion && Input.GetKeyDown(teclaAbrir))
            IntentarAbrir();
    }

    void IntentarAbrir()
    {
        if (Inventario.TieneLlave(tipoLlaveRequerida))
        {
            if (consumirLlave)
                Inventario.UsarLlave(tipoLlaveRequerida);
            Abrir();
        }
        else
        {
            if (mostrarMensajes)
                Debug.Log("Necesitas la " + tipoLlaveRequerida + " para abrir este bunker");
        }
    }

    void Abrir()
    {
        abriendo = true;

        if (audioSource != null && sonidoApertura != null)
            audioSource.PlayOneShot(sonidoApertura);

        if (mostrarMensajes)
            Debug.Log("Bunker abierto");
    }

    void AnimarApertura()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            posicionObjetivo,
            Time.deltaTime * velocidadApertura
        );

        if (Vector3.Distance(transform.position, posicionObjetivo) < 0.01f)
        {
            transform.position = posicionObjetivo;
            abierta = true;
            abriendo = false;

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }
}