// SpecialAttackZone.cs
using UnityEngine;

public class SpecialAttackZone : MonoBehaviour
{
    public int damage = 10;
    public float duration = 0.1f; // Cuánto dura visible/activo (en segundos)

    void Start()
    {
        // Se destruye automáticamente después de X segundos
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided with: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            Executioner executioner = other.GetComponent<Executioner>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            if (executioner != null)
            {

                executioner.TakeDamageAndDie(damage);
            }
        }
    }
}