using UnityEngine;

/// <summary>
/// GameManager — persiste entre escenas.
/// Coloca este script en un GameObject vacío llamado "GameManager"
/// en la PRIMERA escena del juego. Solo necesitas uno.
/// </summary>
public class GameManager2 : MonoBehaviour
{
    // ── Singleton ────────────────────────────────────────────
    public static GameManager2 Instance;

    // ════════════════════════════════════════════════════════
    // DATOS DE MINIJUEGO 2
    // ════════════════════════════════════════════════════════

    [Header("Estado de la regadera")]
    public bool regaderaConseguida = false;
    public bool regaderaConstruida = false;
    public bool regaderaCargada = false;

    [Header("Progreso del minijuego")]
    public int nivel = 0;
    public int Nmision = 0;
    public int objetivos_Cumplidos = 0;
    public int objetivos_Pendientes = 0;

    // Estado de cada objetivo por misión
    // Índice = número de misión, booleanos = estado_Objetivo1/2/3
    [Header("Estados de objetivos")]
    public bool[] estadoObj1 = new bool[10]; // ajusta el tamaño a tus misiones
    public bool[] estadoObj2 = new bool[10];
    public bool[] estadoObj3 = new bool[10];

    // ════════════════════════════════════════════════════════
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Guardar todo el estado de Minijuego_2 de una vez ────
    public void GuardarEstado(Minijuego_2 mj)
    {
        regaderaConseguida = mj.regaderaConseguida;
        regaderaConstruida = mj.regaderaConstruida;
        regaderaCargada = mj.regaderaCargada;
        nivel = mj.nivel;
        Nmision = mj.Nmision;
        objetivos_Cumplidos = mj.objetivos_Cumplidos;
        objetivos_Pendientes = mj.objetivos_Pendientes;

        if (mj.objetivos != null)
        {
            for (int i = 0; i < mj.objetivos.Length && i < estadoObj1.Length; i++)
            {
                estadoObj1[i] = mj.objetivos[i].estado_Objetivo1;
                estadoObj2[i] = mj.objetivos[i].estado_Objetivo2;
                estadoObj3[i] = mj.objetivos[i].estado_Objetivo3;
            }
        }
    }

    // ── Restaurar todo el estado en Minijuego_2 ─────────────
    public void RestaurarEstado(Minijuego_2 mj)
    {
        mj.regaderaConseguida = regaderaConseguida;
        mj.regaderaConstruida = regaderaConstruida;
        mj.regaderaCargada = regaderaCargada;
        mj.nivel = nivel;
        mj.Nmision = Nmision;
        mj.objetivos_Cumplidos = objetivos_Cumplidos;
        mj.objetivos_Pendientes = objetivos_Pendientes;

        if (mj.objetivos != null)
        {
            for (int i = 0; i < mj.objetivos.Length && i < estadoObj1.Length; i++)
            {
                mj.objetivos[i].estado_Objetivo1 = estadoObj1[i];
                mj.objetivos[i].estado_Objetivo2 = estadoObj2[i];
                mj.objetivos[i].estado_Objetivo3 = estadoObj3[i];
            }
        }
    }
}