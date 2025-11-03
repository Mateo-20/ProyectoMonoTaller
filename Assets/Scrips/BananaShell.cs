using UnityEngine;

public class BananaShell : MonoBehaviour
{
    public int damage = 1;           // daño que causa a los enemigos
    public float lifetime = 5f;      // tiempo antes de destruirse

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("serpiente"))
        {
            Destroy(other.gameObject);  // destruye a la serpiente
            Destroy(gameObject);        // destruye la cáscara
        }
    }
}
