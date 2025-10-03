// Enemy.cs
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 4;
    private int currentHealth;

    private Animator animator;
    private bool isDead = false;

    public Transform player;
    private bool isWalking = false;
    private Rigidbody2D rb;
    public float speed = 2f;

        private bool canAttack = true;
    public float attackCooldown = 1f;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead || player == null) return;

        // Solo calcular diferencia en X (ignorar Y completamente)
        float distanceX = player.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);

        if (absDistanceX > 0.1f)
        {
            // Mover solo en X, Y = 0 siempre
            float directionX = Mathf.Sign(distanceX);
            rb.velocity = new Vector2(directionX * speed, 0); // ← ¡Y = 0!

            isWalking = true;

            // Voltear sprite
            if (directionX > 0)
                transform.localScale = new Vector3(7, 7, 1);
            else if (directionX < 0)
                transform.localScale = new Vector3(-7, 7, 1);
        }
        else
        {
            // Detenerse
            rb.velocity = Vector2.zero;
            isWalking = false;
        }

        animator.SetBool("isWalking", isWalking);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica que el objeto sea el jugador
        if (other.CompareTag("Player") && canAttack && !isDead)
        {
            AttackPlayer(other.gameObject);
        }
    }
      void AttackPlayer(GameObject playerObj)
    {
        // Reproducir animación de ataque
        animator.SetTrigger("attack");
        isWalking = false;
        animator.SetBool("isWalking", false);

        // Quitar vida al jugador
        ArcherController archer = playerObj.GetComponent<ArcherController>();

        if (archer != null)
        {
            archer.TakeDamage(1); // Daño configurable
        }

        // Iniciar cooldown
        canAttack = false;
        Invoke("ResetAttack", attackCooldown);
    }
        void ResetAttack()
    {
        canAttack = true;
    }

    void Die()
    {
        isDead = true;
        isWalking = false; 
        animator.SetBool("isWalking", false);
        animator.SetTrigger("die");

        // Usa la variable 'rb' existente
        if (rb != null) rb.simulated = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 1f); // Ajusta este tiempo a tu animación de muerte
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
}