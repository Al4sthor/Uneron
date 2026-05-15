using System.Collections.Generic;
using UnityEngine;

public static class Inventario
{
    // Diccionario que guarda cuántas llaves de cada tipo tiene el jugador
    private static Dictionary<string, int> llaves = new Dictionary<string, int>();

    // Resetear el inventario al iniciar el juego (por si quedaron datos viejos)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        llaves = new Dictionary<string, int>();
    }

    // Agregar una llave al inventario
    public static void AgregarLlave(string tipo)
    {
        if (!llaves.ContainsKey(tipo))
            llaves[tipo] = 0;
        llaves[tipo]++;
        Debug.Log("Llave agregada: " + tipo + " (total: " + llaves[tipo] + ")");
    }

    // Verificar si el jugador tiene la llave (sin consumirla)
    public static bool TieneLlave(string tipo)
    {
        return llaves.ContainsKey(tipo) && llaves[tipo] > 0;
    }

    // Consumir una llave (devuelve true si la tenía y la usó)
    public static bool UsarLlave(string tipo)
    {
        if (TieneLlave(tipo))
        {
            llaves[tipo]--;
            Debug.Log("Llave usada: " + tipo + " (quedan: " + llaves[tipo] + ")");
            return true;
        }
        return false;
    }

    // Cuántas llaves tiene de un tipo
    public static int Cantidad(string tipo)
    {
        return llaves.ContainsKey(tipo) ? llaves[tipo] : 0;
    }
}