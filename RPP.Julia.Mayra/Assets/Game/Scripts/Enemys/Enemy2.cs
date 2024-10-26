using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public float shootRange = 7f;
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public LayerMask playerLayer;
    public float speed = 2f;
    public float moveDistance = 5f;
    private int maxHealth = 5;
    private bool isdead;
    private Transform player;
    private Rigidbody2D rb;
    private float nextFireTime = 0f;
    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private int currentHealth;
    private bool movingRight = true;
    private bool isShooting = false;

    // Audio
    public AudioClip shootSound;
    public AudioClip takeDamageSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    // Animator
    public Animator animator;
    private int Transition;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        initialPosition = transform.position;
        targetPosition = initialPosition + Vector2.right * moveDistance;

        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        Transition = 0;
    }

    void Update()
    {
        if (isdead == false)
        {
            if (player != null && IsPlayerVisible())
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);

                if (distanceToPlayer <= shootRange && Time.time >= nextFireTime && IsFacingPlayer() && !isShooting)
                {
                    StartCoroutine(ShootCoroutine());
                }
                else
                {
                    if (!isShooting)
                    {
                        Move();
                    }
                    else
                    {
                        StopMovement();
                    }
                }
            }
            else
            {
                Transition = 0;
                StopMovement();
            }

            animator.SetInteger("Transition", Transition);
        }
    }

        void Move()
    {
        Transition = 1;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            movingRight = !movingRight;
            targetPosition = movingRight ? initialPosition + Vector2.right * moveDistance : initialPosition + Vector2.left * moveDistance;
            Flip();
        }
    }

    void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

    IEnumerator ShootCoroutine()
    {
        isShooting = true;
        Transition = 2;

        yield return new WaitForSeconds(0.5f);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;
            rbBullet.velocity = direction * 10f;
        }

        audioSource.PlayOneShot(shootSound);
        nextFireTime = Time.time + 1.5f / fireRate;

        yield return new WaitForSeconds(1f);
        isShooting = false;
    }

    bool IsPlayerVisible()
    {
        return (player.gameObject.layer == Mathf.Log(playerLayer.value, 2));
    }

    bool IsFacingPlayer()
    {
        return (movingRight && player.position.x > transform.position.x) || (!movingRight && player.position.x < transform.position.x);
    }

    void Flip()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.y += 180;
        transform.rotation = Quaternion.Euler(rotation);
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
            animator.SetTrigger("hit");
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

    void Die()
    {
        isdead = true;
        animator.SetTrigger("dead");

        audioSource.PlayOneShot(deathSound);
        Destroy(gameObject, 2f); // Aguarda o tempo do som de morte antes de destruir
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * moveDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * moveDistance);
    }
}
