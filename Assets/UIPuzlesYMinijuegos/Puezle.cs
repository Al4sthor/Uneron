using UnityEngine;
using UnityEngine.UIElements;
public class Puezle : MonoBehaviour
{
    UIDocument puzle;
    public Pregunta[] preguntas;
    public Player1 player;
    public Cofres cofre;
    public int parte;
    Button respuesta_1;
    Button respuesta_2;
    Button respuesta_3;
    Button respuesta_4;
    Button cerrar_1;
    Button cerrar_2;
    Button cerrar_3;
    Button reiniciar;
    VisualElement botones;
    VisualElement correcto;
    VisualElement incorrecto;
    VisualElement panelPregunta;
    ProgressBar time;
    float time_Max = 35f;
    float time_actual;
    bool timeActivate = false;
    bool valorRespuesta_1;
    bool valorRespuesta_2;
    bool valorRespuesta_3;
    bool valorRespuesta_4;
    Pregunta basePregunta;

    
    private void OnEnable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        puzle = GetComponent<UIDocument>();

        VisualElement root = puzle.rootVisualElement;
        
        respuesta_1 = root.Q<Button>("Respuesta_1");
        respuesta_2 = root.Q<Button>("Respuesta_2");
        respuesta_3 = root.Q<Button>("Respuesta_3");
        respuesta_4 = root.Q<Button>("Respuesta_4");
        cerrar_1 = root.Q<Button>("Cerrar_1");
        cerrar_2 = root.Q<Button>("Cerrar_2");
        cerrar_3 = root.Q<Button>("Cerrar_3");
        reiniciar = root.Q<Button>("Reiniciar");
        botones = root.Q<VisualElement>("Contenedor");
        correcto = root.Q<VisualElement>("Respuesta_Correcta");
        incorrecto = root.Q<VisualElement>("Incorrecto");
        panelPregunta = root.Q<VisualElement>("Pregunta");
        time = root.Q<ProgressBar>("BarraTiempo");

        time_actual = time_Max;
        time.highValue = time_Max;
        time.value = time_Max;
        timeActivate = true;

        Selección_Pregunta();
        respuesta_1.clicked += () => VerificarRespuesta(1);
        respuesta_2.clicked += () => VerificarRespuesta(2);
        respuesta_3.clicked += () => VerificarRespuesta(3);
        respuesta_4.clicked += () => VerificarRespuesta(4);

        cerrar_1.RegisterCallback<ClickEvent>(cerrarVentana);
        cerrar_2.RegisterCallback<ClickEvent>(cerrarVentana);
        cerrar_3.RegisterCallback<ClickEvent>(cerrarVentana);
        reiniciar.RegisterCallback<ClickEvent>(ReiniciarPregunta);
        
    }

    void Selección_Pregunta()
    {
        int index = Random.Range(0, preguntas.Length);
        basePregunta = preguntas[index];
        panelPregunta.style.backgroundImage = new StyleBackground(basePregunta.pregunta.texture);
        respuesta_1.style.backgroundImage = new StyleBackground(basePregunta.fondo_Respuesta_1.texture);
        respuesta_2.style.backgroundImage = new StyleBackground(basePregunta.fondo_Respuesta_2.texture);
        respuesta_3.style.backgroundImage = new StyleBackground(basePregunta.fondo_Respuesta_3.texture);
        respuesta_4.style.backgroundImage = new StyleBackground(basePregunta.fondo_Respuesta_4.texture);
        valorRespuesta_1 = basePregunta.rRespuesta_1;
        valorRespuesta_2 = basePregunta.rRespuesta_2;
        valorRespuesta_3 = basePregunta.rRespuesta_3;
        valorRespuesta_4 = basePregunta.rRespuesta_4;

        
    }

    void VerificarRespuesta(int nRespuesta)
    {
        timeActivate = false;
        bool CoRRecto = false;

        switch (nRespuesta) 
        {
            case 1: CoRRecto = basePregunta.rRespuesta_1; break;
                case 2: CoRRecto = basePregunta.rRespuesta_2; break;
                    case 3: CoRRecto = basePregunta.rRespuesta_3; break;
                        case 4: CoRRecto = basePregunta.rRespuesta_4; break;
        }
        if (CoRRecto)
        {
            respuestaCorrecta();
        }
        else
        {
            respuestaIncorrecta();
        }
    }

    void respuestaCorrecta()
    {
        correcto.AddToClassList("correcto_activo");
        cofre.ParteConsegida = true;
        player.parteConseguida[parte].parte = true;

    }

    void respuestaIncorrecta()
    {
        incorrecto.AddToClassList("incorrecto_activo");
    }

    void cerrarVentana(ClickEvent evt)
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        gameObject.SetActive(false);
    }

    void ReiniciarPregunta(ClickEvent evt)
    {
        correcto.RemoveFromClassList("correcto_activo");
        incorrecto.RemoveFromClassList("incorrecto_activo");
        time_actual = time_Max;
        time.value = time_Max;
        timeActivate = true;
    }

    [System.Serializable]
    public class Pregunta
    {
        public Sprite pregunta;
        public Sprite fondo_Respuesta_1;
        public Sprite fondo_Respuesta_2;
        public Sprite fondo_Respuesta_3;
        public Sprite fondo_Respuesta_4;
        public bool rRespuesta_1;
        public bool rRespuesta_2;
        public bool rRespuesta_3;
        public bool rRespuesta_4;
    }

    private void Update()
    {
        if (!timeActivate) return;
        time_actual -= Time.deltaTime;
        time.value = time_actual;
        if (time_actual <= 0)
        {
            timeActivate = false;
            respuestaIncorrecta();

        }
    }
}

