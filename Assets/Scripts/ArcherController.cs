using UnityEngine;
//using UnityEngine.SceneManagement; 

public class ArcherController : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    private Rigidbody2D rb;
    private Animator animator;

    public GameObject arrowPrefab;

    public Transform shootPoint;

    public float shootForce = 10f;

    private bool isJumping;
    public float jumpAmount = 7f;

    private bool isDead = false;

    public int maxHealth = 3;

    private int currentHealth;

    Transform spawnPoint;
    public GameObject specialAttackPrefab; // Arrástralo en el Inspector
    public float specialAttackRadius = 1.5f; // Radio del ataque (ajusta según necesites)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        spawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float horizontalMovement = horizontalInput * speed * Time.fixedDeltaTime;
        rb.velocity = new Vector2(horizontalMovement, rb.velocity.y);

        // Voltear solo si hay input y la dirección no coincide
        if (horizontalInput > 0 && transform.localScale.x < 0)
        {
            Flip();
        }
        else if (horizontalInput < 0 && transform.localScale.x > 0)
        {
            Flip();
        }

        // ACTUALIZAR animación de correr aquí para mayor fluidez
        bool isMoving = horizontalInput != 0;
        bool isRunning = isMoving && !isJumping;
        animator.SetBool("isRunning", isRunning);
    }

    void Update()
    {
        if (isDead) return;
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rb.AddForce(Vector2.up * jumpAmount, ForceMode2D.Impulse);
            isJumping = true;
            animator.SetBool("isJumping", true);
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Instancia la flecha en la posición del jugador (o cámara)
            animator.SetTrigger("Shoot");
        }
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("specialAttack");
            Invoke("CreateSpecialAttack", 0.6f);


        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            animator.SetBool("isJumping", false);
        }
    }

    private void Flip()
    {
        if (isDead) return;
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Solo invierte el signo, mantiene el tamaño
        transform.localScale = scale;
    }
    public void Shoot()
    {
        if (isDead) return;
        if (arrowPrefab == null || shootPoint == null) return;
        // Instancia la flecha en la posición del ShootPoint
        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);

        // Determina la dirección: derecha si scale.x > 0, izquierda si scale.x < 0
        Vector2 direction = new Vector2(Mathf.Sign(transform.localScale.x), 0);

        // Aplica fuerza en esa dirección
        arrow.GetComponent<Rigidbody2D>().AddForce(direction * shootForce, ForceMode2D.Impulse);
    }
    public void CreateSpecialAttack()
    {
        if (isDead) return;
        if (specialAttackPrefab == null) return;

        // Instancia el rectángulo justo en la posición del arquero
        GameObject hitbox = Instantiate(specialAttackPrefab, shootPoint.position, Quaternion.identity);

        // Se destruye solo después de 0.2 segundos (¡muy rápido!)
        Destroy(hitbox, 2f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Spikes"))
        {
            TakeDamage(1);
        }

    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
            return;
        }


        Debug.Log("Archer recibió " + damage + " daño. Vida restante: " + currentHealth);
        animator.SetTrigger("hit");

    }
    void Die()
    {
        animator.SetTrigger("die");
        isDead = true;
        // Desactiva el collider y el movimiento

        rb.velocity = Vector2.zero;

        //LLama al méotdo Respawn después de 2 segundos
        Invoke("Respawn", 2f);

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Respawn()
    {
        // Restaura la vida
        currentHealth = maxHealth;
        isDead = false;

        // Reactiva el collide

        // Mueve al jugador al punto de respawn
        transform.position = spawnPoint.position;
        animator.Play("IdleArcher"); 

    }
    
    

}