using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public bool Isjumping;

    public GameObject bulletPrefab; 
    public Transform bulletSpawnPoint;

    private Rigidbody2D rb;
    private float movement;
    private bool isGrounded;
    private float groundCheckRadius = 0.2f;
    private bool canShoot = true;
    
    private Animator animator; 
    private bool facingRight = true;

    private bool canDoubleJump;
    private bool hasDoubleJumped;

    // Audio
    public AudioClip[] sounds; // Array de sons: 0 - Pulo, 1 - Pulo Duplo, 2 - Tiro, 3 - Dano, 4 - Morte
    public AudioClip footstepSound; // Som dos passos
    private AudioSource audioSource;
    private AudioSource footstepSource; // Novo AudioSource para os passos
    private bool isWalking; // Verifica se o som de andar já está tocando

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
        audioSource = GetComponent<AudioSource>();

        // Obtendo o segundo AudioSource para passos
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.loop = true; // Configurado para repetir enquanto o jogador está andando
        footstepSource.clip = footstepSound;

        isWalking = false;
    }

    void Update()
    {
        if (GameManager.Instance.isPlayerDead == false)
        {
            JumpAndRun();
            HandleAttack();
        }
        if (GameManager.Instance.isPlayerDead == true )
        {
            StopFootstepSound(); // Para o som de passos imediatamente
        }
    }

    void JumpAndRun()
    {
        // Verifica se o jogador está no chão
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            hasDoubleJumped = false;
            canDoubleJump = true;
            Isjumping = false;

            if (movement == 0)
            {
                animator.SetInteger("Transitioon", 0); // Parado
            }
            else
            {
                animator.SetInteger("Transitioon", 1); // Correndo
            }
        }
        else if (!isGrounded && !Isjumping)
        {
            animator.SetInteger("Transitioon", 2); // Pulando
        }

        movement = Input.GetAxis("Horizontal");
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;
        rb.velocity = new Vector2(movement * currentSpeed, rb.velocity.y);

        if (movement > 0 && !facingRight)
        {
            Flip();
        }
        else if (movement < 0 && facingRight)
        {
            Flip();
        }

        // Som de passos com o novo AudioSource
        if (movement != 0 && isGrounded && !isWalking && GameManager.Instance.isPlayerDead == false)
        {
            PlayFootstepSound(); // Começa o som de passos
        }
        else if ((movement == 0 || !isGrounded) && isWalking)
        {
            StopFootstepSound(); // Para o som de passos imediatamente
        }

        // Pulo simples ou duplo
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (canDoubleJump && !hasDoubleJumped)
            {
                DoubleJump();
            }
        }
    }

    void Jump()
    {
        Isjumping = true;
        animator.SetInteger("Transitioon", 2); // Pulando
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        PlaySound(0); // Som do pulo
        StopFootstepSound(); // Parar som de passos durante o pulo
    }

    void DoubleJump()
    {
        hasDoubleJumped = true;
        animator.SetInteger("Transitioon", 2); // Pulando
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        PlaySound(1); // Som do pulo duplo
        StopFootstepSound(); // Parar som de passos durante o pulo duplo
    }
    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.F) && canShoot) 
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        if (!GameManager.Instance.Estouinvisivel)
        {
            canShoot = false; 
            animator.SetTrigger("Shoot"); 
            PlaySound(2); // Som de tiro
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = new Vector2((facingRight ? 1 : -1) * 15f, 0); 
            Destroy(bullet, 2f);
            yield return new WaitForSeconds(0.7f);
            canShoot = true; 
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1; 
        transform.localScale = scale;
    }

    public void PlaySound(int index)
    {
        if (index < sounds.Length)
        {
            audioSource.PlayOneShot(sounds[index]);
        }
    }

    public void PlayFootstepSound()
    {
        footstepSource.Play();
        isWalking = true;
    }

    private void StopFootstepSound()
    {
        footstepSource.Stop(); // Agora o som para imediatamente
        isWalking = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            Destroy(col.gameObject);
            TakeDamage(1);
        }
        if (col.gameObject.CompareTag("Morte"))
        {
            GameManager.Instance.LifePlayer = -11;
            GameManager.Instance.respawnMenu.SetActive(true);
            Die();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o jogador está colidindo com uma plataforma
        if (collision.gameObject.CompareTag("Plataforma"))
        {
            // Torna o jogador filho da plataforma
            transform.parent = collision.transform;
        }
    }

    // Quando o jogador sai da colisão com a plataforma
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Verifica se o jogador saiu de uma plataforma
        if (collision.gameObject.CompareTag("Plataforma"))
        {
            // Remove o jogador como filho da plataforma
            transform.parent = null;
        }
    }

    public void TakeDamage(int dmg)
    {
        GameManager.Instance.LifePlayer -= dmg;
        if (GameManager.Instance.LifePlayer <= 0)
        {
            Die();
        }
        else
        {
            PlaySound(3); // Som de dano
            animator.SetTrigger("damage"); // Animação de dano
        }
    }

    private void Die()
    {
        PlaySound(4); // Som de morte
        Debug.Log("Animação de morte ativada"); // Verifica se o método é chamado

        animator.SetTrigger("Dead"); // Aciona a animação de morte
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
