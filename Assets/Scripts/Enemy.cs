using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data;
    public Transform pointA;
    public Transform pointB;
    public Transform player;

    private int currentHealth;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isDead = false;
    private bool isChasing = false; // ← nuevo: ¿está persiguiendo al jugador?
    private Vector2 originalScale;
    private bool canAttack = true;
    private Vector3 patrolTarget;

    void Start()
    {
        currentHealth = data.maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;

        // Inicializar patrulla si no está persiguiendo
        if (pointA == null || pointB == null)
        {
            GameObject pointAObj = new GameObject("PointA");
            pointAObj.transform.position = transform.position + Vector3.left * 6f;
            pointA = pointAObj.transform;

            GameObject pointBObj = new GameObject("PointB");
            pointBObj.transform.position = transform.position + Vector3.right * 6f;
            pointB = pointBObj.transform;
        }
        patrolTarget = pointB.position;
    }

    void Update()
    {
        if (isDead || player == null) return;

        if (isChasing)
        {
            // Perseguir al jugador
            float distanceX = player.position.x - transform.position.x;
            float directionX = Mathf.Sign(distanceX);

            rb.velocity = new Vector2(directionX * data.speed, rb.velocity.y);

            // Voltear sprite
            if (directionX > 0)
                transform.localScale = new Vector3(originalScale.x, originalScale.y, 1);
            else if (directionX < 0)
                transform.localScale = new Vector3(-originalScale.x, originalScale.y, 1);

            animator.SetBool("isWalking", Mathf.Abs(distanceX) > 0.1f);
        }
        else
        {
            // Patrullar
            transform.position = Vector3.MoveTowards(transform.position, patrolTarget, data.speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, patrolTarget) < 0.05f)
            {
                patrolTarget = (patrolTarget == pointA.position) ? pointB.position : pointA.position;
            }

            // Voltear según dirección de patrulla
            float dir = Mathf.Sign(patrolTarget.x - transform.position.x);
            if (dir > 0)
                transform.localScale = new Vector3(originalScale.x, originalScale.y, 1);
            else if (dir < 0)
                transform.localScale = new Vector3(-originalScale.x, originalScale.y, 1);

            animator.SetBool("isWalking", true);
        }
    }

void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player") && !isDead)
    {
        // Al entrar, activar persecución y preparar ataque
        isChasing = true;
    }
}

void OnTriggerStay2D(Collider2D other)
{
    if (other.CompareTag("Player") && canAttack && !isDead)
    {
        AttackPlayer(other.gameObject);
    }
}

    void AttackPlayer(GameObject playerObj)
    {
        animator.SetTrigger("attack");
        animator.SetBool("isWalking", false);
        rb.velocity = Vector2.zero;

        ArcherController archer = playerObj.GetComponent<ArcherController>();
        if (archer != null)
        {
            archer.TakeDamage(data.attackDamage);
        }

        canAttack = false;
        Invoke("ResetAttack", data.attackCooldown);
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("hit");

        // Al recibir daño, empieza a perseguir al jugador
        isChasing = true;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {

        ArcherController player = FindObjectOfType<ArcherController>();
        if (player != null)
        {
            player.addScore(20);
        }
        isDead = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("die");

        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 1f);
    }
}