

using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;

    public float detectionRange = 10f;
    public float attackRange = 3.5f;
    public float minDistance = 3.0f;

    public float attackCooldown = 1.5f;
    private float attackTimer = 0f;

    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.stoppingDistance = attackRange;
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange && distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            anim.SetInteger("stix", 1);
        }
        else if (distance <= attackRange)
        {
            agent.isStopped = true;

            LookAtPlayer();

            anim.SetInteger("stix", 2);

            if (attackTimer >= attackCooldown)
            {
                anim.SetTrigger("attack");
                attackTimer = 0f;
            }
        }
        else
        {
            agent.isStopped = true;
            anim.SetInteger("stix", 0);
        }
    }

    void LateUpdate()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < minDistance)
        {
            Vector3 dir = (transform.position - player.position).normalized;
            transform.position = player.position + dir * minDistance;
        }
    }

    void LookAtPlayer()
    {
        Vector3 dir = (player.position - transform.position);
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    void HitPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Debug.Log("Golpeo al jugador");
        }
    }
}