using UnityEngine;
public class CuboExplota : MonoBehaviour

{
    [SerializeField] private GameObject efectoExplosion;
    private bool yaExplotado = false;

    public void Explotar()
    {
        if (yaExplotado) return;

        yaExplotado = true;

        if (efectoExplosion != null)
        {
            GameObject efecto = Instantiate(efectoExplosion, transform.position, Quaternion.identity);
            Destroy(efecto, 3f);
        }

        Destroy(gameObject);
    }
}