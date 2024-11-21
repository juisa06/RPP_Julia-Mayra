using System.Collections;
using UnityEngine;

public class EnemyFollower : MonoBehaviour
{
    public float speed = 2f;
    public float followRange = 5f;
    public float attackRange = 1f;
    public LayerMask playerLayer;
    private int maxHealth = 5;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public bool isdead;

    private GameObject player;
    private Rigidbody2D rb;
    private int currentHealth;
    private bool isGrounded;
    private bool isWalking;

    // Audio
    public AudioClip takeDamageSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    // Animator
    public Animator animator;
    private int Transition;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        isWalking = false;
        animator = GetComponent<Animator>();

        Transition = 0; // Idle por padrão
    }

    void Update()
    {
        if (!isdead && GameManager.Instance.isPlayerDead == false)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, LayerMask.GetMask("Ground"));

            if (player != null && IsPlayerVisible())
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

                if (distanceToPlayer <= attackRange)
                {
                    StopMovement(); // Fica parado no alcance
                }
                else if (distanceToPlayer <= followRange)
                {
                    FollowPlayer(); // Segue o jogador
                }
                else
                {
                    StopMovement(); // Fica parado fora do alcance
                }
            }
            else
            {
                StopMovement(); // Fica parado se o jogador não for visível
            }

            animator.SetInteger("Transition", Transition);
        }
    }

    void FollowPlayer()
    {
        Transition = 1; // Animação de movimento

        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

        // Girar o inimigo na direção do jogador
        if (direction.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0); // Virado para a direita
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0); // Virado para a esquerda
        }

        isWalking = true;
    }

    void StopMovement()
    {
        Transition = 0; // Idle
        rb.velocity = new Vector2(0, rb.velocity.y);
        isWalking = false;
    }

    public void TakeDamage()
    {
        currentHealth -= GameManager.Instance.playerDamage;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            audioSource.PlayOneShot(takeDamageSound);
            animator.SetTrigger("Damage");
        }
    }

    void Die()
    {
        isdead = true;
        animator.SetTrigger("dead");
        audioSource.PlayOneShot(deathSound);
        Destroy(gameObject, 2);
    }

    bool IsPlayerVisible()
    {
        return (player.gameObject.layer == Mathf.Log(playerLayer.value, 2));
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Player playerScript = col.gameObject.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(1); // Dano ao jogador ao colidir
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("BuletPlayer"))
        {
            TakeDamage();
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
