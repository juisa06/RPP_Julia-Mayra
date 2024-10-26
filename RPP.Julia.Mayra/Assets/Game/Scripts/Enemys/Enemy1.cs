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
    public bool isdead;
    private GameObject player;
    private Rigidbody2D rb;
    private int currentHealth;
    private bool isGrounded;
    public bool IsAttacking = false;

    // Audio
    public AudioClip attackSound;
    public AudioClip takeDamageSound;
    public AudioClip deathSound;
    private AudioSource audioSource;
    private bool isWalking;

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

        Transition = 0; 
    }

    void Update()
    {
        if (isdead == false)
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
            
            animator.SetInteger("Transition", Transition);
        }
    }

    void FollowPlayer()
    {
        Transition = 1; 

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
            Transition = 0; 

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
        Transition = 2; 

        StopMovement(); 

        audioSource.PlayOneShot(attackSound);

        yield return new WaitForSeconds(attackDelay);

        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(1);
        }

        yield return new WaitForSeconds(1f);
        IsAttacking = false;

        Transition = 0; 
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
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
