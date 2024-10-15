using UnityEngine;

public class Spike : MonoBehaviour
{
    public float pushForce = 15f; // Força do impulso aplicado ao jogador

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o objeto que colidiu é o jogador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obtém o Rigidbody2D do jogador
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                GameManager.Instance.LifePlayer--;
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                playerRb.velocity = Vector2.zero;
                playerRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            }
        }
    }
}