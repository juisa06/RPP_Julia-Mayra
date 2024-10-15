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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>(); // Obter o componente Rigidbody2D
        currentHealth = maxHealth; // Definir a vida inicial do inimigo
        initialPosition = transform.position; // Salva a posição inicial do inimigo
        targetPosition = initialPosition + Vector2.right * moveDistance; // Define o ponto alvo inicial
    }

    void Update()
    {
        if (player != null && IsPlayerVisible()) // Verifica se o jogador ainda está na Layer correta
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= shootRange && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1.5f / fireRate; // Calcula o próximo tempo de tiro
            }

            Move();
        }
    }

    void Move()
    {
        // Move o inimigo em direção à posição alvo
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;

        // Verifica se o inimigo chegou ao ponto alvo
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Inverte a direção do movimento
            movingRight = !movingRight;

            // Define o novo ponto alvo com base na direção
            if (movingRight)
                targetPosition = initialPosition + Vector2.right * moveDistance;
            else
                targetPosition = initialPosition + Vector2.left * moveDistance;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); // Cria o projétil na posição de disparo
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            // Calcula a direção para o tiro com base na posição do jogador
            Vector2 direction = (player.position - firePoint.position).normalized;
            rbBullet.velocity = direction * 10f; // Define a velocidade do projétil
        }
    }

    bool IsPlayerVisible()
    {
        return (player.gameObject.layer == Mathf.Log(playerLayer.value, 2)); // Verifica se o jogador está na Layer correta
    }

    public void TakeDamage()
    {
        currentHealth -= GameManager.Instance.playerDamage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "BuletPlayer")
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
