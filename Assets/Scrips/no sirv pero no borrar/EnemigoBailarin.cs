using UnityEngine;

public class EnemigoBailarin : MonoBehaviour
{
    public Animator anim; // animator del robot bailarín
    public AudioSource sonido; // musica del infierno remix

    public bool activado = false; // empieza congelado

    bool yaEmpezo = false; // para no activarlo mil veces

    void Update()
    {
        if (!activado) return;

        if (!yaEmpezo)
        {
            anim.SetTrigger("bailar"); // activa la transicion desde idle
            sonido.Play(); // empieza el show
            yaEmpezo = true;
        }
    }
}