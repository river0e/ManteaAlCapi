using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class LogicaCapibara : MonoBehaviour
{
    private Rigidbody2D fisica;
    private SpriteRenderer dibujante;
    private bool estaVivo = true;
    private int toques = 0; 
    private float temporizadorChichon = 0f;
    private float gravedadBase;
    private AudioSource reproductorSonidos; 

    // Solo el temporizador para la muerte
    private float esperaParaReiniciar = 0f; 

    [Header("Ajustes de Dificultad")]
    public float gravedadInicial = 1.2f;
    public float aumentoGravedadPorSalto = 0.01f; 
    public float fuerzaSalto = 7.5f;

    [Header("Sonidos")]
    public AudioClip sonidoSalto;
    public AudioClip sonidoMuerte;
    public AudioClip sonidoChoque; 

    [Header("Imágenes Capibara")]
    public Sprite imagenTranqui; 
    public Sprite imagenSalto; 
    public Sprite imagenLoser; 
    public Sprite imagenNivelUp; 
    public Sprite imagenChichon; 
    public Sprite imagenZambullida; 
    public Sprite imagenEspacial;

    [Header("Imágenes de Fondo")]
    public SpriteRenderer renderizadorFondo; 
    public Sprite fondoCasa; 
    public Sprite fondoNubes; 
    public Sprite fondoPlanetas;
    public Sprite fondoGalaxia;

    [Header("Estilos de Marcador")]
    public TextMeshProUGUI textoMarcador;
    public TMP_FontAsset fuenteCasa; 
    public float tamañoCasa = 100f; 
    public Color colorCasa = Color.orange;
    public TMP_FontAsset fuenteNubes; 
    public float tamañoNubes = 120f; 
    public Color colorNubes = Color.blue;
    public TMP_FontAsset fuentePlanetas; 
    public float tamañoPlanetas = 100f; 
    public Color colorPlanetas = Color.yellow;
    public TMP_FontAsset fuenteGalaxia; 
    public float tamañoGalaxia = 120f; 
    public Color colorGalaxia = Color.cyan; 

    [Header("Efectos")]
    public EfectoMarcador scriptTemblor; 

    void Start()
    {
        fisica = GetComponent<Rigidbody2D>();
        dibujante = GetComponent<SpriteRenderer>(); 
        reproductorSonidos = GetComponent<AudioSource>(); 
        gravedadBase = gravedadInicial;
        fisica.gravityScale = gravedadBase;
        ActualizarEstiloNivel();

        DebugManager.instance.enableRuntimeUI = false;
        DebugManager.instance.displayRuntimeUI = false;
    }

    void Update()
    {
        // Lógica de espera al morir
        if (!estaVivo) {
            if (esperaParaReiniciar > 0) {
                esperaParaReiniciar -= Time.deltaTime; 
            } 
            else if (Input.GetMouseButtonDown(0)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
            }
            return;
        }

        if (transform.position.y < -5.1f) { PerderJuego(); }

        // El chichón funciona como siempre
        if (temporizadorChichon > 0) { 
            temporizadorChichon -= Time.deltaTime; 
            dibujante.sprite = imagenChichon; 
            return; 
        }
        
        if (transform.position.y > 3.8f) { Chocarse(); return; }

        if (Input.GetMouseButtonDown(0))
        {
            toques++; 
            gravedadBase += aumentoGravedadPorSalto;
            fisica.gravityScale = gravedadBase;

            if (reproductorSonidos != null && sonidoSalto != null) {
                reproductorSonidos.PlayOneShot(sonidoSalto);
            }
            
            float probabilidad = Random.value;
            dibujante.sprite = imagenSalto;

            if (toques >= 200) 
            {
                if (probabilidad < 0.20f) 
                {
                    fisica.gravityScale = gravedadBase * 2f; 
                    dibujante.sprite = imagenZambullida; 
                } 
                else if (probabilidad < 0.30f) 
                {
                    fisica.gravityScale = 0.2f; 
                    dibujante.sprite = imagenEspacial; 
                }
            }
            else if (toques >= 150) 
            {
                if (probabilidad < 0.10f) 
                {
                    fisica.gravityScale = gravedadBase * 2f; 
                    dibujante.sprite = imagenZambullida;
                } 
            }

            fisica.linearVelocity = new Vector2(0, fuerzaSalto);
            if (textoMarcador != null) textoMarcador.text = toques.ToString();
            
            ActualizarEstiloNivel();

            if (toques >= 50 && scriptTemblor != null) scriptTemblor.Temblar();
        }

        if (fisica.linearVelocity.y < -0.1f) {
            if (dibujante.sprite != imagenZambullida && dibujante.sprite != imagenEspacial) {
                if (toques % 10 == 0 && toques > 0) dibujante.sprite = imagenNivelUp; 
                else dibujante.sprite = imagenTranqui;
            }
        }
    }

    void ActualizarEstiloNivel()
    {
        if (toques >= 200) {
            if (renderizadorFondo != null) renderizadorFondo.sprite = fondoGalaxia;
            AplicarEstiloTexto(fuenteGalaxia, 130f, colorGalaxia);
        }
        else if (toques >= 100) { 
            if (renderizadorFondo != null) renderizadorFondo.sprite = fondoPlanetas; 
            AplicarEstiloTexto(fuentePlanetas, tamañoPlanetas, colorPlanetas); 
        }
        else if (toques >= 50) { 
            if (renderizadorFondo != null) renderizadorFondo.sprite = fondoNubes; 
            AplicarEstiloTexto(fuenteNubes, tamañoNubes, colorNubes); 
        }
        else { 
            if (renderizadorFondo != null) renderizadorFondo.sprite = fondoCasa; 
            AplicarEstiloTexto(fuenteCasa, tamañoCasa, colorCasa); 
        }
    }

    void AplicarEstiloTexto(TMP_FontAsset fuente, float tamaño, Color color)
    {
        if (textoMarcador != null) { 
            textoMarcador.font = fuente; 
            textoMarcador.fontSize = tamaño; 
            textoMarcador.color = color; 
        }
    }

    void Chocarse() { 
        if (reproductorSonidos != null && sonidoChoque != null) {
            reproductorSonidos.PlayOneShot(sonidoChoque);
        }

        temporizadorChichon = 0.5f; 
        dibujante.sprite = imagenChichon; 
        
        // Vuelve el rebote instantáneo hacia abajo
        fisica.linearVelocity = new Vector2(0, -5f); 
        
        transform.position = new Vector3(transform.position.x, 3.7f, 0); 
        if (scriptTemblor != null) scriptTemblor.Temblar(); 
    }

    void PerderJuego() { 
        if (estaVivo && reproductorSonidos != null && sonidoMuerte != null) {
            reproductorSonidos.PlayOneShot(sonidoMuerte);
        }

        estaVivo = false; 
        dibujante.sprite = imagenLoser; 
        fisica.linearVelocity = Vector2.zero; 
        fisica.simulated = false; 
        
        // Obliga a esperar 0.5 segundos antes de poder reiniciar al tocar
        esperaParaReiniciar = 0.5f; 
    }
}