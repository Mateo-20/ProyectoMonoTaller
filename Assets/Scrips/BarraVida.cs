using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Image rellenoBarraVida;
    private MovimientoMono playerController;
    private float vidaMaxima;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<MovimientoMono>();
        vidaMaxima = playerController.vidas;
    }

    // Update is called once per frame
    void Update()
    {
        rellenoBarraVida.fillAmount = playerController.vidas / vidaMaxima;
    }
}
