using UnityEngine;

/// <summary>
/// Navegacion entre las 5 interfaces de ZooAR, resueltas como paneles
/// dentro de una sola escena.
///
/// Recuerda desde que interfaz se entro a la realidad aumentada, para que
/// el boton de regreso del overlay devuelva siempre al sitio correcto sin
/// que haya que configurarlo caso por caso.
/// </summary>
public class NavegacionZooAR : MonoBehaviour
{
    [Header("Las 5 interfaces, en orden")]
    [SerializeField] private GameObject interfaz1Presentacion;
    [SerializeField] private GameObject interfaz2Menu;
    [SerializeField] private GameObject interfaz3Costa;
    [SerializeField] private GameObject interfaz4Sierra;
    [SerializeField] private GameObject interfaz5Selva;

    [Header("Realidad aumentada")]
    [Tooltip("GameObject padre que agrupa la ARCamera y todos los Image/Multi Targets.")]
    [SerializeField] private GameObject raRoot;

    [Tooltip("Panel con el titulo y el boton de regreso del overlay de RA.")]
    [SerializeField] private GameObject overlayRA;

    [Tooltip("Camara normal de la escena. Se apaga mientras la ARCamera de " +
             "Vuforia esta activa, porque dos camaras a la vez se pisan.")]
    [SerializeField] private GameObject camaraUI;

    private GameObject panelActual;
    private GameObject panelOrigen;

    private void Start()
    {
        MostrarSolo(interfaz1Presentacion);
        if (raRoot != null) raRoot.SetActive(false);
        if (overlayRA != null) overlayRA.SetActive(false);
        if (camaraUI != null) camaraUI.SetActive(true);
    }

    // ----- Botones de navegacion -----

    /// <summary>Isologo de la Interfaz 1. Arranca tambien el sonido de fondo.</summary>
    public void AbrirMenu()
    {
        if (AudioManagerZooAR.Instancia != null)
            AudioManagerZooAR.Instancia.IniciarFondo();

        MostrarSolo(interfaz2Menu);
    }

    public void AbrirCosta()
    {
        MostrarSolo(interfaz3Costa);
    }

    public void AbrirSierra()
    {
        MostrarSolo(interfaz4Sierra);
    }

    public void AbrirSelva()
    {
        MostrarSolo(interfaz5Selva);
    }

    public void VolverAlMenu()
    {
        MostrarSolo(interfaz2Menu);
    }

    // ----- Entrada y salida de realidad aumentada -----

    /// <summary>
    /// Cualquier boton que lance RA: las figuras de animal y el boton Video Animal.
    /// Guarda la interfaz de origen, oculta la UI, enciende Vuforia y calla el fondo.
    /// </summary>
    public void EntrarEnRA()
    {
        panelOrigen = panelActual;

        OcultarTodas();

        // Primero se apaga la camara normal y despues se enciende Vuforia,
        // para que no haya un instante con dos camaras activas.
        if (camaraUI != null) camaraUI.SetActive(false);
        if (raRoot != null) raRoot.SetActive(true);

        // El overlay lo enciende y apaga cada MarcadorZooAR segun el reconocimiento;
        // aqui solo nos aseguramos de que arranque oculto.
        if (overlayRA != null) overlayRA.SetActive(false);

        if (AudioManagerZooAR.Instancia != null)
            AudioManagerZooAR.Instancia.DetenerFondo();
    }

    /// <summary>
    /// Boton de regreso del overlay de RA. Sin parametros: vuelve solo a la
    /// interfaz desde la que se entro.
    /// </summary>
    public void SalirDeRA()
    {
        if (raRoot != null) raRoot.SetActive(false);
        if (overlayRA != null) overlayRA.SetActive(false);
        if (camaraUI != null) camaraUI.SetActive(true);

        if (AudioManagerZooAR.Instancia != null)
            AudioManagerZooAR.Instancia.SalirDeRA();

        MostrarSolo(panelOrigen != null ? panelOrigen : interfaz2Menu);
    }

    // ----- Interno -----

    private void MostrarSolo(GameObject destino)
    {
        OcultarTodas();

        if (destino != null)
        {
            destino.SetActive(true);
            panelActual = destino;
        }
    }

    private void OcultarTodas()
    {
        if (interfaz1Presentacion != null) interfaz1Presentacion.SetActive(false);
        if (interfaz2Menu != null) interfaz2Menu.SetActive(false);
        if (interfaz3Costa != null) interfaz3Costa.SetActive(false);
        if (interfaz4Sierra != null) interfaz4Sierra.SetActive(false);
        if (interfaz5Selva != null) interfaz5Selva.SetActive(false);
    }
}
