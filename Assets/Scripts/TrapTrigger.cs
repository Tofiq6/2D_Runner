using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [HideInInspector] public TrapSpawner Spawner;
    [HideInInspector] public GameObject TrapRoot;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Spawner == null)
        {
            Debug.LogWarning("TrapTrigger: Spawner referansý yok.");
            return;
        }

        Spawner.OnPlayerTriggered(TrapRoot);
    }
}
