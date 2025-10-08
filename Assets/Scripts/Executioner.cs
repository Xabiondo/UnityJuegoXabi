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

    public float speed = 3f;

    private bool canAttack = true;



    private float attackCooldown = 1f;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();


    }

    // Update is called once per frame
    void Update()
    {
        if (isDead || player == null) return;

        float distanceX = player.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);

        if (absDistanceX > 0.1f)
        {

            float directionX = Mathf.Sign(distanceX);
            rb.velocity = new Vector2(directionX * speed, 0);



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
    

}
