using UnityEngine;
using UnityEngine.UI;

public class ArcherController : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    public HealthBar healthBarUI;
    private Rigidbody2D rb;
    private int maxAmmo = 10;
    private int currentAmmo;
    private Animator animator;

    private int puntuacion = 0; 

    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float shootForce = 10f;
    public AmmoIconManager ammoIconManager;

    private bool isJumping;
    public float jumpAmount = 7f;

    private bool isDead = false;
    public int maxHealth = 3;
    private int currentHealth;

    public Text ammoText;

    private bool estaUsandoEspecial = false; 

    public Text scoreText;

    Transform spawnPoint;
    public GameObject specialAttackPrefab;
    public float specialAttackRadius = 1.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        spawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;

        if (healthBarUI != null)
        {
            healthBarUI.SetMaxHealth(maxHealth);
            healthBarUI.SetCurrentHealth(currentHealth);
        }

        currentAmmo = maxAmmo;
        if (ammoIconManager != null)
            ammoIconManager.SetAmmo(currentAmmo);

    }

    private void FixedUpdate()
    {
        if (isDead) return;
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float horizontalMovement = horizontalInput * speed * Time.fixedDeltaTime;
        rb.velocity = new Vector2(horizontalMovement, rb.velocity.y);

        if (horizontalInput > 0 && transform.localScale.x < 0)
        {
            Flip();
        }
        else if (horizontalInput < 0 && transform.localScale.x > 0)
        {
            Flip();
        }

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

        // Disparo normal
        if (Input.GetMouseButtonDown(0) && currentAmmo > 0)
        {
            animator.SetTrigger("Shoot");
        }

        if (Input.GetMouseButtonDown(1) && currentHealth > 0 && !estaUsandoEspecial)
        {
            estaUsandoEspecial = true; 
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
    public void addScore(int points)
    {
        if (isDead) return;
        puntuacion = puntuacion + points;
        UpdateScoreUI();
        
    }

    private void Flip()
    {
        if (isDead) return;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    private void UpdateScoreUI()
{
    if (scoreText != null)
    {
        scoreText.text = puntuacion.ToString();
    }
}

    public void Shoot()
    {
        if (isDead) return;
        if (arrowPrefab == null || shootPoint == null) return;
        if (currentAmmo <= 0) return;

        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        Vector2 direction = new Vector2(Mathf.Sign(transform.localScale.x), 0);
        arrow.GetComponent<Rigidbody2D>().AddForce(direction * shootForce, ForceMode2D.Impulse);

        currentAmmo--;
        if (ammoIconManager != null)
            ammoIconManager.SetAmmo(currentAmmo);
    }

    public void CreateSpecialAttack()
    {
        if (isDead) return;
        if (specialAttackPrefab == null) return;

        // ✅ Consumir 1 de vida al usar el ataque especial
        TakeDamage(1); // Esto ya maneja la muerte si la vida llega a 0

        // Instanciar el ataque especial
        GameObject hitbox = Instantiate(specialAttackPrefab, shootPoint.position, Quaternion.identity);
        Destroy(hitbox, 2f);
        estaUsandoEspecial = false; 
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
        if (healthBarUI != null)
        {
            healthBarUI.SetCurrentHealth(currentHealth);
        }

        Debug.Log("Archer recibió " + damage + " daño. Vida restante: " + currentHealth);
        animator.ResetTrigger("hit");
        animator.SetTrigger("hit");
    }

    void Die()
    {
        currentHealth = 0;
        healthBarUI.SetCurrentHealth(0);
        animator.SetTrigger("die");
        isDead = true;
        rb.velocity = Vector2.zero;
        Invoke("Respawn", 3f);
    }

    public void Respawn()
    {
        isJumping = false; 
        
        currentHealth = maxHealth;
        isDead = false;
        transform.position = spawnPoint.position;
        animator.Play("IdleArcher");

        if (healthBarUI != null)
        {
            healthBarUI.SetCurrentHealth(currentHealth);
        }

        currentAmmo = maxAmmo;
        if (ammoIconManager != null)
            ammoIconManager.SetAmmo(currentAmmo);

    }
}