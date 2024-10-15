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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
    }

    void Update()
    {
        JumpAndRun();
        HandleAttack();
    }

    void JumpAndRun()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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
        if (movement != 0)
        {
            animator.SetInteger("Transitioon", 1); // Correndo
        }
        else
        {
            animator.SetInteger("Transitioon", 0); // Parado
        }
        
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetInteger("Transitioon", 2); // Pulando
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.F) && canShoot ) 
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        if (GameManager.Instance.Estouinvisivel == false)
        {
            canShoot = false; 
            animator.SetTrigger("Shoot"); 
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            Destroy(col.gameObject);
            GameManager.Instance.LifePlayer--;
        }

        if (col.gameObject.CompareTag("InvisiblePotion"))
        {
            if (!GameManager.Instance.hasInvisiblePotion)
            {
                GameManager.Instance.hasInvisiblePotion = true;
                Destroy(col.gameObject);
            }
        }

        if (col.gameObject.CompareTag("DamageIncrease"))
        {
            GameManager.Instance.playerDamage++;
            Destroy(col.gameObject);
        }

        if (col.gameObject.CompareTag("HealthPickup"))
        {
            GameManager.Instance.LifePlayer++;
            Destroy(col.gameObject);
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
