using System.Collections.Generic;
using UnityEngine;

public class AppleSpawner : MonoBehaviour
{
    public const int ApplePoolSize = 20;

    [SerializeField] private Transform AppleTransform;
    [SerializeField] private GameObject ApplePrefab;
    [SerializeField] private int startingActiveCount = 20;
    [SerializeField] private float spacing;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private Queue<GameObject> activeQueue = new Queue<GameObject>();
    private float currentEndX;
    private bool spawningInProgress;

    void Start()
    {
        if (ApplePrefab == null)
        {
            Debug.LogError("AppleSpawner: applePrefab atanmadý.");
            return;
        }

        // Havuzu oluþtur
        for (int i = 0; i < ApplePoolSize; i++)
        {
            GameObject obj = Instantiate(ApplePrefab, AppleTransform);
            obj.SetActive(false);

            pool.Enqueue(obj);
        }

        // Baþlangýçta 'startingActiveCount' kadar apple'i aktif et ve yerleþtir
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

        // currentEndX en son aktif apple'in x konumu olur
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

    public void OnPlayerTriggered(GameObject appleThatTriggered)
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
        Vector3 newPos = new Vector3(nextX, transform.position.y, transform.position.z);
        first.transform.position = newPos;

        // Tekrar aktif et ve kuyruðun sonuna ekle
        first.SetActive(true);
        activeQueue.Enqueue(first);

        // currentEndX'i güncelle
        currentEndX = nextX;

        spawningInProgress = false;
    }

}

