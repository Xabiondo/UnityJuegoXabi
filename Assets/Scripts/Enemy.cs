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

        float distanceX = player.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);

        if (absDistanceX > 0.1f)
        {

            float directionX = Mathf.Sign(distanceX);
            rb.velocity = new Vector2(directionX * speed, 0);

            isWalking = true;

            if (directionX > 0)
                transform.localScale = new Vector3(7, 7, 1);
            else if (directionX < 0)
                transform.localScale = new Vector3(-7, 7, 1);
        }
        else
        {

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

        animator.SetTrigger("attack");
        isWalking = false;
        animator.SetBool("isWalking", false);

        ArcherController archer = playerObj.GetComponent<ArcherController>();

        if (archer != null)
        {
            archer.TakeDamage(1); 
        }

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