

using UnityEngine;
using UnityEngine.AI;

public class CuboEscape : MonoBehaviour
{
    public Transform player;
    public Transform[] waypoints;
    public float detectionDistance = 6f;
    public UI_Llave uiLlave;
    public Minijuego_2 mn;

    [Header("Llave que entrega")]
    [Tooltip("Identificador de la llave. Las puertas con este mismo nombre se podrán abrir con ella.")]
    public string tipoLlave = "LlaveDorada";

    private NavMeshAgent agent;
    private int currentWaypoint = 0;
    private bool escapar = false;
    private bool recogido = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0.2f;
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, player.position);

        if (!escapar && distancia <= detectionDistance)
        {
            escapar = true;
            IrAlSiguientePunto();
        }

        if (escapar && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            IrAlSiguientePunto();
        }
    }

    void IrAlSiguientePunto()
    {
        if (waypoints.Length == 0) return;
        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !recogido)
        {
            recogido = true;
            Debug.Log("¡Tienes la llave!");
            if (mn.objetivos[1].estado_Objetivo2 == false )
            { 
                mn.objetivos[1].estado_Objetivo2 = true;
                mn.Objetivo_Cumplido();
            }

            // Agregar la llave al inventario del jugador
            Inventario.AgregarLlave(tipoLlave);

            // Mostrar la UI con la pista (igual que antes)
            if (uiLlave != null)
            {
                uiLlave.Mostrar();
            }
            else
            {
                Debug.LogWarning("UI_Llave no está asignado");
            }

            Destroy(gameObject);
        }
    }
}