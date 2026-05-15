using UnityEngine;

public class Lago : MonoBehaviour
{
    public GameObject canvas;
    public Minijuego_2 mN;
    private bool jugadorDentro = false;
    float tiempoInicio;
    float duracion = 60f;

    void Start()
    {
        if (canvas != null)
            canvas.SetActive(false);
        tiempoInicio = Time.time;
    }

    void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            if (canvas != null)
            {
                canvas.SetActive(true);
                mN.DesactivarVisibilidad();
            }
        }

        if (jugadorDentro && Input.GetKeyDown(KeyCode.Z))
        {
            if (Time.time - tiempoInicio >= duracion)
            {
                mN.CargarRegadera();
                tiempoInicio = Time.time;
            }
            else
            {
                mN.intreaccionlago();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
            mN.interacciónBasica();

            if (Time.time > duracion && mN.regaderaConstruida)
            {
                mN.interacciónRegadera();
            }

            if (mN.objetivos[1].estado_Objetivo1 == false)
            {
                mN.objetivos[1].estado_Objetivo1 = true;
                mN.Objetivo_Cumplido();
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