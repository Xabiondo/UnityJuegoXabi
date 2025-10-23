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

    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float shootForce = 10f;
    public AmmoIconManager ammoIconManager;

    private bool isJumping;
    public float jumpAmount = 7f;

    // ✅ Ahora representa las CARGAS del ataque especial
    private int maxSpecialCharges = 2;
    private int currentSpecialCharges;

    private bool isDead = false;
    public int maxHealth = 3;
    private int currentHealth;

    public Text ammoText; // Puedes eliminarlo si no lo usas

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

        // ✅ Inicializar cargas del ataque especial
        currentSpecialCharges = maxSpecialCharges;
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

        // Ataque especial: solo si hay cargas
        if (Input.GetMouseButtonDown(1) && currentSpecialCharges > 0)
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
        scale.x *= -1;
        transform.localScale = scale;
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
        if (currentSpecialCharges <= 0) return; // Doble verificación

        GameObject hitbox = Instantiate(specialAttackPrefab, shootPoint.position, Quaternion.identity);
        Destroy(hitbox, 2f);

        // ✅ Reducir una carga
        currentSpecialCharges--;
        
        // (Opcional) Aquí podrías actualizar una UI de cargas si la creas más tarde
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
        animator.SetTrigger("die");
        isDead = true;
        rb.velocity = Vector2.zero;
        Invoke("Respawn", 2f);
    }

    public void Respawn()
    {
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

        // ✅ Restaurar las cargas del ataque especial al respawnear
        currentSpecialCharges = maxSpecialCharges;
    }
}