
using UnityEngine;

public class CinematicaBoss : MonoBehaviour
{
    [System.Serializable]
    public class Toma
    {
        public Transform posicion;
        public Transform objetivo;
        public float duracion = 3f;
    }

    [Header("Tomas")]
    public Toma[] tomas;

    [Header("Configuracion")]
    public float velocidadRotacion = 3f;
    public bool iniciarAutomatico = false;

    private int tomaActual = 0;
    private float timerToma = 0f;
    private bool activa = false;
    private Vector3 posicionOrigen;

    void Start()
    {
        if (iniciarAutomatico)
            IniciarCinematica();
    }

    public void IniciarCinematica()
    {
        if (tomas.Length == 0) return;

        activa = true;
        tomaActual = 0;
        timerToma = 0f;

        // Teletransportar la camara al primer Empty al inicio
        if (tomas[0].posicion != null)
        {
            transform.position = tomas[0].posicion.position;
            transform.rotation = tomas[0].posicion.rotation;
        }

        posicionOrigen = transform.position;
    }

    void Update()
    {
        if (!activa) return;
        if (tomas.Length == 0) return;

        Toma toma = tomas[tomaActual];
        timerToma += Time.deltaTime;

        float progreso = Mathf.Clamp01(timerToma / toma.duracion);
        float suave = Mathf.SmoothStep(0f, 1f, progreso);

        if (toma.posicion != null)
            transform.position = Vector3.Lerp(posicionOrigen, toma.posicion.position, suave);

        if (toma.objetivo != null)
        {
            Vector3 dir = toma.objetivo.position - transform.position;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(dir),
                    Time.deltaTime * velocidadRotacion
                );
        }

        if (timerToma >= toma.duracion)
        {
            tomaActual++;
            timerToma = 0f;
            posicionOrigen = transform.position;

            if (tomaActual >= tomas.Length)
            {
                activa = false;
                Debug.Log("Cinematica terminada");
            }
        }
    }
}