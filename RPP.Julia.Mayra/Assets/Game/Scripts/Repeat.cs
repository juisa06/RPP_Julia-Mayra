using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeat : MonoBehaviour
{
    public Camera mainCamera; // Referência à câmera principal.
    private float backgroundWidth; // A largura do background.

    private Vector3 lastCameraPosition; // Posição da câmera no último frame.

    [SerializeField] private float parallaxEffect = 1f; // Intensidade do movimento do background (efeito de parallax).

    private void Start()
    {
        // Calcula a largura do background considerando o tamanho da sprite e a escala do transform.
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        backgroundWidth = spriteRenderer.bounds.size.x; // Tamanho real do background considerando o SpriteRenderer.

        // Inicializa a última posição da câmera.
        lastCameraPosition = mainCamera.transform.position;
    }

    private void Update()
    {
        // Calcula a diferença de movimento da câmera em relação ao último frame.
        float cameraMovement = mainCamera.transform.position.x - lastCameraPosition.x;

        // Aplica o efeito de parallax (movimento inverso).
        transform.position -= new Vector3(cameraMovement * parallaxEffect, 0, 0);

        // Atualiza a última posição da câmera para o próximo frame.
        lastCameraPosition = mainCamera.transform.position;

        // Verifica se o background saiu completamente da tela e reposiciona.
        if (transform.position.x > mainCamera.transform.position.x + backgroundWidth)
        {
            RepositionToLeft();
        }
        else if (transform.position.x < mainCamera.transform.position.x - backgroundWidth)
        {
            RepositionToRight();
        }
    }

    private void RepositionToLeft()
    {
        // Reposiciona o background à frente, para a esquerda, quando ele ultrapassa a câmera à direita.
        Vector2 offset = new Vector2(backgroundWidth, 0);  // Coloca o background diretamente no fim da próxima imagem.
        transform.position = (Vector2)transform.position + offset;
    }

    private void RepositionToRight()
    {
        // Reposiciona o background à frente, para a direita, quando ele ultrapassa a câmera à esquerda.
        Vector2 offset = new Vector2(-backgroundWidth, 0);  // Coloca o background diretamente no fim da próxima imagem.
        transform.position = (Vector2)transform.position + offset;
    }
}