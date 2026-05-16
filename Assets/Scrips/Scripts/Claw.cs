using System.Collections.Generic;
using UnityEngine;

public class Claw : MonoBehaviour
{
    public float velocidadHorizontal = 200f;
    public GameObject panel;
    public Minijuego_2 mN;
    public float gravedad = 200f;
    public float fuerzaSubida = 300f;
    public float pesoPorObjeto = 80f;

    private List<FishableObject> objetosCapturados = new List<FishableObject>();

    public float minX = -800f;
    public float maxX = 800f;
    public float minY = -1280f;
    public float maxY = 800f;
    public float pesoTotal = 0f;
    public float pesoMaximo = 1300f;
    public bool sobrecargado = false;
    private RectTransform rect;
    void Start()
    {
        
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        float inputX = 0f;
        float inputY = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) inputX = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) inputX = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) inputY = 1f;

        Vector2 move = Vector2.zero;
        move.x = inputX * velocidadHorizontal;

        pesoTotal = objetosCapturados.Count * pesoPorObjeto;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panel != null)
            {
                panel.SetActive(false);
                mN.ActivarVisibilidad();
            }
        }

        if (pesoTotal >= pesoMaximo && !sobrecargado)
        {
            PerderObjetos();
            sobrecargado=true;
        }

        if (inputY > 0) 
        {
            move.y = (inputY * fuerzaSubida) - (gravedad + pesoTotal);
        }
        else
        {
            // cae automáticamente
            move.y = -(gravedad + pesoTotal);
        }

        rect.anchoredPosition += move  * Time.deltaTime;

        rect.anchoredPosition = new Vector2(
            Mathf.Clamp(rect.anchoredPosition.x, minX, maxX),
            Mathf.Clamp(rect.anchoredPosition.y, minY, maxY)
            );
        if (rect.anchoredPosition.y >= maxY - 10f) // margen pequeño
        {
            SoltarObjetos();
        }
       
    }

    void PerderObjetos()
    {
        pesoTotal = 0f;
        foreach (var obj in objetosCapturados)
        {
            if (obj != null)
            {
                obj.MarcarComoPerdido();
                Destroy(obj.gameObject);
            }
        }

        objetosCapturados.Clear();

        sobrecargado = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Objetivo"))
        {
            FishableObject obj = other.GetComponent<FishableObject>();

            if (obj != null && !objetosCapturados.Contains(obj))
            {
                obj.Capturar(transform);

                objetosCapturados.Add(obj);

            
            }
        }
    }
    void SoltarObjetos()
    {
        objetosCapturados.Clear();
    }
    public void RemoverObjeto(FishableObject obj)
    {
        if (objetosCapturados.Contains(obj))
        {
            objetosCapturados.Remove(obj);
        }
    }


}
