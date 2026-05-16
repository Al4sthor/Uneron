
using UnityEngine;

public class BossTrigger : MonoBehaviour
{

    [Header("Boss")]
    public GameObject bossObject;

    [Header("Cinematica")]
    public GameObject cinematicaObject;
    public ControlVideo controlVideo;

    [Header("Configuracion")]
    [Tooltip("Activa el boss al entrar a la zona")]
    public bool activarAlEntrar = true;
    [Tooltip("Segundos antes de activar el boss despues de la cinematica")]
    public float delayActivacion = 3f;

    private bool yaActivo = false;

    void Start()
    {
        if (bossObject != null)
            bossObject.SetActive(false);

        if (cinematicaObject != null)
            cinematicaObject.SetActive(false);

        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Algo entro al trigger: " + other.gameObject.name + " Tag: " + other.tag);

        if (!other.CompareTag("Player")) return;
        if (yaActivo) return;

        yaActivo = true;
        IniciarSecuencia();
        
    }

    void IniciarSecuencia()
    {
        // Reproducir video si existe
        if (controlVideo != null)
        {
            controlVideo.ReproducirVideo();
        }
        else if (cinematicaObject != null)
        {
            cinematicaObject.SetActive(true);
        }

        Invoke(nameof(ActivarBoss), delayActivacion);
    }

    void ActivarBoss()
    {
        if (bossObject != null)
            bossObject.SetActive(true);

        if (cinematicaObject != null)
            cinematicaObject.SetActive(false);
    }
}