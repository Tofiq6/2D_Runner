using UnityEngine;


public class PlatformTrigger : MonoBehaviour
{
    [HideInInspector] public PlatformSpawner Spawner;
    [HideInInspector] public GameObject PlatformRoot;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Spawner == null)
        {
            Debug.LogWarning("PlatformTrigger: Spawner referansý yok.");
            return;
        }

        Spawner.OnPlayerTriggered(PlatformRoot);
    }
}