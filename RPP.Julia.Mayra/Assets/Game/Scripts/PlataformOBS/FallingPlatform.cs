using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    private Vector3 originalPosition; // Armazena a posição original da plataforma
    public float fallSpeed = 50f; // Velocidade da queda
    public float delayBeforeFall = 1.5f; // Tempo de atraso antes de começar a cair
    public float resetTime = 2f; // Tempo para resetar a plataforma
    public float returnSpeed = 5f; // Velocidade para retornar à posição original

    private bool isFalling = false;

    private void Start()
    {
        // Armazena a posição original da plataforma
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (isFalling)
        {
            // Move a plataforma para baixo
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
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

        // Aguarda o tempo de reset antes de começar a retornar
        yield return new WaitForSeconds(resetTime);
        StartCoroutine(ResetPlatform());
    }

    private IEnumerator ResetPlatform()
    {
        isFalling = false;

        // Move a plataforma de volta à posição original suavemente
        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, returnSpeed * Time.deltaTime);
            yield return null;
        }

        // Garante que a posição final seja exatamente a posição original
        transform.position = originalPosition;
    }
}