using UnityEngine;

public class Cofres : MonoBehaviour
{
    public GameObject uiPuzzle;
    public Minijuego_2 mN;
    public bool ParteConsegida = false;
    private bool jugadorDentro = false;


    void Start()
    {
        if (uiPuzzle != null)
            uiPuzzle.SetActive(false);
    }

    void Update()
    {
        if (!ParteConsegida) { 
            if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
            {
                if (uiPuzzle != null)
                    uiPuzzle.SetActive(true);
                if (mN.objetivos[1].estado_Objetivo3 == false) 
                { 
                    mN.objetivos[1].estado_Objetivo3 = true;
                    mN.Objetivo_Cumplido();
                }
                
            }
        }
    }

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
