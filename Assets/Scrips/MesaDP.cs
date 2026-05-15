using UnityEngine;

public class MesaDP : MonoBehaviour
{
    public GameObject Mesa;
    public Minijuego_2 mN;
    public Vida vid;
    public bool partesConseguidas;
    bool jugadorDentro;
    public bool completo;
    string mensaje;
    void Update()
    {
        if (partesConseguidas&&!completo)
        {
            if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
            {
                if (Mesa != null)
                {
                    Mesa.SetActive(true);
                    mN.DesactivarVisibilidad();
                    vid.DesactivarVisibilidad();
                }
                     
                if (mN.objetivos[2].estado_Objetivo1 == false)
                { mN.objetivos[2].estado_Objetivo1 = true;
                  mN.Objetivo_Cumplido();
                }
               
            }
        }
        else
        {
            if (completo)
            {
                if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
                {
                    mensaje = "Ya Arreglaste la regadera, ve a regargarla";
                    mN.MostrarMensaje(mensaje);
                }
            }
            else
            {
                if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
                {
                    mensaje = "Te faltan partes por encontrar";
                    mN.MostrarMensaje(mensaje);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
            mN.ActivarInteraccion();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Mesa.SetActive(false);
            jugadorDentro = false;
            mN.DesactivarInteraccion();
            mN.ActivarVisibilidad();
            vid.ActivarVisibilidad();
        }
    }

}
