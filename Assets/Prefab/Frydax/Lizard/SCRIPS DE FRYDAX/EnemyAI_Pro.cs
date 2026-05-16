
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI_Pro : MonoBehaviour
{
    [Header("Jugador")]
    public Transform player;

    [Header("Puntos de Patrulla")]
    public Transform[] patrolPoints;

    [Header("Vida")]
    public int vida = 3;
    private bool isDead = false;

    [Header("Ataque")]
    public int damage = 20;
    public float detectionRange = 5f;
    public float attackRange = 3.5f;
    public float hearingRange = 2f;
    public float viewAngle = 60f;
    public float attackCooldown = 1.5f;
    private float attackTimer;
    public float attackHitDelay = 0.3f;

    [Header("Respawn")]
    public bool puedeRespawnear = true;
    [Tooltip("Tiempo en minutos antes de que reaparezca")]
    public float tiempoRespawnMinutos = 1f;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private int vidaInicial;

    [Header("Memoria")]
    private bool hasSeenPlayer = false;
    private float memoryTimer;
    public float memoryTime = 0.5f;

    private NavMeshAgent agent;
    private Animator anim;

    private int currentPoint = 0;
    private Vector3 lastKnownPlayerPos;

    // fix para patrol - tiempo minimo entre puntos
    private float timerPatrol = 0f;
    private float minTimerPatrol = 0.5f;

    private enum Estado { Idle = 0, Walk = 1, Run = 2, Dead = 3 }
    private enum State { Patrol, Chase, Attack, Search }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Guardar posicion y vida inicial para respawn
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        vidaInicial = vida;

        agent.stoppingDistance = 0.5f;
        agent.autoBraking = false;

        currentState = State.Patrol;

        if (patrolPoints.Length > 0)
            GoToNextPoint();
    }

    void Update()
    {
        if (isDead) return;

        attackTimer += Time.deltaTime;
        timerPatrol += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer();

        // Memoria
        if (canSeePlayer)
        {
            hasSeenPlayer = true;
            memoryTimer = memoryTime;
            lastKnownPlayerPos = player.position;
        }
        else
        {
            memoryTimer -= Time.deltaTime;
            if (memoryTimer <= 0f)
                hasSeenPlayer = false;
        }

        // Sonido
        if (!canSeePlayer && distance <= hearingRange)
        {
            lastKnownPlayerPos = player.position;
            hasSeenPlayer = true;
        }

        switch (currentState)
        {
            case State.Patrol:
                agent.isStopped = false;
                agent.stoppingDistance = 0.5f;

                anim.SetInteger("stix", (int)Estado.Walk);

                // Fix principal: esperar timer Y distancia para evitar titilado
                if (timerPatrol >= minTimerPatrol &&
                    !agent.pathPending &&
                    agent.remainingDistance <= agent.stoppingDistance)
                {
                    GoToNextPoint();
                    timerPatrol = 0f;
                }

                if (hasSeenPlayer)
                    currentState = State.Chase;
                break;

            case State.Chase:
                agent.isStopped = false;
                agent.stoppingDistance = attackRange;

                if (distance <= detectionRange || distance <= hearingRange)
                    agent.SetDestination(player.position);
                else
                {
                    hasSeenPlayer = false;
                    agent.stoppingDistance = 0.5f;
                    currentState = State.Patrol;
                    GoToNextPoint();
                }

                anim.SetInteger("stix", (int)Estado.Run);

                if (distance <= attackRange)
                    currentState = State.Attack;
                break;

            case State.Attack:
                agent.isStopped = true;
                LookAtPlayer();

                if (attackTimer >= attackCooldown)
                {
                    anim.SetTrigger("attack");
                    attackTimer = 0f;
                    Invoke(nameof(HitPlayer), attackHitDelay);
                }

                if (distance > attackRange)
                {
                    agent.isStopped = false;
                    agent.stoppingDistance = 0.5f;
                    currentState = State.Chase;
                }
                break;

            case State.Search:
                agent.isStopped = false;
                agent.stoppingDistance = 0.5f;
                agent.SetDestination(lastKnownPlayerPos);

                anim.SetInteger("stix", (int)Estado.Walk);

                if (canSeePlayer)
                    currentState = State.Chase;

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                    currentState = State.Patrol;
                break;
        }
    }

    void HitPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Player1 p = player.GetComponent<Player1>();
            if (p != null)
            {
                p.TakeDamage(damage);
                Debug.Log("Frydax golpeo al jugador");
            }
        }
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.SetDestination(patrolPoints[currentPoint].position);
        currentPoint = (currentPoint + 1) % patrolPoints.Length;
    }

    bool CanSeePlayer()
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 dir = (player.position - origin).normalized;

        if (Vector3.Angle(transform.forward, dir) < viewAngle / 2f)
        {
            if (Physics.Raycast(origin, dir, out RaycastHit hit, detectionRange))
                return hit.transform == player;
        }
        return false;
    }

    void LookAtPlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    public void TomarDaño(int dañoRecibido)
    {
        if (isDead) return;
        vida -= dañoRecibido;
        if (vida <= 0)
            Morir();
    }

    void Morir()
    {
        isDead = true;
        agent.isStopped = true;
        agent.enabled = false;

        anim.SetInteger("stix", (int)Estado.Dead);

        if (puedeRespawnear)
        {
            // Convertir minutos a segundos
            float segundos = tiempoRespawnMinutos * 60f;
            Invoke(nameof(Respawnear), segundos);
            // Ocultar el objeto en lugar de destruirlo
            Invoke(nameof(OcultarCuerpo), 2f);
        }
        else
        {
            Destroy(gameObject, 2f);
        }
    }

    void OcultarCuerpo()
    {
        gameObject.SetActive(false);
    }

    void Respawnear()
    {
        // Reiniciar todo
        vida = vidaInicial;
        isDead = false;
        currentState = State.Patrol;
        currentPoint = 0;
        hasSeenPlayer = false;
        attackTimer = 0f;
        timerPatrol = 0f;

        // Volver a la posicion inicial
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        // Reactivar componentes
        gameObject.SetActive(true);
        agent.enabled = true;
        agent.isStopped = false;

        anim.SetInteger("stix", (int)Estado.Walk);

        if (patrolPoints.Length > 0)
            GoToNextPoint();

        Debug.Log("Frydax ha reaparecido");
    }
}