
using UnityEngine;

public class CamaraCI1 : MonoBehaviour
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
    public float velocidadMovimiento = 2f;
    public float velocidadRotacion = 3f;
    public bool iniciarAutomatico = false;

    private int tomaActual = 0;
    private float timerToma = 0f;
    private bool activa = false;
    private Vector3 posicionOrigen;
    private Quaternion rotacionOrigen;

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
        posicionOrigen = transform.position;
        rotacionOrigen = transform.rotation;
    }

    void Update()
    {
        if (!activa) return;
        if (tomas.Length == 0) return;

        Toma toma = tomas[tomaActual];
        timerToma += Time.deltaTime;

        float progreso = Mathf.Clamp01(timerToma / toma.duracion);
        float progresoSuave = Mathf.SmoothStep(0f, 1f, progreso);

        if (toma.posicion != null)
        {
            transform.position = Vector3.Lerp(
                posicionOrigen,
                toma.posicion.position,
                progresoSuave
            );
        }

        if (toma.objetivo != null)
        {
            Vector3 direccion = toma.objetivo.position - transform.position;
            if (direccion != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    rotacionObjetivo,
                    Time.deltaTime * velocidadRotacion
                );
            }
        }
        else if (toma.posicion != null)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                toma.posicion.rotation,
                progresoSuave
            );
        }

        if (timerToma >= toma.duracion)
        {
            tomaActual++;
            timerToma = 0f;
            posicionOrigen = transform.position;
            rotacionOrigen = transform.rotation;

            if (tomaActual >= tomas.Length)
            {
                activa = false;
                Debug.Log("Cinematica terminada");
            }
        }
    }
}