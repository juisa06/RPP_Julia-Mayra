using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    public Transform originalPosition; // A posição original da plataforma
    public float fallSpeed = 50f; // Velocidade da queda (alta velocidade para cair rapidamente)
    public float delayBeforeFall = 1.5f; // Tempo de atraso antes de começar a cair
    public float resetTime = 2f; // Tempo para resetar a plataforma

    private bool isFalling = false;
    private float fallStartTime;

    private void Start()
    {
        // Armazena a posição original da plataforma
        if (originalPosition == null)
        {
            originalPosition = transform;
        }
    }

    private void Update()
    {
        if (isFalling)
        {
            // Move a plataforma para baixo
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

            // Verifica se a plataforma chegou ao fundo (você pode definir uma altura mínima)
            if (Time.time - fallStartTime >= resetTime)
            {
                StartCoroutine(ResetPlatform());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !isFalling)
        {
            // Começa o delay antes da queda
            StartCoroutine(FallAfterDelay());
        }
    }

    private IEnumerator FallAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeFall);
        isFalling = true;
        fallStartTime = Time.time;
    }

    private IEnumerator ResetPlatform()
    {
        yield return new WaitForSeconds(resetTime);
        // Move a plataforma de volta à posição original
        while (Vector3.Distance(transform.position, originalPosition.position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition.position, Time.deltaTime * fallSpeed);
            yield return null;
        }
        transform.position = originalPosition.position; // Garante que a plataforma esteja exatamente na posição original
        isFalling = false;
    }
}
