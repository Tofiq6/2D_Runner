using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public const int platformPoolSize = 3;

    [SerializeField] private Transform GridTransform;
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private int startingActiveCount = 2;
    [SerializeField] private float spacing = 33f;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private Queue<GameObject> activeQueue = new Queue<GameObject>();
    private float currentEndX;
    private bool spawningInProgress;

    void Start()
    {
        if (platformPrefab == null)
        {
            Debug.LogError("PlatformSpawner: platformprefab atanmadý.");
            return;
        }

        // Havuzu oluþtur
        for (int i = 0; i < platformPoolSize; i++)
        {
            GameObject obj = Instantiate(platformPrefab, GridTransform);
            obj.SetActive(false);

            var trigger = obj.GetComponentInChildren<PlatformTrigger>();
            if (trigger == null)
            {
                trigger = obj.AddPlatformInChildrenIfMissing<PlatformTrigger>();
            }

            if (trigger != null)
            {
                trigger.Spawner = this;
                trigger.PlatformRoot = obj;
            }

            pool.Enqueue(obj);
        }

        Vector3 startPos = transform.position;
        for (int i = 0; i < startingActiveCount; i++)
        {
            if (pool.Count == 0) break;
            GameObject p = pool.Dequeue();
            p.SetActive(true);
            Vector3 pos = new Vector3(startPos.x + spacing * i, startPos.y, startPos.z);
            p.transform.position = pos;
            activeQueue.Enqueue(p);
        }

        if (activeQueue.Count > 0)
        {
            GameObject[] arr = activeQueue.ToArray();
            currentEndX = arr[arr.Length - 1].transform.position.x;
        }
    }

    public void OnPlayerTriggered(GameObject platformThatTriggered)
    {
        if (spawningInProgress) return;
        spawningInProgress = true;

        if (activeQueue.Count == 0)
        {
            spawningInProgress = false;
            return;
        }

        GameObject oldest = activeQueue.Dequeue();
        oldest.SetActive(false);

        float nextX = currentEndX + spacing;
        Vector3 newPos = new Vector3(nextX, transform.position.y, transform.position.z);
        oldest.transform.position = newPos;
        oldest.SetActive(true);

        activeQueue.Enqueue(oldest);
        currentEndX = nextX;

        spawningInProgress = false;
    }
}


static class PlatformSpawnerExtensions
{
    public static T AddPlatformInChildrenIfMissing<T>(this GameObject go) where T : Component
    {
        T found = go.GetComponentInChildren<T>(true);
        if (found != null) return found;

        return go.AddComponent<T>();
    }
}
