using UnityEngine;

/// <summary>
/// Controla el sonido de fondo de ZooAR y los sonidos de las experiencias de RA.
/// Sobrevive a los cambios de escena.
///
/// Reglas del taller que implementa:
///  - El fondo arranca en la Interfaz 1 y sigue sonando por las 5 interfaces.
///  - Se detiene SOLO al activar una experiencia de RA.
///  - Durante la RA suena unicamente el audio del modelo 3D o del video.
///  - Al regresar, el audio de RA para y el fondo se REANUDA.
///
/// Colocar en un GameObject vacio llamado "AudioManager" con dos AudioSource hijos.
/// </summary>
[DefaultExecutionOrder(-100)]
public class AudioManagerZooAR : MonoBehaviour
{
    public static AudioManagerZooAR Instancia { get; private set; }

    [Header("Fuentes de audio")]
    [Tooltip("AudioSource dedicado al sonido de fondo de ZooAR.")]
    [SerializeField] private AudioSource fuenteFondo;

    [Tooltip("AudioSource dedicado a los sonidos de los modelos 3D y del video.")]
    [SerializeField] private AudioSource fuenteRA;

    [Header("Sonido de fondo de ZooAR")]
    [SerializeField] private AudioClip clipFondo;

    [Range(0f, 1f)]
    [SerializeField] private float volumenFondo = 0.6f;

    private void Awake()
    {
        // Singleton: si ya existe uno, este sobra.
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);

        if (fuenteFondo != null)
        {
            fuenteFondo.clip = clipFondo;
            fuenteFondo.loop = true;
            fuenteFondo.playOnAwake = false;
            fuenteFondo.volume = volumenFondo;
        }

        if (fuenteRA != null)
        {
            fuenteRA.playOnAwake = false;
        }
    }

    /// <summary>Interfaz 1: se llama al tocar el isologo.</summary>
    public void IniciarFondo()
    {
        if (fuenteFondo == null || fuenteFondo.isPlaying) return;
        fuenteFondo.Play();
    }

    /// <summary>Se entra a una experiencia de RA: el fondo se calla.</summary>
    public void DetenerFondo()
    {
        if (fuenteFondo != null && fuenteFondo.isPlaying) fuenteFondo.Pause();
    }

    /// <summary>Se vuelve a una interfaz: el fondo retoma donde quedo.</summary>
    public void ReanudarFondo()
    {
        if (fuenteFondo == null || fuenteFondo.isPlaying) return;

        fuenteFondo.UnPause();

        // Si nunca llego a sonar (por ejemplo tras recargar la escena), arrancarlo.
        if (!fuenteFondo.isPlaying) fuenteFondo.Play();
    }

    /// <summary>Sonido exclusivo del modelo 3D reconocido, o audio del video.</summary>
    public void ReproducirSonidoRA(AudioClip clip, bool enBucle = true)
    {
        if (fuenteRA == null || clip == null) return;

        fuenteRA.Stop();
        fuenteRA.clip = clip;
        fuenteRA.loop = enBucle;
        fuenteRA.Play();
    }

    public void DetenerSonidoRA()
    {
        if (fuenteRA != null && fuenteRA.isPlaying) fuenteRA.Stop();
    }

    /// <summary>Atajo para el boton de regreso: corta la RA y reanuda el fondo.</summary>
    public void SalirDeRA()
    {
        DetenerSonidoRA();
        ReanudarFondo();
    }
}
