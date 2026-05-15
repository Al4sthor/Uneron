using UnityEngine;

public class FishableObject : MonoBehaviour
{
    public int puntos = 10;
    private bool capturado = false;
    private Transform hook;
    private Animator anim;
    public bool perdido = false;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Capturar(Transform hookTransform)
    {
        capturado = true;
        hook = hookTransform;

        if (anim != null)
            anim.enabled = false;
    }

    void Update()
    {
        if (capturado && hook != null)
        {
            transform.position = hook.position;

            RectTransform rect = GetComponent<RectTransform>();

            if (rect.anchoredPosition.y >= 1000f) // ajusta este valor
            {
                if (!perdido)
                {
                    GameManager.instance.SumarPuntos(puntos);
                    
                }
                
                hook.GetComponent<Claw>().RemoverObjeto(this);
                Destroy(gameObject);
            }
        }
       
    }
    public void MarcarComoPerdido()
    {
        perdido = true;
    }
}
