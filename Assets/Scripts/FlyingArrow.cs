using UnityEngine;

public class FlyingArrow : MonoBehaviour
{
    public int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Executioner executioner = collision.gameObject.GetComponent<Executioner>();
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (executioner != null)
        {
            executioner.TakeDamageAndDie(1);
            Destroy(gameObject);
            
        }
        else
        {
            // Destruir al tocar cualquier cosa (suelo, paredes, etc.)
            Destroy(gameObject);
        }
    }

    // Por si acaso no impacta, destruir después de X segundos
    void Start()
    {

        Collider2D shooterCollider = transform.parent?.GetComponent<Collider2D>();
        if (shooterCollider != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), shooterCollider);
        }
        Destroy(gameObject, 5f);
        
    }
}