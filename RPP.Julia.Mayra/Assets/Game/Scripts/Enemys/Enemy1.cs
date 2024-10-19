using System.Collections;
using UnityEngine;

public class EnemyFollower : MonoBehaviour
{
    public float speed = 2f;
    public float followRange = 5f;
    public float attackRange = 1f;
    public LayerMask playerLayer;
    private int maxHealth = 5;
    public float groundCheckRadius = 0.2f;
    public Transform groundCheck;
    public float attackDelay = 0.3f;

    private GameObject player;
    private Rigidbody2D rb;
    private int currentHealth;
    private bool isGrounded;
    public bool IsAttacking = false;

    // Audio
    public AudioClip attackSound;
    public AudioClip takeDamageSound;
    private AudioSource audioSource;
    private bool isWalking;

    // Animator
    public Animator animator;
    private int Transition; // Parâmetro de animação

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        isWalking = false;
        animator = GetComponent<Animator>();

        Transition = 0; // Inicializa o parâmetro de transição
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, LayerMask.GetMask("Ground"));

        if (player != null && IsPlayerVisible())
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= attackRange && !IsAttacking)
            {
                StartCoroutine(AttackPlayerCoroutine());
            }
            else if (distanceToPlayer <= followRange && !IsAttacking)
            {
                FollowPlayer();
            }
            else
            {
                StopMovement();
            }
        }
        else
        {
            StopMovement();
        }

        if (!isGrounded)
        {
            StopMovement();
        }

        // Passa o valor da variável Transition para o Animator
        animator.SetInteger("Transition", Transition);
    }

    void FollowPlayer()
    {
        Transition = 1; // Transição para animação de andar

        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

        if (!isWalking)
        {
            isWalking = true;
        }
    }

    void StopMovement()
    {
        if (IsAttacking == false)
        {
            Transition = 0; // Transição para animação de parado

            rb.velocity = new Vector2(0, rb.velocity.y);

            if (isWalking)
            {
                isWalking = false;
            }
        }
    }

    IEnumerator AttackPlayerCoroutine()
    {
        IsAttacking = true;
        Transition = 2; // Transição para animação de ataque

        StopMovement(); // Garante que o inimigo pare ao atacar

        audioSource.PlayOneShot(attackSound);

        yield return new WaitForSeconds(attackDelay);

        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(1);
        }

        yield return new WaitForSeconds(1f);
        IsAttacking = false;

        Transition = 0; // Volta para a animação de parado após o ataque
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        audioSource.PlayOneShot(takeDamageSound);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    bool IsPlayerVisible()
    {
        return (player.gameObject.layer == Mathf.Log(playerLayer.value, 2));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("BuletPlayer"))
        {
            TakeDamage(GameManager.Instance.playerDamage);
            Destroy(col.gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
