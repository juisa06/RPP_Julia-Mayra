using System.Collections;
using UnityEngine;

public class EnemyFollower : MonoBehaviour
{
    public float speed = 2f; // Velocidade de movimento do inimigo
    public float followRange = 5f; // Distância em que o inimigo começa a seguir o jogador
    public float attackRange = 1f; // Distância em que o inimigo começa a atacar o jogador
    public LayerMask playerLayer; // Layer do jogador
    private int maxHealth = 5; 
    public float groundCheckRadius = 0.2f; // Raio para verificar se o inimigo está no chão
    public Transform groundCheck; // Ponto de verificação do chão
    public float attackDelay = 0.3f; // Tempo de espera antes de atacar

    private GameObject player; // Referência ao jogador
    private Rigidbody2D rb; // Referência ao Rigidbody2D do inimigo
    private int currentHealth; // Vida atual do inimigo
    private bool isGrounded; // Verifica se o inimigo está no chão
    public bool IsAtacking = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Encontrar o jogador pela tag
        rb = GetComponent<Rigidbody2D>(); // Obter o componente Rigidbody2D
        currentHealth = maxHealth; // Definir a vida inicial do inimigo
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, LayerMask.GetMask("Ground"));

        if (player != null && IsPlayerVisible())
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= attackRange && !IsAtacking)
            {
                StartCoroutine(AttackPlayerCoroutine()); // Começa a corrotina para atacar o jogador
            }
            else if (distanceToPlayer <= followRange)
            {
                FollowPlayer();
            }
            else
            {
                StopMovement(); // Para o movimento quando o jogador estiver fora do alcance
            }
        }
        else
        {
            StopMovement(); // Para o movimento se o jogador não estiver visível
        }

        // Adiciona um comportamento para parar o movimento se não estiver no chão
        if (!isGrounded)
        {
            StopMovement();
        }
    }

    void FollowPlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized; // Calcula a direção para o jogador
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y); // Define a velocidade do Rigidbody2D para seguir o jogador, apenas no eixo X
    }

    void StopMovement()
    {
        rb.velocity = new Vector2(0, rb.velocity.y); // Para o movimento do inimigo no eixo X
    }

    IEnumerator AttackPlayerCoroutine()
    {
        IsAtacking = true;
        yield return new WaitForSeconds(attackDelay);
        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(1); // Certifique-se de que o método TakeDamage no script do jogador aplica dano
        }
        yield return new WaitForSeconds(1f);
        IsAtacking = false;
    }

    bool IsPlayerVisible()
    {
        return (player.gameObject.layer == Mathf.Log(playerLayer.value, 2)); // Verifica se o jogador está na Layer correta
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("BuletPlayer"))
        {
            TakeDamage(GameManager.Instance.playerDamage);
            Destroy(col.gameObject);
        }
    }

    void Die()
    {
        Destroy(gameObject); // Destroi o inimigo
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
