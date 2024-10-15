using UnityEngine;

public class InvisibilityPotion : MonoBehaviour
{
    public float invisibilityDuration = 5f;
    public float cooldownTime = 15f;
    public  bool isInvisible = false;
    private bool onCooldown = false;

    private Renderer playerRenderer;
    private Color originalColor; 

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
        originalColor = playerRenderer.material.color; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !onCooldown && GameManager.Instance.hasInvisiblePotion == true)
        {
            ActivateInvisibility();
            GameManager.Instance.hasInvisiblePotion = false;
        }
    }

    void ActivateInvisibility()
    {
        GameManager.Instance.Estouinvisivel = true;
        isInvisible = true;
        onCooldown = true;
        Color transparentColor = originalColor;
        transparentColor.a = 0.3f;
        playerRenderer.material.color = transparentColor;
        gameObject.layer = LayerMask.NameToLayer("Invisible");
        Invoke("DeactivateInvisibility", invisibilityDuration);
        Invoke("ResetCooldown", cooldownTime);
    }

    void DeactivateInvisibility()
    {
        GameManager.Instance.Estouinvisivel = false;
        isInvisible = false;
        playerRenderer.material.color = originalColor;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    void ResetCooldown()
    {
        onCooldown = false;
    }
}
