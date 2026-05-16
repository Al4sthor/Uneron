using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class FrydaxBoss : MonoBehaviour
{
    [Header("Jugador")]
    public Transform player;

    [Header("Vida")]
    public int vidaMaxima = 15;
    private int vidaActual;
    private bool isDead = false;

    [Header("Barra de Vida UI")]
    public Slider barraVida;
    public GameObject uiBoss;

    [Header("Fases")]
    [Tooltip("Porcentaje de vida para entrar en fase 2 (0 a 1)")]
    public float umbralFase2 = 0.6f;
    [Tooltip("Porcentaje de vida para entrar en fase 3 (0 a 1)")]
    public float umbralFase3 = 0.3f;
    private int faseActual = 1;

    [Header("Velocidades por Fase")]
    public float velocidadFase1 = 3f;
    public float velocidadFase2 = 5f;
    public float velocidadFase3 = 7f;

    [Header("Daño por Fase")]
    public int dañoFase1 = 20;
    public int dañoFase2 = 35;
    public int dañoFase3 = 50;

    [Header("Ataque Normal")]
    public float attackRange = 4f;
    public float attackCooldown = 1.5f;
    public float attackHitDelay = 0.3f;
    private float attackTimer;

    [Header("Ataque Especial (AoE)")]
    public float rangoAoe = 6f;
    public int dañoAoe = 40;
    [Tooltip("Cada cuantos segundos usa el ataque especial")]
    public float cooldownAoe = 8f;
    private float timerAoe;

    [Header("Deteccion")]
    public float detectionRange = 8f;
    public float hearingRange = 3f;
    public float viewAngle = 90f;

    [Header("Puntos de Patrulla")]
    public Transform[] patrolPoints;

    [Header("Memoria")]
    public float memoryTime = 2f;
    private float memoryTimer;
    private bool hasSeenPlayer = false;
    private Vector3 lastKnownPlayerPos;

    [Header("Respawn")]
    public bool puedeRespawnear = false;
    [Tooltip("Tiempo en minutos para reaparecer")]
    public float tiempoRespawnMinutos = 3f;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    [Header("Drop al Morir")]
    public GameObject objetoDrop;
    public Vector3 offsetDrop = new Vector3(0, 1, 0);

    [Header("Eventos")]
    public UnityEngine.Events.UnityEvent onBossDetectaJugador;
    public UnityEngine.Events.UnityEvent onBossMuere;
    public UnityEngine.Events.UnityEvent onFase2;
    public UnityEngine.Events.UnityEvent onFase3;

    private NavMeshAgent agent;
    private Animator anim;
    private int currentPoint = 0;
    private float timerPatrol = 0f;
    private bool primeraDeteccion = false;

    private enum Estado { Idle = 0, Walk = 1, Run = 2, Dead = 3 }
    private enum State { Patrol, Chase, Attack, Search }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        vidaActual = vidaMaxima;
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        agent.speed = velocidadFase1;
        agent.stoppingDistance = 0.5f;
        agent.autoBraking = false;

        currentState = State.Patrol;

        // Ocultar UI al inicio
        if (uiBoss != null) uiBoss.SetActive(false);
        if (barraVida != null)
        {
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaMaxima;
        }

        if (patrolPoints.Length > 0)
            GoToNextPoint();
    }

    void Update()
    {
        if (isDead) return;

        attackTimer += Time.deltaTime;
        timerPatrol += Time.deltaTime;
        timerAoe += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer();

        ActualizarFase();

        // Memoria
        if (canSeePlayer)
        {
            if (!primeraDeteccion)
            {
                primeraDeteccion = true;
                if (uiBoss != null) uiBoss.SetActive(true);
                onBossDetectaJugador?.Invoke();
            }
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

        if (!canSeePlayer && distance <= hearingRange)
        {
            lastKnownPlayerPos = player.position;
            hasSeenPlayer = true;
        }

        // Ataque especial AoE independiente del estado
        if (timerAoe >= cooldownAoe && distance <= rangoAoe)
        {
            AtaqueEspecial();
            timerAoe = 0f;
        }

        switch (currentState)
        {
            case State.Patrol:
                agent.isStopped = false;
                agent.stoppingDistance = 0.5f;
                anim.SetInteger("stix", (int)Estado.Walk);

                if (timerPatrol >= 0.5f &&
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

                if (canSeePlayer) currentState = State.Chase;
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                    currentState = State.Patrol;
                break;
        }
    }

    void ActualizarFase()
    {
        float porcentajeVida = (float)vidaActual / vidaMaxima;

        if (porcentajeVida <= umbralFase3 && faseActual < 3)
        {
            faseActual = 3;
            agent.speed = velocidadFase3;
            attackCooldown = 0.8f;
            onFase3?.Invoke();
            Debug.Log("BOSS FASE 3 - MODO FURIA");
        }
        else if (porcentajeVida <= umbralFase2 && faseActual < 2)
        {
            faseActual = 2;
            agent.speed = velocidadFase2;
            attackCooldown = 1.2f;
            onFase2?.Invoke();
            Debug.Log("BOSS FASE 2");
        }
    }

    int DañoActual()
    {
        if (faseActual == 3) return dañoFase3;
        if (faseActual == 2) return dañoFase2;
        return dañoFase1;
    }

    void HitPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Player1 p = player.GetComponent<Player1>();
            if (p != null)
            {
                p.TakeDamage(DañoActual());
                Debug.Log("Boss golpeo al jugador - Fase " + faseActual);
            }
        }
    }

    void AtaqueEspecial()
    {
        anim.SetTrigger("attack");
        Invoke(nameof(AplicarDañoAoe), attackHitDelay);
        Debug.Log("Boss usa ataque especial AoE");
    }

    void AplicarDañoAoe()
    {
        float distancia = Vector3.Distance(transform.position, player.position);
        if (distancia <= rangoAoe)
        {
            Player1 p = player.GetComponent<Player1>();
            if (p != null)
            {
                p.TakeDamage(dañoAoe);
                Debug.Log("Boss daño AoE al jugador");
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
        vidaActual -= dañoRecibido;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        if (barraVida != null)
            barraVida.value = vidaActual;

        if (vidaActual <= 0)
            Morir();
    }

    void Morir()
    {
        isDead = true;
        agent.isStopped = true;
        agent.enabled = false;

        anim.SetInteger("stix", (int)Estado.Dead);

        if (uiBoss != null) uiBoss.SetActive(false);

        // Soltar drop
        if (objetoDrop != null)
            Instantiate(objetoDrop, transform.position + offsetDrop, Quaternion.identity);

        onBossMuere?.Invoke();

        if (puedeRespawnear)
        {
            float segundos = tiempoRespawnMinutos * 60f;
            Invoke(nameof(OcultarCuerpo), 2f);
            Invoke(nameof(Respawnear), segundos);
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
        vidaActual = vidaMaxima;
        isDead = false;
        faseActual = 1;
        primeraDeteccion = false;
        hasSeenPlayer = false;
        attackTimer = 0f;
        timerAoe = 0f;
        timerPatrol = 0f;
        currentPoint = 0;
        attackCooldown = 1.5f;

        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        gameObject.SetActive(true);
        agent.enabled = true;
        agent.isStopped = false;
        agent.speed = velocidadFase1;

        if (barraVida != null) barraVida.value = vidaMaxima;

        anim.SetInteger("stix", (int)Estado.Walk);

        if (patrolPoints.Length > 0)
            GoToNextPoint();

        Debug.Log("Boss ha reaparecido");
    }
}