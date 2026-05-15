using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;

public class Minijuego_2 : MonoBehaviour
{
    UIDocument minijuego_2;
    public Objetivos[] objetivos;

    public bool minjuegoActivado = false;
    public bool interfazActiva =true;
    public Sprite falta_piesas;
    public Sprite semilla;
    public Sprite brote;
    public Sprite germinacion;
    public Sprite flor;
    int nivel = 0;
    public bool regaderaCargada = false;

    VisualElement regadera;
    VisualElement zona_Centro;
    ProgressBar barra;
    bool arrastrable = false;
    public bool regaderaConseguida;
    public bool regaderaConstruida;
    Vector2 offset;

    Label mision1;
    Label mision2;
    Label mision3;
    Image estado_Mision1;
    Image estado_Mision2;
    Image estado_Mision3;
    int Nmision = 0;
    public int objetivos_Cumplidos = 0;
    public int objetivos_Pendientes = 0;
    VisualElement fondo_minijuego;
    VisualElement interaccion;
    VisualElement regadera_Carga;
    VisualElement faltan_Partes;
    Label lInteraccion;

    private void Start()
    {
        Actualizar_Objetivos();
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    private void OnEnable()
    {
        minijuego_2 = GetComponent<UIDocument>();

        VisualElement root = minijuego_2.rootVisualElement;
        mision1 = root.Q<Label>("Mision1");
        mision2 = root.Q<Label>("Mision2");
        mision3 = root.Q<Label>("Mision3");
        estado_Mision1 = root.Q<Image>("Estado_Mision1");
        estado_Mision2 = root.Q<Image>("Estado_Mision2");
        estado_Mision3 = root.Q<Image>("Estado_Mision3");
        fondo_minijuego = root.Q<VisualElement>("Contenedor_Minijuego");
        interaccion = root.Q<VisualElement>("Interacciones");
        faltan_Partes = root.Q<VisualElement>("Faltan_Partes");
        lInteraccion = root.Q<Label>("LB_Interaccion");
        regadera_Carga = root.Q<VisualElement>("Estado_Regadre");
        regadera = root.Q<VisualElement>("Regadera");
        zona_Centro = root.Q<VisualElement>("Zona_Centro");
        barra = root.Q<ProgressBar>("Barra_Progreso");

        if (barra != null)
        {
            barra.lowValue = 0;
            barra.highValue = 100;
            barra.value = 0;
        }

        RegistrarEventos();
    }

    public void ActivarInteraccion()
    {
        if (interaccion == null) return;
        interaccion.AddToClassList("interacciones_Activa");
    }

    public void DesactivarInteraccion()
    {
        if (interaccion == null) return;
        interaccion.RemoveFromClassList("interacciones_Activa");
    }

    public void ActivarMinijuego()
    {
        if (regaderaConseguida && regaderaConstruida)
        {
            if (fondo_minijuego != null)
                fondo_minijuego.AddToClassList("contenedor_Visible");
            minjuegoActivado = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
        else
        {
            if (faltan_Partes != null)
                faltan_Partes.AddToClassList("contenedor_Visible2");
            minjuegoActivado = true;
        }
    }

    public void DesactivarMinijuego()
    {
        if (regaderaConseguida && regaderaConstruida)
        {
            if (fondo_minijuego != null)
                fondo_minijuego.RemoveFromClassList("contenedor_Visible");
            minjuegoActivado = false;
        }
        else
        {
            if (faltan_Partes != null)
                faltan_Partes.RemoveFromClassList("contenedor_Visible2");
            minjuegoActivado = false;
        }
    }

    void RegistrarEventos()
    {
        if (regadera == null) return;
        regadera.RegisterCallback<PointerDownEvent>(OnPointerDown);
        regadera.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        regadera.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerDown(PointerDownEvent evt)
    {
        arrastrable = true;
        offset = evt.localPosition;
        Debug.Log("si se oprimio");
    }

    void OnPointerMove(PointerMoveEvent evt)
    {
        if (!arrastrable) return;
        regadera.style.left = evt.position.x - offset.x;
        regadera.style.top = evt.position.y - offset.y;
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        arrastrable = false;
        if (Centrado())
        {
            if (regaderaCargada)
            {
                AumentarProgreso(15 * (nivel + 1));
                regaderaCargada = false;
            }
            else
            {
                if (regadera_Carga != null)
                    regadera_Carga.AddToClassList("regadera_Activa");
            }
        }
    }

    public void CargarRegadera()
    {
        regaderaCargada = true;
        if (regadera_Carga != null)
            regadera_Carga.RemoveFromClassList("regadera_Activa");
        if (objetivos[2].estado_Objetivo2 == false)
        {
            objetivos[2].estado_Objetivo2 = true;
            Objetivo_Cumplido();
        }
    }

    public void intreaccionlago()
    {
        if (lInteraccion == null || interaccion == null) return;
        lInteraccion.text = "espera un minuto para volver a cargar";
        ActivarInteraccion();
    }

    public void interacciónRegadera()
    {
        if (lInteraccion == null || interaccion == null) return;
        lInteraccion.text = "Presiona Z para cargar";
        ActivarInteraccion();
    }

    public void MostrarMensaje(string mensaje)
    {
        if (lInteraccion == null || interaccion == null) return;
        if (mensaje != null)
        {
            lInteraccion.text = "! " + mensaje + " !";
            ActivarInteraccion();
        }
    }

    public void interacciónBasica()
    {
        if (lInteraccion == null || interaccion == null) return;
        lInteraccion.text = "Presiona E";
        ActivarInteraccion();
    }

    public void ActivarVisibilidad()
    {
        if (minijuego_2 == null) return;
        minijuego_2.rootVisualElement.style.display = DisplayStyle.Flex;
        interfazActiva = true;
    }

    public void DesactivarVisibilidad()
    {
        if (minijuego_2 == null) return;
        minijuego_2.rootVisualElement.style.display = DisplayStyle.None;
        interfazActiva = false;
    }

    bool Centrado()
    {
        if (zona_Centro == null || regadera == null) return false;
        Rect zona = zona_Centro.worldBound;
        Rect objeto = regadera.worldBound;
        return zona.Overlaps(objeto);
    }

    void AumentarProgreso(float cantidad)
    {
        if (barra == null) return;
        barra.value += cantidad;

        if (barra.value > barra.highValue)
        {
            if (nivel >= 3) barra.value = barra.highValue;
            barra.value = 0;
            nivel++;
            Nivel_planta();
        }
    }

    public void Actualizar_Objetivos()
    {
        if (objetivos == null || objetivos.Length == 0) return;

        if (objetivos[Nmision].objetivo1 != null)
        {
            mision1.text = objetivos[Nmision].objetivo1;
            mision1.RemoveFromClassList("objetivo_Mision_Oculto");
            estado_Mision1.RemoveFromClassList("objetivo_Mision_Oculto");
            estado_Mision1.RemoveFromClassList("mision_Completada");
            objetivos_Pendientes++;
        }
        if (objetivos[Nmision].objetivo2 != null)
        {
            mision2.text = objetivos[Nmision].objetivo2;
            mision2.RemoveFromClassList("objetivo_Mision_Oculto");
            estado_Mision2.RemoveFromClassList("objetivo_Mision_Oculto");
            estado_Mision2.RemoveFromClassList("mision_Completada");
            objetivos_Pendientes++;
        }
        else
        {
            mision2.AddToClassList("objetivo_Mision_Oculto");
            estado_Mision2.AddToClassList("objetivo_Mision_Oculto");
        }
        if (objetivos[Nmision].objetivo3 != null)
        {
            mision3.text = objetivos[Nmision].objetivo3;
            mision3.RemoveFromClassList("objetivo_Mision_Oculto");
            estado_Mision3.RemoveFromClassList("objetivo_Mision_Oculto");
            estado_Mision3.RemoveFromClassList("mision_Completada");
            objetivos_Pendientes++;
        }
        else
        {
            mision3.AddToClassList("objetivo_Mision_Oculto");
            estado_Mision3.AddToClassList("objetivo_Mision_Oculto");
        }
    }

    public void Objetivo_Cumplido()
    {
        if (objetivos == null || objetivos.Length == 0) return;

        if (objetivos[Nmision].estado_Objetivo1)
        {
            estado_Mision1.AddToClassList("mision_Completada");
            objetivos_Cumplidos++;
        }
        if (objetivos[Nmision].estado_Objetivo2)
        {
            estado_Mision2.AddToClassList("mision_Completada");
            objetivos_Cumplidos++;
        }
        if (objetivos[Nmision].estado_Objetivo3)
        {
            estado_Mision3.AddToClassList("mision_Completada");
            objetivos_Cumplidos++;
        }

        if (objetivos_Cumplidos > 4)
        {
            Nmision++;
            objetivos_Pendientes = 0;
            objetivos_Cumplidos = 0;
            Actualizar_Objetivos();
        }
    }

    void Nivel_planta()
    {
        if (fondo_minijuego == null) return;

        if (nivel == 1)
            fondo_minijuego.style.backgroundImage = new StyleBackground(germinacion.texture);

        if (nivel == 2)
        {
            fondo_minijuego.style.backgroundImage = new StyleBackground(brote.texture);
            fondo_minijuego.style.backgroundColor = Color.green;
        }

        if (nivel == 3)
        {
            fondo_minijuego.style.backgroundImage = new StyleBackground(flor.texture);
            fondo_minijuego.style.backgroundColor = Color.blue;
        }
    }

    private void Update()
    {
    }

    [System.Serializable]
    public class Objetivos
    {
        public string mision;
        public string objetivo1;
        public string objetivo2;
        public string objetivo3;
        public bool estado_Objetivo1;
        public bool estado_Objetivo2;
        public bool estado_Objetivo3;
    }
}