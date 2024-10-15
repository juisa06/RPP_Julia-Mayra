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
    private Rigidbody2D rb;
    private float nextFireTime = 0f;
    private int currentHealth;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player != null && IsPlayerVisible())
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= shootRange && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1.5f / fireRate;
            }
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;
            rbBullet.velocity = direction * 10f;
        }
    }

    bool IsPlayerVisible()
    {
        return (player.gameObject.layer == Mathf.Log(playerLayer.value, 2));
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
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
