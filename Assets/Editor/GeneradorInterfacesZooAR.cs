using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.Events;

/// <summary>
/// Genera de un clic las 5 interfaces de ZooAR, el overlay de RA, el
/// AudioManager y la navegacion, con todos los botones ya cableados.
///
/// IMPORTANTE: este archivo va en Assets/Editor/ (crea la carpeta si no existe).
/// Los otros tres scripts van en Assets/Scripts/.
///
/// Uso: menu ZooAR -> Generar interfaces.
/// Despues solo arrastras tus imagenes a los campos Source Image de cada
/// Image y tus clips a los AudioSource. Nada de armar jerarquia a mano.
/// </summary>
public static class GeneradorInterfacesZooAR
{
    // Resolucion de referencia: celular en vertical.
    private const float ANCHO = 1080f;
    private const float ALTO = 1920f;

    private static readonly Color GRIS_PANEL = new Color(0.93f, 0.92f, 0.89f, 1f);
    private static readonly Color GRIS_BOTON = new Color(0.85f, 0.84f, 0.80f, 1f);
    private static readonly Color CARBON = new Color(0.08f, 0.08f, 0.07f, 1f);
    private static readonly Color CREMA = new Color(0.95f, 0.94f, 0.91f, 1f);

    [MenuItem("ZooAR/Generar interfaces")]
    public static void Generar()
    {
        if (GameObject.Find("ZooAR_Canvas") != null)
        {
            EditorUtility.DisplayDialog(
                "ZooAR",
                "Ya existe un objeto llamado ZooAR_Canvas en la escena.\n\n" +
                "Borralo primero si quieres regenerar las interfaces desde cero.",
                "Entendido");
            return;
        }

        // ---------- Canvas ----------
        var canvasGO = new GameObject("ZooAR_Canvas",
            typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasGO, "Generar interfaces ZooAR");

        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var escalador = canvasGO.GetComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(ANCHO, ALTO);
        escalador.matchWidthOrHeight = 0.5f;

        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem",
                typeof(EventSystem), typeof(StandaloneInputModule));
            Undo.RegisterCreatedObjectUndo(es, "Generar interfaces ZooAR");
        }

        // ---------- Interfaz 1: presentacion ----------
        var i1 = CrearPanel("Interfaz1_Presentacion", canvasGO.transform);
        CrearFondo("Fondo", i1.transform);
        var btnIsologo = CrearBoton("BotonIsologo", i1.transform,
            new Vector2(520, 520), new Vector2(0, 80), "ISOLOGO\nZooAR", 46);
        CrearTexto("Pista", i1.transform, new Vector2(760, 90), new Vector2(0, -320),
            "Toca el isologo para entrar", 38, TextAnchor.MiddleCenter);

        // ---------- Interfaz 2: menu principal ----------
        var i2 = CrearPanel("Interfaz2_Menu", canvasGO.transform);
        CrearFondo("Fondo", i2.transform);
        CrearTexto("Titulo", i2.transform, new Vector2(960, 140), new Vector2(0, 740),
            "REGIONES DEL PERU", 76, TextAnchor.MiddleCenter);

        var btnCosta = CrearBoton("BotonCosta", i2.transform,
            new Vector2(880, 190), new Vector2(0, 420), "COSTA", 56);
        var btnSierra = CrearBoton("BotonSierra", i2.transform,
            new Vector2(880, 190), new Vector2(0, 190), "SIERRA", 56);
        var btnSelva = CrearBoton("BotonSelva", i2.transform,
            new Vector2(880, 190), new Vector2(0, -40), "SELVA", 56);
        var btnVideo = CrearBoton("BotonVideoAnimal", i2.transform,
            new Vector2(880, 190), new Vector2(0, -270), "VIDEO ANIMAL", 56);

        // ---------- Interfaces 3, 4 y 5: regiones ----------
        // En ZooAR el nombre va a la DERECHA de la figura.
        var i3 = CrearInterfazRegion("Interfaz3_Costa", canvasGO.transform,
            "REGION COSTA", 3);
        var i4 = CrearInterfazRegion("Interfaz4_Sierra", canvasGO.transform,
            "REGION SIERRA", 3);
        var i5 = CrearInterfazRegion("Interfaz5_Selva", canvasGO.transform,
            "REGION SELVA", 1);

        // ---------- Overlay de RA ----------
        var overlay = CrearPanel("OverlayRA", canvasGO.transform);
        var imgOverlay = overlay.GetComponent<Image>();
        if (imgOverlay != null) Object.DestroyImmediate(imgOverlay);

        CrearTexto("TituloRA", overlay.transform, new Vector2(960, 140),
            new Vector2(0, 780), "REGION COSTA", 80, TextAnchor.MiddleCenter);
        var btnRegreso = CrearBoton("BotonRegreso", overlay.transform,
            new Vector2(380, 160), new Vector2(0, -740), "REGRESAR", 48);

        // ---------- RA root ----------
        var raRoot = new GameObject("RA_Root");
        Undo.RegisterCreatedObjectUndo(raRoot, "Generar interfaces ZooAR");

        // ---------- AudioManager ----------
        var audioGO = new GameObject("AudioManager", typeof(AudioManagerZooAR));
        Undo.RegisterCreatedObjectUndo(audioGO, "Generar interfaces ZooAR");

        var fuenteFondo = new GameObject("FuenteFondo", typeof(AudioSource));
        fuenteFondo.transform.SetParent(audioGO.transform, false);
        var fuenteRA = new GameObject("FuenteRA", typeof(AudioSource));
        fuenteRA.transform.SetParent(audioGO.transform, false);

        var audioSO = new SerializedObject(audioGO.GetComponent<AudioManagerZooAR>());
        audioSO.FindProperty("fuenteFondo").objectReferenceValue =
            fuenteFondo.GetComponent<AudioSource>();
        audioSO.FindProperty("fuenteRA").objectReferenceValue =
            fuenteRA.GetComponent<AudioSource>();
        audioSO.ApplyModifiedPropertiesWithoutUndo();

        // ---------- Navegacion ----------
        var navGO = new GameObject("Navegacion", typeof(NavegacionZooAR));
        Undo.RegisterCreatedObjectUndo(navGO, "Generar interfaces ZooAR");
        var nav = navGO.GetComponent<NavegacionZooAR>();

        var navSO = new SerializedObject(nav);
        navSO.FindProperty("interfaz1Presentacion").objectReferenceValue = i1;
        navSO.FindProperty("interfaz2Menu").objectReferenceValue = i2;
        navSO.FindProperty("interfaz3Costa").objectReferenceValue = i3;
        navSO.FindProperty("interfaz4Sierra").objectReferenceValue = i4;
        navSO.FindProperty("interfaz5Selva").objectReferenceValue = i5;
        navSO.FindProperty("raRoot").objectReferenceValue = raRoot;
        navSO.FindProperty("overlayRA").objectReferenceValue = overlay;
        navSO.ApplyModifiedPropertiesWithoutUndo();

        // ---------- Cableado de botones ----------
        UnityEventTools.AddPersistentListener(btnIsologo.onClick, nav.AbrirMenu);
        UnityEventTools.AddPersistentListener(btnCosta.onClick, nav.AbrirCosta);
        UnityEventTools.AddPersistentListener(btnSierra.onClick, nav.AbrirSierra);
        UnityEventTools.AddPersistentListener(btnSelva.onClick, nav.AbrirSelva);
        UnityEventTools.AddPersistentListener(btnVideo.onClick, nav.EntrarEnRA);
        UnityEventTools.AddPersistentListener(btnRegreso.onClick, nav.SalirDeRA);

        CablearRegion(i3, nav);
        CablearRegion(i4, nav);
        CablearRegion(i5, nav);

        // Todo visible mientras editas; Start() deja solo la Interfaz 1.
        Selection.activeGameObject = canvasGO;

        Debug.Log(
            "[ZooAR] Interfaces generadas.\n" +
            "Siguiente: asigna tus imagenes en los Source Image, tus clips en los " +
            "AudioSource, y mete la ARCamera de Vuforia y los targets dentro de RA_Root.");
    }

    // ================= Constructores de bloques =================

    private static GameObject CrearInterfazRegion(
        string nombre, Transform padre, string titulo, int cuantasFiguras)
    {
        var panel = CrearPanel(nombre, padre);
        CrearFondo("Fondo", panel.transform);
        CrearTexto("Titulo", panel.transform, new Vector2(960, 140),
            new Vector2(0, 760), titulo, 72, TextAnchor.MiddleCenter);

        float[] alturas = cuantasFiguras == 1
            ? new[] { 300f }
            : new[] { 380f, 100f, -180f };

        for (int i = 0; i < cuantasFiguras; i++)
        {
            var fila = new GameObject($"Fila{i + 1}", typeof(RectTransform));
            fila.transform.SetParent(panel.transform, false);
            var rt = fila.GetComponent<RectTransform>();
            Centrar(rt, new Vector2(960, 300), new Vector2(0, alturas[i]));

            // Figura: el boton que lanza la RA.
            CrearBoton($"BotonFigura{i + 1}", fila.transform,
                new Vector2(300, 300), new Vector2(-280, 0), $"FIGURA {i + 1}", 36);

            // Nombre del animal, a la DERECHA de la figura y sin funcion interactiva.
            CrearTexto($"NombreAnimal{i + 1}", fila.transform,
                new Vector2(440, 110), new Vector2(170, 0),
                $"[ANIMAL {i + 1}]", 54, TextAnchor.MiddleLeft);
        }

        CrearBoton("BotonRetorno", panel.transform,
            new Vector2(720, 150), new Vector2(0, -640), "RETORNAR AL MENU", 48);

        return panel;
    }

    private static void CablearRegion(GameObject panel, NavegacionZooAR nav)
    {
        foreach (var btn in panel.GetComponentsInChildren<Button>(true))
        {
            if (btn.name.StartsWith("BotonFigura"))
                UnityEventTools.AddPersistentListener(btn.onClick, nav.EntrarEnRA);
            else if (btn.name == "BotonRetorno")
                UnityEventTools.AddPersistentListener(btn.onClick, nav.VolverAlMenu);
        }
    }

    // ================= Utilidades =================

    private static GameObject CrearPanel(string nombre, Transform padre)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(padre, false);
        Estirar(go.GetComponent<RectTransform>());
        var img = go.GetComponent<Image>();
        img.color = GRIS_PANEL;
        img.raycastTarget = false;
        return go;
    }

    private static Image CrearFondo(string nombre, Transform padre)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(padre, false);
        go.transform.SetAsFirstSibling();
        Estirar(go.GetComponent<RectTransform>());
        var img = go.GetComponent<Image>();
        img.color = CREMA;
        img.raycastTarget = false;
        return img;
    }

    private static Button CrearBoton(
        string nombre, Transform padre, Vector2 tam, Vector2 pos,
        string etiqueta, int tamTexto)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(padre, false);
        Centrar(go.GetComponent<RectTransform>(), tam, pos);

        var img = go.GetComponent<Image>();
        img.color = GRIS_BOTON;

        var btn = go.GetComponent<Button>();
        btn.targetGraphic = img;

        var txt = CrearTexto("Etiqueta", go.transform, tam, Vector2.zero,
            etiqueta, tamTexto, TextAnchor.MiddleCenter);
        txt.raycastTarget = false;

        return btn;
    }

    private static Text CrearTexto(
        string nombre, Transform padre, Vector2 tam, Vector2 pos,
        string contenido, int tamTexto, TextAnchor alineacion)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(padre, false);
        Centrar(go.GetComponent<RectTransform>(), tam, pos);

        var txt = go.GetComponent<Text>();
        txt.text = contenido;
        txt.fontSize = tamTexto;
        txt.alignment = alineacion;
        txt.color = CARBON;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        txt.font = FuenteUI();

        return txt;
    }

    private static Font _fuente;

    private static Font FuenteUI()
    {
        if (_fuente != null) return _fuente;

        // Unity 2022.2 en adelante.
        try { _fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); }
        catch { /* version antigua */ }

        if (_fuente == null)
        {
            try { _fuente = Resources.GetBuiltinResource<Font>("Arial.ttf"); }
            catch { /* sin fuente integrada */ }
        }

        if (_fuente == null)
            Debug.LogWarning("[ZooAR] No se encontro la fuente integrada. " +
                             "Asigna una fuente a mano en los componentes Text.");

        return _fuente;
    }

    private static void Estirar(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void Centrar(RectTransform rt, Vector2 tam, Vector2 pos)
    {
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = tam;
        rt.anchoredPosition = pos;
    }
}
