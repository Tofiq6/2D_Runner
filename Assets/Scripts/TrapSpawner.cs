using System.Collections.Generic;
using UnityEngine;

public class TrapSpawner : MonoBehaviour
{
    public const int TrapPoolSize = 5;

    [SerializeField] private Transform TrapTransform;
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private int startingActiveCount = 5;
    [SerializeField] private float spacing = 10f;
    [SerializeField] private float rollHeight = 2f;
    [SerializeField] private float jumpHeight = 5f;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private Queue<GameObject> activeQueue = new Queue<GameObject>();
    private float currentEndX;
    private bool spawningInProgress;

    void Start()
    {
        if (trapPrefab == null)
        {
            Debug.LogError("TrapSpawner: trapPrefab atanmadý.");
            return;
        }

        // Havuzu oluþtur
        for (int i = 0; i < TrapPoolSize; i++)
        {
            GameObject obj = Instantiate(trapPrefab, TrapTransform);
            obj.SetActive(false);

            var trigger = obj.GetComponentInChildren<TrapTrigger>();
            if (trigger == null)
            {
                trigger = obj.AddTrapInChildrenIfMissing<TrapTrigger>();
            }

            if (trigger != null)
            {
                trigger.Spawner = this;
                trigger.TrapRoot = obj;
            }

            pool.Enqueue(obj);
        }

        // Baþlangýçta 'startingActiveCount' kadar trap'i aktif et ve yerleþtir
        Vector3 startPos = transform.position;
        for (int i = 0; i < startingActiveCount; i++)
        {
            if (pool.Count == 0) break;
            GameObject p = pool.Dequeue();
            p.SetActive(true);

            float randY = GetRandomYRelativeTo(startPos.y);
            Vector3 pos = new Vector3(startPos.x + spacing * i, randY, startPos.z);
            p.transform.position = pos;

            activeQueue.Enqueue(p);
        }

        // currentEndX en son aktif trap'in x konumu olur
        if (activeQueue.Count > 0)
        {
            GameObject[] arr = activeQueue.ToArray();
            currentEndX = arr[arr.Length - 1].transform.position.x;
        }
        else
        {
            currentEndX = transform.position.x;
        }
    }

    // Trap içindeki trigger tarafýndan çaðrýlýr
    public void OnPlayerTriggered(GameObject trapThatTriggered)
    {
        if (spawningInProgress) return;
        spawningInProgress = true;

        if (activeQueue.Count == 0)
        {
            spawningInProgress = false;
            return;
        }

        // Kuyruðun baþýndaki (ilk) objeyi çýkar, pasifleþtir, en sona taþý ve tekrar aktif et
        GameObject first = activeQueue.Dequeue();

        // isteðe baðlý olarak önce pasifleþtir (state reset için)
        first.SetActive(false);

        // Yeni pozisyon: currentEndX + spacing
        float nextX = currentEndX + spacing;
        float randY = GetRandomYRelativeTo(transform.position.y);
        Vector3 newPos = new Vector3(nextX, randY, transform.position.z);
        first.transform.position = newPos;

        // Tekrar aktif et ve kuyruðun sonuna ekle
        first.SetActive(true);
        activeQueue.Enqueue(first);

        // currentEndX'i güncelle
        currentEndX = nextX;

        spawningInProgress = false;
    }

    // artýk randY ya rollHeight ya da jumpHeight olacak þekilde seçiliyor
    private float GetRandomYRelativeTo(float baseY)
    {
        float chosenOffset = (Random.value < 0.5f) ? rollHeight : jumpHeight;
        return baseY + chosenOffset;
    }
}

static class TrapSpawnerExtensions
{
    public static T AddTrapInChildrenIfMissing<T>(this GameObject go) where T : Component
    {
        T found = go.GetComponentInChildren<T>(true);
        if (found != null) return found;
        return go.AddComponent<T>();
    }
}
