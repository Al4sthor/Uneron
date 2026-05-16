using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;
public class PuzleN1 : MonoBehaviour
{


    UIDocument puzleN1;



    Button Respuesta_1;
    Button Respuesta_2;
    Button Respuesta_3;
    Button Respuesta_4;
    Button cerrar;
    VisualElement correcto;

    private void OnEnable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        puzleN1 = GetComponent<UIDocument>();

        VisualElement root = puzleN1.rootVisualElement;
        Respuesta_1 = root.Q<Button>("Respuesta_1");
        Respuesta_2 = root.Q<Button>("Respuesta_2");
        Respuesta_3 = root.Q<Button>("Respuesta_3");
        Respuesta_4 = root.Q<Button>("Respuesta_4");
        cerrar = root.Q<Button>("Cerrar");
        correcto = root.Q<VisualElement>("RespuestaCorrecta");



        //Registro de eventos
        Respuesta_1.RegisterCallback<ClickEvent>(respuestaIncorrecta);
        Respuesta_4.RegisterCallback<ClickEvent>(respuestaCorrecta);
        cerrar.RegisterCallback<ClickEvent>(cerrarVentana);
    }

    void respuestaIncorrecta(ClickEvent evt)
    {

    }
    void respuestaCorrecta(ClickEvent evt)
    {
        
        correcto.AddToClassList("respuestaCorrecta-activa");
    }
    void cerrarVentana(ClickEvent evt)
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        gameObject.SetActive(false);
    }


}
