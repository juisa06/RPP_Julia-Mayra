using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;  // Ponto A
    public Transform pointB;  // Ponto B
    public float speed = 2f;  // Velocidade da plataforma

    private Vector3 targetPosition;  // Posição alvo atual

    void Start()
    {
        // Começa movendo para o ponto B
        targetPosition = pointB.position;
    }

    void FixedUpdate()
    {
        // Move a plataforma em direção ao ponto alvo
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Verifica se chegou ao ponto alvo
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Alterna entre o ponto A e B
            if (targetPosition == pointA.position)
            {
                targetPosition = pointB.position;
            }
            else
            {
                targetPosition = pointA.position;
            }
        }
    }

    // Para visualizar os pontos no editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(pointA.position, 0.2f);
        Gizmos.DrawSphere(pointB.position, 0.2f);
        Gizmos.DrawLine(pointA.position, pointB.position);
    }
}