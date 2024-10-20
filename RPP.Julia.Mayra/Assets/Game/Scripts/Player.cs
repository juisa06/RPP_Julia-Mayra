using System;
using System.Collections;
using System.Collections.Generic;
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
    public AudioClip[] sounds; // Array de sons: 0 - Pulo, 1 - Pulo Duplo, 2 - Tiro
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
        JumpAndRun();
        HandleAttack();
    }

    void JumpAndRun()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            hasDoubleJumped = false;
            canDoubleJump = true;
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
        if (movement != 0 && isGrounded && !isWalking)
        {
            PlayFootstepSound(); // Começa o som de passos
        }
        else if ((movement == 0 || !isGrounded) && isWalking)
        {
            StopFootstepSound(); // Para o som de passos imediatamente
        }

        if (movement != 0)
        {
            animator.SetInteger("Transitioon", 1); // Correndo
        }
        else
        {
            animator.SetInteger("Transitioon", 0); // Parado
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
        animator.SetInteger("Transitioon", 2); // Pulando
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        PlaySound(0); // Som do pulo
        StopFootstepSound(); // Parar som de passos durante o pulo
    }

    void DoubleJump()
    {
        hasDoubleJumped = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        animator.SetInteger("Transitioon", 2); // Pulando
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
            bulletRb.velocity = new Vector2((facingRight ? 1 : -1) * 10f, 0); 
            Destroy(bullet, 5f);
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

    private void PlaySound(int index)
    {
        if (index < sounds.Length)
        {
            audioSource.PlayOneShot(sounds[index]);
        }
    }

    private void PlayFootstepSound()
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
            GameManager.Instance.LifePlayer--;
        }
        if (col.gameObject.CompareTag("Morte"))
        {
            GameManager.Instance.LifePlayer = 0;
        }
    }

    public void TakeDamage(int Dmg)
    {
        GameManager.Instance.LifePlayer--;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
