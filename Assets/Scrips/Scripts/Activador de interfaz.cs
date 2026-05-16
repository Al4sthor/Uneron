using UnityEngine;

public class Activadordeinterfaz : MonoBehaviour
{
    public Minijuego_2 mN;
    public Vida vid;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        { 
            if (mN.interfazActiva)
            {
                mN.DesactivarVisibilidad();
                vid.DesactivarVisibilidad();
            }
            else
            {
                mN.ActivarVisibilidad();
                vid.ActivarVisibilidad();
            }
        }
    }

}
