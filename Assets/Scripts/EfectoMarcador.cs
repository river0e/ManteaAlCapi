using UnityEngine;
using TMPro; // Necesario para controlar el color del texto

public class EfectoMarcador : MonoBehaviour
{
    private RectTransform rectMarcador;
    private TextMeshProUGUI texto; // Referencia al componente de texto
    private Vector2 posicionOriginal;
    private float tiempoTemblor = 0f;
    private float fuerza = 15f;

    void Start()
    {
        rectMarcador = GetComponent<RectTransform>();
        texto = GetComponent<TextMeshProUGUI>(); // Pillamos el componente de texto
        posicionOriginal = rectMarcador.anchoredPosition;
    }

    void Update()
    {
        if (tiempoTemblor > 0)
        {
            float sacudidaX = Random.Range(-fuerza, fuerza);
            float sacudidaY = Random.Range(-fuerza, fuerza);
            rectMarcador.anchoredPosition = posicionOriginal + new Vector2(sacudidaX, sacudidaY);
            tiempoTemblor -= Time.deltaTime;
        }
        else
        {
            rectMarcador.anchoredPosition = posicionOriginal;
        }
    }

    public void Temblar()
    {
        tiempoTemblor = 0.3f;
    }

    // Función nueva para cambiar el color del score desde el otro script
    public void CambiarColor(Color nuevoColor)
    {
        texto.color = nuevoColor;
    }
}