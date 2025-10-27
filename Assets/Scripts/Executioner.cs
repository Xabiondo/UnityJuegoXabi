using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Executioner : MonoBehaviour
{
    public int maxHealth = 1;
    private int currentHealth = 1;

    private Animator animator;
    private bool isDead;

    public Transform player;
    private Rigidbody2D rb;

    public float speed = 5f;
    public float detectionRange = 16f; 
    private bool canAttack = true;
    private float attackCooldown = 1f;

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

        // ✅ Solo actuar si el jugador está dentro del rango de detección
        if (absDistanceX <= detectionRange)
        {
            if (absDistanceX > 0.1f)
            {
                float directionX = Mathf.Sign(distanceX);
                rb.velocity = new Vector2(directionX * speed, 0);

                // Voltear sprite
                if (directionX > 0)
                    transform.localScale = new Vector3(7, 7, 1);
                else if (directionX < 0)
                    transform.localScale = new Vector3(-7, 7, 1);
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            // ✅ Fuera de rango: quedarse quieto
            rb.velocity = Vector2.zero;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canAttack && !isDead)
        {
            AttackPlayer(other.gameObject);
        }
    }

    void AttackPlayer(GameObject playerObj)
    {
        animator.SetTrigger("Attack");

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

    public void TakeDamageAndDie(int damage)
    {
 
        currentHealth -= damage; 

        ArcherController player = FindObjectOfType<ArcherController>();
        if (player != null)
        {
            player.addScore(20);
        }

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            animator.SetTrigger("die");
            Destroy(gameObject, 0.8f);
        }
    }
}