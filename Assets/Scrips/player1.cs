using UnityEngine;
using UnityEngine.SceneManagement;

public class Player1 : MonoBehaviour
{
    // ---------------- MINIJUEGO ----------------
    public Minijuego_2 mN;
    public Vida vid;
    public PartesConseguidas[] parteConseguida;

    // ---------------- PUNTAJE ----------------
    public float puntaje;
    public float puntajeLimite = 150f;

    // ---------------- MOVIMIENTO ----------------
    public float speed = 3f;
    public float runMultiplier = 2f;
    public float jumpForce = 5f;

    public Light luz;

    private Animator anim;
    private Rigidbody rb;
    private bool isGrounded;

    private Vector3 direccionMovimiento = Vector3.zero;
    private float velocidadActual = 0f;

    [HideInInspector] public float targetRotationY; // La camara lo lee

    // ---------------- VIDA ----------------
    public float maxHealth = 100f;
    public float currentHealth;
    private bool isDead = false;

    // ---------------- MINAS ----------------
    public GameObject minaPrefab;
    public Transform spawnMinaPoint;
    public float mineCooldown = 1f;
    private float mineTimer;

    // ---------------- TRANSICION ----------------
    public GameObject canvasTransicion;
    public float tiempoEspera = 3f;
    public string nombreEscenaDestino = "Desarrollo";

    // ---------------- MUERTE ----------------
    public GameObject canvasReintentar;
    public string escenaMenu = "Menu";
    private CanvasGroup canvasGroup;

    // ---------------- ESTADOS ----------------
    private enum Estado
    {
        Idle = 0,
        Forward = 1,
        Run = 2,
        Left = 3,
        Right = 4,
        Back = 5,
        ForwardRight = 6,
        ForwardLeft = 7,
        BackRight = 8,
        BackLeft = 9,
        Dead = 10
    }

    private Estado estadoActual = Estado.Idle;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        currentHealth = maxHealth;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        targetRotationY = transform.eulerAngles.y;

        if (canvasTransicion != null)
            canvasTransicion.SetActive(false);

        if (canvasReintentar != null)
        {
            canvasGroup = canvasReintentar.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }

    void Update()
    {
        if (isDead) return;

        HandleMine();
        CalcularMovimiento();
        HandleJump();
        HandleInteract();
        HandleRotacion();       // Solo rota el cuerpo, sin Slerp
        Activar_Minijuego();
        VerificarPartes();

        if (puntaje >= puntajeLimite && mN.regaderaConstruida)
            ActivarTransicion();

        if (Input.GetKeyDown(KeyCode.R) && luz != null)
            luz.enabled = !luz.enabled;
    }

    void FixedUpdate()
    {
        if (isDead) return;
        AplicarMovimiento();
    }

    // ---------------- ROTACION DEL CUERPO ----------------
    // Solo gira el cuerpo en Y con el mouse. Sin Slerp = sin traba.
    // La camara maneja su propio angulo vertical por separado.
    void HandleRotacion()
    {
        float mouseX = Input.GetAxis("Mouse X");
        targetRotationY += mouseX * 2f;
        transform.rotation = Quaternion.Euler(0f, targetRotationY, 0f);
    }

    // ---------------- MOVIMIENTO ----------------
    void CalcularMovimiento()
    {
        bool w = Input.GetKey(KeyCode.W);
        bool s = Input.GetKey(KeyCode.S);
        bool a = Input.GetKey(KeyCode.A);
        bool d = Input.GetKey(KeyCode.D);
        bool run = Input.GetKey(KeyCode.LeftShift);

        Estado newState;

        if (w && d) newState = Estado.ForwardRight;
        else if (w && a) newState = Estado.ForwardLeft;
        else if (s && d) newState = Estado.BackRight;
        else if (s && a) newState = Estado.BackLeft;
        else if (w) newState = run ? Estado.Run : Estado.Forward;
        else if (s) newState = Estado.Back;
        else if (a) newState = Estado.Left;
        else if (d) newState = Estado.Right;
        else newState = Estado.Idle;

        estadoActual = newState;
        anim.SetInteger("stix", (int)estadoActual);

        Vector3 dir = Vector3.zero;

        switch (estadoActual)
        {
            case Estado.Forward:
            case Estado.Run:
                dir = transform.forward;
                if (!mN.objetivos[0].estado_Objetivo3)
                {
                    mN.objetivos[0].estado_Objetivo3 = true;
                    mN.Objetivo_Cumplido();
                }
                break;

            case Estado.Back: dir = -transform.forward; break;
            case Estado.Left: dir = -transform.right; break;
            case Estado.Right: dir = transform.right; break;

            case Estado.ForwardRight:
                dir = (transform.forward + transform.right).normalized; break;
            case Estado.ForwardLeft:
                dir = (transform.forward - transform.right).normalized; break;
            case Estado.BackRight:
                dir = (-transform.forward + transform.right).normalized; break;
            case Estado.BackLeft:
                dir = (-transform.forward - transform.right).normalized; break;
        }

        direccionMovimiento = dir;
        velocidadActual = (estadoActual == Estado.Run) ? speed * runMultiplier : speed;
    }

    void AplicarMovimiento()
    {
        Vector3 move = direccionMovimiento.normalized * velocidadActual * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    // ---------------- MINAS ----------------
    void HandleMine()
    {
        mineTimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Q) && mineTimer >= mineCooldown)
        {
            if (minaPrefab != null && spawnMinaPoint != null)
            {
                Instantiate(minaPrefab, spawnMinaPoint.position, Quaternion.identity);
                mineTimer = 0f;
            }
        }
    }

    // ---------------- SALTO ----------------
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            anim.SetTrigger("jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (!mN.objetivos[0].estado_Objetivo1)
            {
                mN.objetivos[0].estado_Objetivo1 = true;
                mN.Objetivo_Cumplido();
            }
        }
    }

    // ---------------- INTERACT ----------------
    void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.ResetTrigger("interact");
            anim.SetTrigger("interact");

            if (!mN.objetivos[0].estado_Objetivo2)
            {
                mN.objetivos[0].estado_Objetivo2 = true;
                mN.Objetivo_Cumplido();
            }
        }
    }

    // ---------------- MINIJUEGO ----------------
    void Activar_Minijuego()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!mN.minjuegoActivado) mN.ActivarMinijuego();
            else mN.DesactivarMinijuego();
        }
    }

    // ---------------- VIDA ----------------
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        vid.ActualizarVida();
        if (currentHealth <= 0) Morir();
    }

    void Morir()
    {
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        anim.SetInteger("stix", (int)Estado.Dead);
        Invoke(nameof(MostrarCanvas), 3f);
    }

    void MostrarCanvas()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            mN.DesactivarVisibilidad();
            vid.DesactivarVisibilidad();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Reintentar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        mN.ActivarVisibilidad();
        vid.ActivarVisibilidad();
    }

    public void IrAlMenu()
    {
        SceneManager.LoadScene(escenaMenu);
    }

    // ---------------- GROUND ----------------
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    // ---------------- TRANSICION ----------------
    void ActivarTransicion()
    {
        if (canvasTransicion != null)
        {
            canvasTransicion.SetActive(true);
            Invoke(nameof(CambiarEscena), tiempoEspera);
            mN.DesactivarVisibilidad();
            vid.DesactivarVisibilidad();
        }
    }

    void CambiarEscena()
    {
        SceneManager.LoadScene(nombreEscenaDestino);
    }

    // ---------------- PARTES ----------------
    void VerificarPartes()
    {
        if (parteConseguida == null || parteConseguida.Length < 3) return;

        if (parteConseguida[0].parte && parteConseguida[1].parte && parteConseguida[2].parte)
        {
            mN.regaderaConseguida = true;
            mN.objetivos[2].estado_Objetivo3 = true;
            mN.Objetivo_Cumplido();
        }
    }

    [System.Serializable]
    public class PartesConseguidas
    {
        public bool parte = false;
    }
}