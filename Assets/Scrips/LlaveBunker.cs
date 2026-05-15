
using UnityEngine;

public class LlaveBunker : MonoBehaviour
{
    public string tipoLlave = "LlaveBunker";

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("Llave del bunker recogida");
        Inventario.AgregarLlave(tipoLlave);
        Destroy(gameObject);
    }
}