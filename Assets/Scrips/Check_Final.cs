using UnityEngine;

public class Check_Final : MonoBehaviour
{
    public Minijuego_2 mN;
    private bool jugadorDentro = false;
    private bool ParteConsegida = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
            if (!ParteConsegida)
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