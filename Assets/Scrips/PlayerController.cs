using UnityEngine;
using System.Collections;
using Unity.Cinemachine;


public class MovimientoMono : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad;
    public float fuerzaSalto;
    public Rigidbody2D rb2d;
    bool tocandoSuelo = false;

    [Header("Ataque con Cáscaras")]
    public GameObject cascaraPrefab;
    public Transform throwPoint;
    public float fuerzaLanzamiento = 8f;
    public int cantidadCáscaras = 0;

    [Header("Daño y Retroceso")]
    public float retrocesoFuerza = 5f;
    public float tiempoInmunidad = 0.5f;
    private bool estaHerido = false;

    private Animator animator;

    public CinemachineBasicMultiChannelPerlin shake;
    public float shakeTime;

    public int vidas = 3;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (estaHerido) return;

        // --- Movimiento horizontal ---
        float movimientoHorizontal = Input.GetAxis("Horizontal");
        rb2d.linearVelocity = new Vector2(movimientoHorizontal * velocidad, rb2d.linearVelocity.y);

        // --- Dirección del mono ---
        if (movimientoHorizontal > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (movimientoHorizontal < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // --- Salto ---
        if (Input.GetKeyDown(KeyCode.W) && tocandoSuelo)
        {
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, fuerzaSalto);
            animator.SetTrigger("Jump");
        }

        // --- Animaciones de movimiento ---
        animator.SetFloat("Speed", Mathf.Abs(movimientoHorizontal));
        animator.SetBool("isJumping", !tocandoSuelo);

        // --- Lanzar cáscara ---
        if (Input.GetKeyDown(KeyCode.Return) && cantidadCáscaras > 0)
        {
            LanzarCascara();
            cantidadCáscaras--;
        }
    }

    void LanzarCascara()
    {
        GameObject nuevaCascara = Instantiate(cascaraPrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D rbCascara = nuevaCascara.GetComponent<Rigidbody2D>();
        rbCascara.linearVelocity = new Vector2(transform.localScale.x * fuerzaLanzamiento, 0f);
    }

    // --- Detección de colisiones con enemigos y objetos ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Banana"))
        {
            Destroy(collision.gameObject);
            cantidadCáscaras++;
        }

        if (collision.gameObject.CompareTag("serpiente") && !estaHerido)
        {
            
            shake.enabled = true;
            StartCoroutine(RecibirDaño(collision.transform));
            Invoke("ApagarShake", shakeTime);
        }
    }
    public void ApagarShake()
    {
        shake.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("serpiente") && !estaHerido)
        {
            
            StartCoroutine(RecibirDaño(collision.transform));
        }
    }

    private IEnumerator RecibirDaño(Transform enemigo)
    {
        estaHerido = true;

        // 🔹 Activa la animación inmediatamente
        animator.SetTrigger("HurtTrigger");
        animator.Update(0f); // fuerza a actualizar el frame

        // 🔹 Retroceso instantáneo
        Vector2 direccion = (transform.position - enemigo.position).normalized;
        rb2d.linearVelocity = new Vector2(direccion.x * retrocesoFuerza, rb2d.linearVelocity.y + 2f);

        vidas--;
        Debug.Log("Vidas restantes: " + vidas);

        if (vidas <= 0)
        {
            Debug.Log("Wukong ha muerto");
            animator.SetBool("Die", true); 
            rb2d.linearVelocity = Vector2.zero; 
            this.enabled = false; 
            yield break;
        }

        // Espera el tiempo de inmunidad
        yield return new WaitForSeconds(tiempoInmunidad);

        estaHerido = false;
    }

    // --- Suelo ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("suelo"))
        {
            tocandoSuelo = true;
            animator.ResetTrigger("Jump");
        }

        if (collision.gameObject.CompareTag("serpiente") && !estaHerido)
        {
            StartCoroutine(RecibirDaño(collision.transform));
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("suelo"))
        {
            tocandoSuelo = false;
        }
    }
}