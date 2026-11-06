using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Vuforia;

/// <summary>
/// Va en el Image Target del dibujo a mano alzada (experiencia Video Animal).
///
/// Es el caso aparte: aqui el audio no es un AudioClip del AudioManager sino
/// la pista del propio video, asi que el fondo se calla pero el sonido de RA
/// lo emite el VideoPlayer.
/// </summary>
[RequireComponent(typeof(ObserverBehaviour))]
public class MarcadorVideoZooAR : MonoBehaviour
{
    [Header("Video")]
    [Tooltip("VideoPlayer con el clip educativo, maximo 1 minuto.")]
    [SerializeField] private VideoPlayer reproductor;

    [Tooltip("Quad o RawImage donde se proyecta el video.")]
    [SerializeField] private GameObject pantallaVideo;

    [Header("Overlay")]
    [SerializeField] private GameObject overlayUI;
    [SerializeField] private Text titulo;
    [SerializeField] private string textoTitulo = "VIDEO ANIMAL";

    private ObserverBehaviour observador;

    private void Awake()
    {
        observador = GetComponent<ObserverBehaviour>();

        if (observador != null)
            observador.OnTargetStatusChanged += AlCambiarEstado;

        if (reproductor != null)
        {
            reproductor.playOnAwake = false;
            reproductor.isLooping = false;
        }

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
        if (pantallaVideo != null) pantallaVideo.SetActive(true);
        if (overlayUI != null) overlayUI.SetActive(true);
        if (titulo != null) titulo.text = textoTitulo;

        // El fondo se calla; el sonido de la experiencia es el del video.
        if (AudioManagerZooAR.Instancia != null)
            AudioManagerZooAR.Instancia.DetenerFondo();

        if (reproductor != null && !reproductor.isPlaying)
            reproductor.Play();
    }

    private void Ocultar()
    {
        if (reproductor != null && reproductor.isPlaying)
            reproductor.Pause();

        if (pantallaVideo != null) pantallaVideo.SetActive(false);
        if (overlayUI != null) overlayUI.SetActive(false);
    }
}
