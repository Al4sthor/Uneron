using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player1 player;
    public Minijuego_2 mN;
    public static GameManager instance;
    public TextMeshProUGUI textPoints;
    public int score;
    public Animator animator;

    void Awake()
    {
        // Singleton (solo uno en toda la escena)
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        ActualizarText();
    }

    private void Update()
    {
        if (player.puntaje > 200 && player.puntaje < 300)
        {
            animator.SetBool("Nv 2", true);
            animator.SetBool("Nv 1", false);
            if (mN.objetivos[2].estado_Objetivo1 == false)
            {
                mN.objetivos[2].estado_Objetivo1 = true;
                mN.Objetivo_Cumplido();
            }
        }
        if (player.puntaje > 301)
        {
            animator.SetBool("Nv 1", true);
            animator.SetBool("Nv 2", true);
        }
    }

    // Método para sumar puntos
    public void SumarPuntos(int points)
    {
        score += points;
        player.puntaje = score;
        ActualizarText();
    }
    void ActualizarText()
    {
        if(textPoints != null)
        {
            textPoints.text = "Points: " + score;
        }
    }
}

