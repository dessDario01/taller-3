using UnityEngine;
using UnityEngine.UI;
using Vuforia;

/// <summary>
/// Se coloca en CADA Image Target / Multi Target de ZooAR (los 3 de Costa,
/// los 3 de Sierra, el Merge Cube de Selva y el dibujo a mano del Video Animal).
///
/// Reglas del taller que implementa:
///  - El titulo y el boton de regreso aparecen UNICAMENTE cuando se reconoce el marcador.
///  - Al reconocer, el sonido de fondo se detiene y suena el sonido exclusivo del modelo.
///  - Al perder el marcador, el sonido del modelo se detiene.
/// </summary>
[RequireComponent(typeof(ObserverBehaviour))]
public class MarcadorZooAR : MonoBehaviour
{
    [Header("Contenido que aparece al reconocer el marcador")]
    [Tooltip("Modelo 3D con texturas, o el reproductor de video.")]
    [SerializeField] private GameObject contenido3D;

    [Tooltip("Panel con el titulo y el boton de regreso.")]
    [SerializeField] private GameObject overlayUI;

    [Header("Titulo del overlay")]
    [Tooltip("Si usas TextMeshPro, cambia Text por TMP_Text y agrega using TMPro.")]
    [SerializeField] private Text titulo;

    [Tooltip("VIDEO ANIMAL, REGION COSTA, REGION SIERRA o REGION SELVA.")]
    [SerializeField] private string textoTitulo = "REGION COSTA";

    [Header("Sonido exclusivo de este modelo")]
    [Tooltip("Debe ser distinto al de los demas animales.")]
    [SerializeField] private AudioClip sonidoModelo;
    [SerializeField] private bool sonidoEnBucle = true;

    private ObserverBehaviour observador;

    private void Awake()
    {
        observador = GetComponent<ObserverBehaviour>();

        if (observador != null)
            observador.OnTargetStatusChanged += AlCambiarEstado;

        // Nada visible hasta que Vuforia reconozca el marcador.
        Ocultar();
    }

    private void OnDestroy()
    {
        if (observador != null)
            observador.OnTargetStatusChanged -= AlCambiarEstado;
    }

    private void AlCambiarEstado(ObserverBehaviour comportamiento, TargetStatus estado)
    {
        bool reconocido = estado.Status == Status.TRACKED
                       || estado.Status == Status.EXTENDED_TRACKED;

        if (reconocido) Mostrar();
        else Ocultar();
    }

    private void Mostrar()
    {
        if (contenido3D != null) contenido3D.SetActive(true);
        if (overlayUI != null) overlayUI.SetActive(true);
        if (titulo != null) titulo.text = textoTitulo;

        if (AudioManagerZooAR.Instancia != null)
        {
            AudioManagerZooAR.Instancia.DetenerFondo();
            AudioManagerZooAR.Instancia.ReproducirSonidoRA(sonidoModelo, sonidoEnBucle);
        }
    }

    private void Ocultar()
    {
        if (contenido3D != null) contenido3D.SetActive(false);
        if (overlayUI != null) overlayUI.SetActive(false);

        // Se perdio el reconocimiento: el PDF pide detener el sonido del modelo.
        if (AudioManagerZooAR.Instancia != null)
            AudioManagerZooAR.Instancia.DetenerSonidoRA();
    }
}
