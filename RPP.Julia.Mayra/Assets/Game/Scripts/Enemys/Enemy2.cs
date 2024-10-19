using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public float shootRange = 7f; // Distância em que o inimigo começa a atirar
    public float fireRate = 1f; // Taxa de tiro (em segundos)
    public GameObject bulletPrefab; // Prefab do projétil
    public Transform firePoint; // Ponto de origem do projétil
    public LayerMask playerLayer; // Layer do jogador
    public float speed = 2f; // Velocidade de movimento
    public float moveDistance = 5f; // Distância ajustável de movimento
    private int maxHealth = 5; // Vida máxima do inimigo

    private Transform player;
    private Rigidbody2D rb; // Referência ao Rigidbody2D do inimigo
    private float nextFireTime = 0f;
    private Vector2 initialPosition; // Posição inicial do inimigo
    private Vector2 targetPosition; // Posição atual do alvo
    private int currentHealth; // Vida atual do inimigo
    private bool movingRight = true; // Direção do movimento
    private bool isShooting = false; // Verifica se está atirando

    // Audio
    public AudioClip shootSound; // Som do tiro
    public AudioClip takeDamageSound; // Som ao tomar dano
    private AudioSource audioSource; // Fonte de áudio

    // Animator
    public Animator animator;
    private int Transition; // Parâmetro de animação

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        initialPosition = transform.position;
        targetPosition = initialPosition + Vector2.right * moveDistance;

        // Configurar o componente de áudio
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        Transition = 0; // Inicializa a transição de animação
    }

    void Update()
    {
        if (player != null && IsPlayerVisible()) // Verifica se o jogador está na Layer correta
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Verifica se o inimigo deve atirar
            if (distanceToPlayer <= shootRange && Time.time >= nextFireTime && IsFacingPlayer() && !isShooting)
            {
                StartCoroutine(ShootCoroutine());
            }
            else
            {
                // Só movimenta se não estiver atirando
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
            // Inimigo em modo idle se o jogador não estiver visível
            Transition = 0; // Transição para animação de idle
            StopMovement();
        }

        // Atualiza o parâmetro de animação
        animator.SetInteger("Transition", Transition);
    }

    void Move()
    {
        Transition = 1; // Transição para animação de andar

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;

        // Verifica se o inimigo chegou ao ponto alvo
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            movingRight = !movingRight;

            // Define o novo ponto alvo com base na direção
            if (movingRight)
                targetPosition = initialPosition + Vector2.right * moveDistance;
            else
                targetPosition = initialPosition + Vector2.left * moveDistance;

            // Gira o inimigo para a nova direção
            Flip();
        }
    }

    void StopMovement()
    {
        rb.velocity = Vector2.zero; // Para o movimento
    }

    IEnumerator ShootCoroutine()
    {
        isShooting = true;
        Transition = 2; // Transição para animação de tiro

        // Criar projétil e tocar o som de tiro
        yield return new WaitForSeconds(0.5f); // Tempo para começar a atirar

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;
            rbBullet.velocity = direction * 10f; // Define a velocidade do projétil
        }

        // Toca o som de tiro
        audioSource.PlayOneShot(shootSound);

        // Define o próximo tempo de tiro
        nextFireTime = Time.time + 1.5f / fireRate;

        yield return new WaitForSeconds(1f); // Tempo de espera após atirar

        isShooting = false; // Permite que o inimigo atire novamente
    }

    bool IsPlayerVisible()
    {
        return (player.gameObject.layer == Mathf.Log(playerLayer.value, 2)); // Verifica se o jogador está na Layer correta
    }

    bool IsFacingPlayer()
    {
        // Verifica se o inimigo está virado na direção do jogador
        return (movingRight && player.position.x > transform.position.x) || (!movingRight && player.position.x < transform.position.x);
    }

    void Flip()
    {
        // Inverte a direção do inimigo visualmente
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.y += 180; // Inverte a rotação no eixo Y
        transform.rotation = Quaternion.Euler(rotation);
    }

    public void TakeDamage()
    {
        currentHealth -= GameManager.Instance.playerDamage;

        // Tocar som de tomar dano
        audioSource.PlayOneShot(takeDamageSound);

        if (currentHealth <= 0)
        {
            Die();
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
        Destroy(gameObject); // Destroi o inimigo
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        Gizmos.color = Color.blue;

        // Desenha a área de movimento do inimigo
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * moveDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * moveDistance);
    }
}
