
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    [Header("Configuracion")]
    [Tooltip("Marca esto si es el checkpoint de la escena Inicio")]
    public bool esEscenaInicio = false;

    private bool yaGuardo = false;

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (yaGuardo) return;
        yaGuardo = true;

        Player1 player = other.GetComponent<Player1>();
        if (player == null) return;

        if (esEscenaInicio)
        {
            Debug.Log("[CHECKPOINT] Escena Inicio. Creando jugador y partida...");
            ApiManager.Instance.CrearJugador();
        }
        else
        {
            float vida = player.currentHealth;
            float puntos = player.puntaje;
            string escena = SceneManager.GetActiveScene().name;
            int minas = Mina.MinasActivasActuales();

            // Posicion actual del jugador
            Vector3 pos = player.transform.position;

            // Estado de las piezas de la regadera
            bool pieza1 = player.parteConseguida != null && player.parteConseguida.Length > 0 && player.parteConseguida[0].parte;
            bool pieza2 = player.parteConseguida != null && player.parteConseguida.Length > 1 && player.parteConseguida[1].parte;
            bool pieza3 = player.parteConseguida != null && player.parteConseguida.Length > 2 && player.parteConseguida[2].parte;

            string inventario = "{\"pieza1\":" + pieza1.ToString().ToLower() +
                                ",\"pieza2\":" + pieza2.ToString().ToLower() +
                                ",\"pieza3\":" + pieza3.ToString().ToLower() +
                                ",\"minas\":" + minas + "}";

            string objetivos = "{\"regaderaConstruida\":" + player.mN.regaderaConstruida.ToString().ToLower() + "}";

            Debug.Log("[CHECKPOINT] Guardando progreso. Escena: " + escena +
                      " Vida: " + vida + " Puntos: " + puntos +
                      " Pos: (" + pos.x + "," + pos.y + "," + pos.z + ")" +
                      " Inventario: " + inventario);

            ApiManager.Instance.GuardarProgreso(vida, puntos, escena, pos, inventario, objetivos);
        }
    }
}