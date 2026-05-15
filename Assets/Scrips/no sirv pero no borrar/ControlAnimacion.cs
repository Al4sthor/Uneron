using UnityEngine;

public class ControlAnimacion : MonoBehaviour
{
    public Animator animator;
    private bool jugadorCerca = false;

    void Update()
    {
        if (jugadorCerca)
        {
            // Primera animación
            if (Input.GetKeyDown(KeyCode.T))
            {
                animator.SetTrigger("saludo");
            }

            // Segunda animación (si quieres usar otra tecla)
            if (Input.GetKeyDown(KeyCode.Y))
            {
                animator.SetTrigger("saludote");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}