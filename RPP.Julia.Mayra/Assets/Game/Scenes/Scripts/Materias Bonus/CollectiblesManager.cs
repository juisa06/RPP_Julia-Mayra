using UnityEngine;
using UnityEngine.UI;

public class CollectibleManager : MonoBehaviour
{
    // Referências aos objetos Canvas
    public Image[] rareStonesImages;
    public Image[] puzzlePiecesImages;
    public Image[] rareAnimalsImages;

    // Contadores dos itens coletados
    private int[] collectedRareStones = new int[6];
    private int[] collectedPuzzlePieces = new int[6];
    private int[] collectedRareAnimals = new int[6];

    private void Start()
    {
        // Inicializa os contadores e esconde as imagens
        InitializeImages(rareStonesImages);
        InitializeImages(puzzlePiecesImages);
        InitializeImages(rareAnimalsImages);
    }

    private void InitializeImages(Image[] images)
    {
        foreach (Image image in images)
        {
            image.gameObject.SetActive(false);
        }
    }

    public void CollectRareStone(int index)
    {
        if (IsIndexValid(index, collectedRareStones.Length))
        {
            collectedRareStones[index]++;
            CheckUnlock(rareStonesImages, collectedRareStones);
        }
    }

    public void CollectPuzzlePiece(int index)
    {
        if (IsIndexValid(index, collectedPuzzlePieces.Length))
        {
            collectedPuzzlePieces[index]++;
            CheckUnlock(puzzlePiecesImages, collectedPuzzlePieces);
        }
    }

    public void CollectRareAnimal(int index)
    {
        if (IsIndexValid(index, collectedRareAnimals.Length))
        {
            collectedRareAnimals[index]++;
            CheckUnlock(rareAnimalsImages, collectedRareAnimals);
        }
    }

    private bool IsIndexValid(int index, int length)
    {
        return index >= 0 && index < length;
    }

    private void CheckUnlock(Image[] images, int[] collectedItems)
    {
        for (int i = 0; i < collectedItems.Length; i++)
        {
            if (collectedItems[i] > 0)
            {
                images[i].gameObject.SetActive(true);
            }
        }
    }
}