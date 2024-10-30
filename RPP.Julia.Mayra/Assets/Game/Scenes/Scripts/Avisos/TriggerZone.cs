using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public string triggerName;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            TriggerObserver.TriggerEnter(triggerName);
            Destroy(GetComponent<BoxCollider2D>());
        }
    }
}