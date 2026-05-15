using UnityEngine;

public class enemigoSeguimiento : MonoBehaviour
{
    public Transform jugador; // aqui arrastro el player en el inspector
    public Animator anim; // animator del enemigo
    public AudioSource sonido; // sonido del enemigo cuando empieza a perseguir

    public float velocidad = 3f; // que tan rapido corre esta cosa
    public float distanciaAtaque = 2f; // cuando llega a esta distancia empieza a pegar
    public float distanciaParar = 1.5f; // distancia minima para no atravesar al jugador

    public bool activado = false; // empieza dormido hasta que algo lo active

    bool sonidoApagado = false; // esto es por si yo mismo apago el sonido con la tecla P

    void Update()
    {
        // si el enemigo no esta activado no hace nada, basicamente esta modo estatua
        if (!activado) return;

        // saco la distancia entre el enemigo y el jugador
        float distancia = Vector3.Distance(transform.position, jugador.position);

        // direccion hacia donde esta el jugador
        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0; // para que no mire raro hacia arriba o abajo

        // el enemigo gira para mirar al jugador (modo: te estoy viendo)
        transform.rotation = Quaternion.LookRotation(direccion);

        // ---------------- CONTROL DEL SONIDO ----------------

        // si presiono P cambio entre prender o apagar el sonido
        if (Input.GetKeyDown(KeyCode.P))
        {
            sonidoApagado = !sonidoApagado;

            if (sonidoApagado)
            {
                sonido.Stop(); // lo apago porque me estresa
            }
            else
            {
                sonido.Play(); // lo vuelvo a prender
            }
        }

        // si el sonido no esta apagado manualmente entonces que suene
        if (!sonidoApagado && !sonido.isPlaying)
        {
            sonido.Play();
        }

        // ---------------- PERSEGUIR ----------------

        // si esta lejos corre hacia mi
        if (distancia > distanciaAtaque)
        {
            anim.SetInteger("estado", 1); // animacion de correr

            // solo se mueve si aun no esta demasiado pegado
            if (distancia > distanciaParar)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    jugador.position,
                    velocidad * Time.deltaTime
                );
            }
        }

        // ---------------- ATACAR ----------------

        else
        {
            anim.SetInteger("estado", 2); // animacion de ataque
        }
    }
}