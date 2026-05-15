using UnityEngine;

public class ZonaCinematica : MonoBehaviour
{
    // camara del jugador normal
    public GameObject playerCamera;

    // camara que hace la cinematic
    public GameObject cinematicCamera;

    // sonido dramatico o lo que sea
    public AudioSource cinematicSound;

    // puntos por donde se mueve la camara (la ruta basicamente)
    public Transform[] puntosCamara;

    // velocidad de movimiento
    public float velocidad = 3f;

    // velocidad de rotacion para que no gire como robot poseido
    public float velocidadRotacion = 3f;

    // para que no se repita la cinematic mil veces
    private bool yaPaso = false;

    private void OnTriggerEnter(Collider other)
    {
        // revisamos que el que entro sea el jugador
        if (other.CompareTag("Player") && !yaPaso)
        {
            yaPaso = true;
            StartCoroutine(Cinematica());
        }
    }

    System.Collections.IEnumerator Cinematica()
    {
        // apagamos camara jugador porque si no se pelean
        playerCamera.SetActive(false);

        // prendemos la camara de cinematic
        cinematicCamera.SetActive(true);

        cinematicSound.Play();

        Transform cam = cinematicCamera.transform;

        // recorremos todos los puntos
        for (int i = 0; i < puntosCamara.Length; i++)
        {
            Transform objetivo = puntosCamara[i];

            while (Vector3.Distance(cam.position, objetivo.position) > 0.05f)
            {
                // mover camara hacia el punto
                cam.position = Vector3.MoveTowards(
                    cam.position,
                    objetivo.position,
                    velocidad * Time.deltaTime
                );

                // calcular rotacion hacia el punto
                Quaternion rotacionObjetivo = Quaternion.LookRotation(objetivo.position - cam.position);

                // girar suave en vez de girar como loco
                cam.rotation = Quaternion.Slerp(
                    cam.rotation,
                    rotacionObjetivo,
                    velocidadRotacion * Time.deltaTime
                );

                yield return null;
            }
        }

        // parar sonido
        cinematicSound.Stop();

        // apagar camara cinematic
        cinematicCamera.SetActive(false);

        // volver a camara jugador
        playerCamera.SetActive(true);
    }
}
