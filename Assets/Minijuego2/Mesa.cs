using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Mesa : MonoBehaviour
{
    public Vida vid;
    public Minijuego_2 mN;
    public MesaDP ms;
    [Header("Sprites de las piezas")]
    public Sprite spriteCuerpo;
    public Sprite spriteManguera;
    public Sprite spriteAlcachofa;
    public Sprite spriteRegaderaFinal;

    // ── Referencias UI ──────────────────────────────────
    private VisualElement _root;
    private VisualElement[] _craftSlots = new VisualElement[3];
    private VisualElement[] _slotIcons = new VisualElement[3];
    private Label[] _slotHints = new Label[3];
    private VisualElement[] _piezaItems = new VisualElement[3];
    private Button[] _btnColocar = new Button[3];

    private VisualElement _resultadoSlot;
    private VisualElement _resultadoIcon;
    private Label _resultadoHint;
    private VisualElement _flashOverlay;
    private Button _btnEnsamblar;
    private Label _statusMsg;
    private Button _btnCerrar;

    // ── Estado ──────────────────────────────────────────
    private bool[] _slotLleno = new bool[3];

    // ── Nombres de clases CSS ────────────────────────────
    private const string CSS_SLOT_FILLED = "craft_slot--filled";
    private const string CSS_PIEZA_COLOCADA = "pieza_item--colocada";
    private const string CSS_BTN_DISABLED = "btn_colocar--disabled";
    private const string CSS_RESULTADO_OK = "resultado_slot--completo";

    // ════════════════════════════════════════════════════
    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }
    void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        CacheReferences();
        RegistrarEventos();
        ActualizarEstado();
    }

    // ── Cachear referencias a elementos del UXML ────────
    void CacheReferences()
    {
        for (int i = 0; i < 3; i++)
        {
            _craftSlots[i] = _root.Q<VisualElement>($"craft_slot_{i}");
            _slotIcons[i] = _root.Q<VisualElement>($"slot_icon_{i}");
            _slotHints[i] = _root.Q<Label>($"slot_hint_{i}");
            _piezaItems[i] = _root.Q<VisualElement>(GetNombrePieza(i));
            _btnColocar[i] = _root.Q<Button>($"btn_colocar_{i}");
        }

        _resultadoSlot = _root.Q<VisualElement>("resultado_slot");
        _resultadoIcon = _root.Q<VisualElement>("resultado_icon");
        _resultadoHint = _root.Q<Label>("resultado_hint");
        _flashOverlay = _root.Q<VisualElement>("flash_overlay");
        _btnEnsamblar = _root.Q<Button>("btn_ensamblar");
        _statusMsg = _root.Q<Label>("status_msg");
        _btnCerrar = _root.Q<Button>("btn_cerrar");
    }

    // ── Registrar eventos de botones ────────────────────
    void RegistrarEventos()
    {
        for (int i = 0; i < 3; i++)
        {
            int idx = i; // closure
            _btnColocar[i].clicked += () => ColocarPieza(idx);
        }

        _btnEnsamblar.clicked += () => StartCoroutine(AnimacionEnsamblar());
        _btnCerrar.clicked += CerrarPanel;
    }

    // ════════════════════════════════════════════════════
    // LÓGICA: colocar una pieza en su slot
    // ════════════════════════════════════════════════════
    void ColocarPieza(int idx)
    {
        if (_slotLleno[idx]) return;

        _slotLleno[idx] = true;

        // Mostrar imagen de la pieza en el slot
        _slotIcons[idx].style.backgroundImage = new StyleBackground(GetSpritePieza(idx));
        _slotIcons[idx].style.width = 70;
        _slotIcons[idx].style.height = 70;
        _slotHints[idx].style.display = DisplayStyle.None;

        // Marcar slot como lleno (cambia borde a verde)
        _craftSlots[idx].AddToClassList(CSS_SLOT_FILLED);

        // Atenuar la pieza del inventario
        _piezaItems[idx].AddToClassList(CSS_PIEZA_COLOCADA);
        _btnColocar[idx].AddToClassList(CSS_BTN_DISABLED);
        _btnColocar[idx].SetEnabled(false);

        ActualizarEstado();
    }

    // ── Actualizar mensaje y botón Ensamblar ────────────
    void ActualizarEstado()
    {
        int llenos = 0;
        foreach (var s in _slotLleno) if (s) llenos++;

        int faltan = 3 - llenos;
        if (faltan > 0)
        {
            _statusMsg.text = $"Faltan {faltan} pieza{(faltan > 1 ? "s" : "")}";
            _btnEnsamblar.SetEnabled(false);
            _btnEnsamblar.AddToClassList("btn_ensamblar--disabled");
        }
        else
        {
            _statusMsg.text = "¡Listo para ensamblar!";
            _btnEnsamblar.SetEnabled(true);
            _btnEnsamblar.RemoveFromClassList("btn_ensamblar--disabled");
        }
    }

    // ════════════════════════════════════════════════════
    // ANIMACIÓN: flash de destello + aparición de regadera
    // ════════════════════════════════════════════════════
    IEnumerator AnimacionEnsamblar()
    {
        _btnEnsamblar.SetEnabled(false);
        _statusMsg.text = "";



        // 1. Mostrar flash blanco
        _flashOverlay.style.display = DisplayStyle.Flex;
        _flashOverlay.style.opacity = new StyleFloat(1f);

        yield return new WaitForSeconds(0.12f);

        // 2. Ocultar flash y mostrar resultado
        _flashOverlay.style.opacity = new StyleFloat(0f);
        yield return new WaitForSeconds(0.08f);
        _flashOverlay.style.display = DisplayStyle.None;

        // 3. Mostrar imagen final
        _resultadoHint.style.display = DisplayStyle.None;
        _resultadoIcon.style.backgroundImage = new StyleBackground(spriteRegaderaFinal);
        _resultadoIcon.style.display = DisplayStyle.Flex;
        _resultadoSlot.AddToClassList(CSS_RESULTADO_OK);

        // 4. Pequeño "pop" de escala vía transición
        _resultadoIcon.style.scale = new StyleScale(new Scale(new Vector2(1.2f, 1.2f)));
        yield return new WaitForSeconds(0.12f);
        _resultadoIcon.style.scale = new StyleScale(new Scale(Vector2.one));

        _statusMsg.text = "✨ ¡Regadera ensamblada!";

        // 5. Aquí puedes disparar tu lógica de juego:
        ms.completo = true;
        mN.regaderaConstruida = true;
    }

    // ── Cerrar panel ────────────────────────────────────
    void CerrarPanel()
    {
        gameObject.SetActive(false);
        vid.ActivarVisibilidad();
        mN.ActivarVisibilidad();
    }

    // ── Reiniciar (útil para testing) ───────────────────
    public void Reiniciar()
    {
        for (int i = 0; i < 3; i++)
        {
            _slotLleno[i] = false;
            _craftSlots[i].RemoveFromClassList(CSS_SLOT_FILLED);
            _slotIcons[i].style.backgroundImage = new StyleBackground(StyleKeyword.None);
            _slotHints[i].style.display = DisplayStyle.Flex;
            _piezaItems[i].RemoveFromClassList(CSS_PIEZA_COLOCADA);
            _btnColocar[i].RemoveFromClassList(CSS_BTN_DISABLED);
            _btnColocar[i].SetEnabled(true);
        }

        _resultadoSlot.RemoveFromClassList(CSS_RESULTADO_OK);
        _resultadoIcon.style.display = DisplayStyle.None;
        _resultadoHint.style.display = DisplayStyle.Flex;
        _flashOverlay.style.display = DisplayStyle.None;

        ActualizarEstado();
    }

    // ── Helpers ─────────────────────────────────────────
    Sprite GetSpritePieza(int idx) => idx switch
    {
        0 => spriteCuerpo,
        1 => spriteManguera,
        _ => spriteAlcachofa
    };

    string GetNombrePieza(int idx) => idx switch
    {
        0 => "pieza_cuerpo",
        1 => "pieza_manguera",
        _ => "pieza_alcachofa"
    };
}
