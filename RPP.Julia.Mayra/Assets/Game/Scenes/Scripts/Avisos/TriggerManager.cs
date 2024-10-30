using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;  // Necessário para trabalhar com o UI Text

public class TriggerManager : MonoBehaviour
{
    public Text triggerText;  // Referência ao componente Text no Canvas

    private void OnEnable()
    {
        TriggerObserver.OnTriggerEnterEvent += HandleTriggerEnter;
    }

    private void OnDisable()
    {
        TriggerObserver.OnTriggerEnterEvent -= HandleTriggerEnter;
    }

    private void HandleTriggerEnter(string triggerName)
    {
        switch (triggerName)
        {
            case "Pule":
                triggerText.text = "Pule Para desviar de obstaculos";
                break;

            case "Enemy":
                triggerText.text = "há um inimigo a frente";
                break;
            case "PlataformFall":
                triggerText.text = "Essa Plataforma cai! seja rapido!";
                break;
            case "MovePlataform":
                triggerText.text = "Cuidado Com essa Plataforma que se move";
                break;

            default:
                triggerText.text = "Jogador entrou em um trigger desconhecido: " + triggerName;
                break;
        }
        StartCoroutine(ClearTextAfterDelay(3f));
    }

    private IEnumerator ClearTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        triggerText.text = "";
    }
}