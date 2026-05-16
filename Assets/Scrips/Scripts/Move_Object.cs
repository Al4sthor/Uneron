
using UnityEngine;

public class Move_Object : MonoBehaviour
{

    public float speed = 200f;

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Si sale de la pantalla (izquierda)
        if (transform.position.x < 39f)
        {
            Destroy(gameObject);
        }
    }
}
