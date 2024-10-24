using System.Collections;
using UnityEngine;

public class EnemyShooterStatic : MonoBehaviour
{
    public float shootRange = 7f; // Distância em que o inimigo começa a atirar
    public float fireRate = 1f; // Taxa de tiro (em segundos)
    public GameObject bulletPrefab; // Prefab do projétil
    public Transform firePoint; // Ponto de origem do projétil
    public LayerMask playerLayer; // Layer do jogador
    private int maxHealth = 5; // Vida máxima do inimigo

    private Transform player;
    private float nextFireTime = 0f;
    private int currentHealth;

    // Audio
    public AudioClip shootSound; // Som do tiro
    public AudioClip takeDamageSound; // Som ao tomar dano
    private AudioSource audioSource; // Fonte de áudio

    // Animator
    public Animator animator;
    private bool isAttacking; // Verifica se o inimigo está atacando

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        // Inicializa o estado de ataque
        isAttacking = false;
    }

    void Update()
    {
        if (player != null && IsPlayerVisible())
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= shootRange && Time.time >= nextFireTime)
            {
                StartCoroutine(Attack());
                nextFireTime = Time.time + 1.5f / fireRate; // Atualiza o próximo tempo de ataque
            }
        }

        // Atualiza o parâmetro de animação
        animator.SetBool("isAttacking", isAttacking);
    }

    private IEnumerator Attack()
    {
        isAttacking = true; // Marca que o inimigo está atacando

        // Toca o som de tiro
        audioSource.PlayOneShot(shootSound);

        // Executa o ataque
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;
            rbBullet.velocity = direction * 10f; // Define a velocidade do projétil
        }

        // Aguarda um breve período antes de finalizar o ataque
        yield return new WaitForSeconds(0.5f); // Duração da animação de ataque (ajuste conforme necessário)

        isAttacking = false; // Marca que o inimigo não está mais atacando
    }

    bool IsPlayerVisible()
    {
        return (player.gameObject.layer == Mathf.Log(playerLayer.value, 2));
    }

    public void TakeDamage()
    {
        currentHealth -= GameManager.Instance.playerDamage;

        // Toca som de dano
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
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
