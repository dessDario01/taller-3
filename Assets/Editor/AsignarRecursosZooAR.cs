using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

/// <summary>
/// Asigna automaticamente los recursos de Assets/ZooAR a los slots de las
/// interfaces que creo GeneradorInterfacesZooAR.
///
/// Hace tres cosas que si no son muy tediosas a mano:
///   1. Convierte todas las imagenes de Assets/ZooAR/Imagenes a tipo Sprite.
///   2. Pone cada sprite en el Source Image que le toca.
///   3. Escribe los nombres reales de los animales y limpia las etiquetas
///      provisionales de los botones que ya tienen imagen.
///
/// Va en Assets/Editor/. Menu: ZooAR -> Asignar recursos.
/// Se puede volver a correr sin problema.
/// </summary>
public static class AsignarRecursosZooAR
{
    private const string IMG = "Assets/ZooAR/Imagenes/";
    private const string AUD = "Assets/ZooAR/Audio/";

    // Interfaz, fila, imagen y nombre de cada animal.
    private static readonly (string panel, int fila, string imagen, string nombre)[] ANIMALES =
    {
        ("Interfaz3_Costa",  1, "PINGUINOBOTN",   "Pinguino de Humboldt"),
        ("Interfaz3_Costa",  2, "FIG_BALLENA",    "Ballena jorobada"),
        ("Interfaz3_Costa",  3, "FIG_DELFIN",     "Delfin"),
        ("Interfaz4_Sierra", 1, "FIG_ALPACA",     "Alpaca"),
        ("Interfaz4_Sierra", 2, "FIG_CONDOR",     "Condor andino"),
        ("Interfaz4_Sierra", 3, "FIG_CHINCHILLA", "Chinchilla"),
        ("Interfaz5_Selva",  1, "FIG_JAGUAR",     "Jaguar"),
    };

    [MenuItem("ZooAR/Asignar recursos")]
    public static void Asignar()
    {
        var canvasGO = GameObject.Find("ZooAR_Canvas");
        if (canvasGO == null)
        {
            EditorUtility.DisplayDialog(
                "ZooAR",
                "No encuentro ZooAR_Canvas en la escena.\n\n" +
                "Corre primero ZooAR -> Generar interfaces.",
                "Entendido");
            return;
        }

        int sprites = ConvertirImagenesASprite();
        var raiz = canvasGO.transform;
        int puestos = 0, faltantes = 0;

        // ---------- Fondos de las cinco interfaces ----------
        foreach (var panel in new[]
        {
            "Interfaz1_Presentacion", "Interfaz2_Menu",
            "Interfaz3_Costa", "Interfaz4_Sierra", "Interfaz5_Selva"
        })
        {
            if (PonerSprite(raiz, $"{panel}/Fondo", "FONDO", false)) puestos++;
            else faltantes++;
        }

        // ---------- Interfaz 1 ----------
        if (PonerSprite(raiz, "Interfaz1_Presentacion/BotonIsologo", "ISOLOGO", true)) puestos++;
        else faltantes++;
        LimpiarEtiqueta(raiz, "Interfaz1_Presentacion/BotonIsologo");

        // ---------- Interfaz 2: mascaras del menu ----------
        var menu = new (string objeto, string imagen)[]
        {
            ("BotonCosta",       "BOTONCOSTA"),
            ("BotonSierra",      "BOTONSIERRA"),
            ("BotonSelva",       "BOTONSELVA"),
            ("BotonVideoAnimal", "BOTN_VIDEO_ANIMALES"),
        };

        foreach (var (objeto, imagen) in menu)
        {
            if (PonerSprite(raiz, $"Interfaz2_Menu/{objeto}", imagen, true)) puestos++;
            else faltantes++;
            LimpiarEtiqueta(raiz, $"Interfaz2_Menu/{objeto}");
        }

        // ---------- Interfaces 3, 4 y 5: figuras y nombres ----------
        foreach (var (panel, fila, imagen, nombre) in ANIMALES)
        {
            string rutaBoton = $"{panel}/Fila{fila}/BotonFigura{fila}";

            if (PonerSprite(raiz, rutaBoton, imagen, true)) puestos++;
            else faltantes++;

            LimpiarEtiqueta(raiz, rutaBoton);
            PonerTexto(raiz, $"{panel}/Fila{fila}/NombreAnimal{fila}", nombre);
        }

        // ---------- Overlay de RA ----------
        if (PonerSprite(raiz, "OverlayRA/BotonRegreso", "BOTON_REGRESO", true)) puestos++;
        else faltantes++;
        LimpiarEtiqueta(raiz, "OverlayRA/BotonRegreso");

        // ---------- Sonido de fondo ----------
        bool audioOk = PonerClipDeFondo();

        EditorUtility.SetDirty(canvasGO);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log(
            $"[ZooAR] Recursos asignados. Sprites convertidos: {sprites}. " +
            $"Slots cubiertos: {puestos}. Sin imagen: {faltantes}. " +
            $"Sonido de fondo: {(audioOk ? "asignado" : "NO encontrado")}.\n" +
            "Guarda la escena con Ctrl+S.");
    }

    // ================= Interno =================

    /// <summary>Marca como Sprite todas las imagenes de la carpeta.</summary>
    private static int ConvertirImagenesASprite()
    {
        if (!Directory.Exists(IMG)) return 0;

        int n = 0;
        foreach (var ruta in Directory.GetFiles(IMG))
        {
            string ext = Path.GetExtension(ruta).ToLowerInvariant();
            if (ext != ".png" && ext != ".jpg" && ext != ".jpeg") continue;

            string rutaUnity = ruta.Replace('\\', '/');
            var imp = AssetImporter.GetAtPath(rutaUnity) as TextureImporter;
            if (imp == null) continue;

            if (imp.textureType != TextureImporterType.Sprite)
            {
                imp.textureType = TextureImporterType.Sprite;
                imp.spriteImportMode = SpriteImportMode.Single;
                imp.alphaIsTransparency = true;
                imp.SaveAndReimport();
                n++;
            }
        }
        return n;
    }

    private static bool PonerSprite(
        Transform raiz, string ruta, string nombreImagen, bool preservarProporcion)
    {
        var t = raiz.Find(ruta);
        if (t == null)
        {
            Debug.LogWarning($"[ZooAR] No encuentro el objeto: {ruta}");
            return false;
        }

        var img = t.GetComponent<Image>();
        if (img == null) return false;

        var sprite = CargarSprite(nombreImagen);
        if (sprite == null)
        {
            Debug.LogWarning($"[ZooAR] Falta la imagen {nombreImagen} en {IMG}");
            return false;
        }

        Undo.RecordObject(img, "Asignar recursos ZooAR");
        img.sprite = sprite;
        img.color = Color.white;
        img.preserveAspect = preservarProporcion;
        EditorUtility.SetDirty(img);
        return true;
    }

    private static Sprite CargarSprite(string nombre)
    {
        foreach (var ext in new[] { ".png", ".jpg", ".jpeg" })
        {
            var s = AssetDatabase.LoadAssetAtPath<Sprite>(IMG + nombre + ext);
            if (s != null) return s;
        }
        return null;
    }

    /// <summary>Quita el texto provisional de un boton que ya tiene imagen.</summary>
    private static void LimpiarEtiqueta(Transform raiz, string rutaBoton)
    {
        var t = raiz.Find(rutaBoton + "/Etiqueta");
        if (t == null) return;

        var txt = t.GetComponent<Text>();
        if (txt == null || string.IsNullOrEmpty(txt.text)) return;

        Undo.RecordObject(txt, "Asignar recursos ZooAR");
        txt.text = "";
        EditorUtility.SetDirty(txt);
    }

    private static void PonerTexto(Transform raiz, string ruta, string contenido)
    {
        var t = raiz.Find(ruta);
        if (t == null) return;

        var txt = t.GetComponent<Text>();
        if (txt == null) return;

        Undo.RecordObject(txt, "Asignar recursos ZooAR");
        txt.text = contenido;
        EditorUtility.SetDirty(txt);
    }

    private static bool PonerClipDeFondo()
    {
        var manager = Object.FindFirstObjectByType<AudioManagerZooAR>();
        if (manager == null) return false;

        AudioClip clip = null;
        if (Directory.Exists(AUD))
        {
            foreach (var ruta in Directory.GetFiles(AUD))
            {
                if (Path.GetExtension(ruta).ToLowerInvariant() != ".mp3") continue;
                if (!Path.GetFileName(ruta).ToUpperInvariant().Contains("FONDO")) continue;

                clip = AssetDatabase.LoadAssetAtPath<AudioClip>(ruta.Replace('\\', '/'));
                if (clip != null) break;
            }
        }

        if (clip == null) return false;

        var so = new SerializedObject(manager);
        so.FindProperty("clipFondo").objectReferenceValue = clip;
        so.ApplyModifiedProperties();
        return true;
    }
}
