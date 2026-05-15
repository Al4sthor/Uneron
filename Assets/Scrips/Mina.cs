
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mina : MonoBehaviour
{
    [Header("Daño y explosión")]
    public float damage = 50f;
    public float explosionRadius = 3f;
    public GameObject explosionPrefab;

    [Header("Límites")]
    [Tooltip("Cantidad máxima de minas que pueden estar activas al mismo tiempo.")]
    public int maxMinasActivas = 5;
    [Tooltip("Segundos antes de que la mina explote sola si nadie la pisa.")]
    public float tiempoVidaMaxima = 180f;
    [Tooltip("Segundos de seguridad al colocar la mina antes de armarse.")]
    public float tiempoArmado = 4f;

    [Header("Control por Escena")]
    [Tooltip("Marca esto si quieres desactivar minas manualmente en esta escena")]
    public bool minasDesactivadas = false;
    [Tooltip("Nombre exacto de la escena donde no se pueden usar minas")]
    public string escenaSinMinas = "final";

    private bool armed = false;
    private bool exploded = false;
    private bool seCuentaEnTotal = false;

    private static int minasActivas = 0;
    private static int maxGlobal = 5;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetCounter()
    {
        minasActivas = 0;
    }

    public static bool PuedeColocarMina()
    {
        return minasActivas < maxGlobal;
    }

    public static int MinasActivasActuales()
    {
        return minasActivas;
    }

    void Start()
    {
        // Bloquear minas por escena o por flag manual
        string escenaActual = SceneManager.GetActiveScene().name;
        if (escenaActual == escenaSinMinas || minasDesactivadas)
        {
            Debug.Log("Minas desactivadas en esta escena: " + escenaActual);
            Destroy(gameObject);
            return;
        }

        maxGlobal = maxMinasActivas;

        if (minasActivas >= maxGlobal)
        {
            Debug.Log("Límite de minas alcanzado. No se puede colocar otra.");
            Destroy(gameObject);
            return;
        }

        minasActivas++;
        seCuentaEnTotal = true;
        Debug.Log("Mina colocada. Activas: " + minasActivas + "/" + maxGlobal);

        Invoke("ArmMine", tiempoArmado);
        Invoke("AutoExplote", tiempoVidaMaxima);
    }

    void OnDestroy()
    {
        if (seCuentaEnTotal)
            minasActivas = Mathf.Max(0, minasActivas - 1);
    }

    void ArmMine()
    {
        armed = true;
        Debug.Log("Mina armada y activa");
    }

    void AutoExplote()
    {
        if (exploded) return;
        Debug.Log("Mina expiró sin detectar nada. Autodestruyendo.");
        exploded = true;
        ExplodeFX();
    }

    void Update()
    {
        if (!armed || exploded) return;
        DetectTargets();
    }

    void DetectTargets()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            EnemyAI_Pro enemy = hit.GetComponentInParent<EnemyAI_Pro>();
            FrydaxBoss boss = hit.GetComponentInParent<FrydaxBoss>();
            Player1 player = hit.GetComponentInParent<Player1>();

            if (enemy != null)
            {
                ExplodeEnemy(enemy);
                return;
            }
            if (boss != null)
            {
                ExplodeBoss(boss);
                return;
            }
            if (player != null)
            {
                ExplodePlayer(player);
                return;
            }
        }
    }

    void ExplodeEnemy(EnemyAI_Pro enemy)
    {
        if (exploded) return;
        exploded = true;
        enemy.TomarDaño((int)damage);
        ExplodeFX();
    }

    void ExplodeBoss(FrydaxBoss boss)
    {
        if (exploded) return;
        exploded = true;
        boss.TomarDaño((int)damage);
        ExplodeFX();
    }

    void ExplodePlayer(Player1 player)
    {
        if (exploded) return;
        exploded = true;
        player.TakeDamage(damage);
        ExplodeFX();
    }

    void ExplodeFX()
    {
        CancelInvoke("AutoExplote");
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}