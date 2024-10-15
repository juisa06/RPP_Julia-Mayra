using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // Velocidade do projétil

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = transform.right * speed; // Define a velocidade do projétil
        }
    }
}