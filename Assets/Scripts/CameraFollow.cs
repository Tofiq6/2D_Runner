using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                 // Takip edilecek oyuncu

    [Header("Smoothing")]
    public float smoothTime = 0.15f;         // Küçük deðer = daha sert takip, büyük = daha yumuþak
    private Vector3 velocity = Vector3.zero;

    [Header("Offset & Depth")]
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Kamera ile oyuncu arasý offset (z genelde -10)
    public bool maintainZ = true;            // true ise offset.z korunur

    [Header("Deadzone (opsiyonel)")]
    public bool useDeadzone = false;
    public Vector2 deadzoneSize = new Vector2(0.5f, 0.5f); // X ve Y ekseninde yarý-geniþlik

    [Header("Bounds (opsiyonel)")]
    public bool useBounds = false;
    public Vector2 minBounds;                // Sol-alt köþe world koordinatý
    public Vector2 maxBounds;                // Sað-üst köþe world koordinatý

    void FixedUpdate()
    {
        if (target == null) return;

        // Hedef pozisyonu (offset eklenmiþ)
        Vector3 targetPos = target.position + offset;

        if (maintainZ)
        {
            // Kamera Z'sini sabit tut (ör. -10)
            targetPos.z = offset.z;
        }

        // Deadzone kontrolü: eðer hedef deadzone içinde ise hedef pozisyonu = mevcut kamera pozisyonu
        if (useDeadzone)
        {
            Vector3 camToTarget = target.position - transform.position;
            // Deadzone candidate: eðer hem X hem Y ekseninde deadzone içinde ise kamera hareket etmesin.
            if (Mathf.Abs(camToTarget.x) <= deadzoneSize.x && Mathf.Abs(camToTarget.y) <= deadzoneSize.y)
            {
                // hedef pozisyonu olarak kamera pozisyonunu kullan -> no movement
                targetPos = transform.position;
            }
        }

        // Smooth takip
        Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // Bounds uygulama
        if (useBounds)
        {
            float camZ = newPos.z;
            // Kamera ortalanmýþ olduðunu varsayýyoruz; orthographic size hesaplamasý yapalým.
            Camera cam = GetComponent<Camera>();
            if (cam != null && cam.orthographic)
            {
                float vertExtent = cam.orthographicSize;
                float horzExtent = vertExtent * Screen.width / (float)Screen.height;

                // Min/max x ve y deðerlerini hesapla (kamera view kenarlarýnýn clamp edilmesi)
                float minX = minBounds.x + horzExtent;
                float maxX = maxBounds.x - horzExtent;
                float minY = minBounds.y + vertExtent;
                float maxY = maxBounds.y - vertExtent;

                newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
                newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
            }
            else
            {
                // Perspective ya da orthographic olmayan durumlarda doðrudan clamp et
                newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
                newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);
            }

            newPos.z = camZ; // z'yi koru
        }

        transform.position = newPos;
    }

    // Hýzlý görsel yardýmcý (Inspector'da deadzone'u göster)
    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Gizmos.color = Color.cyan;
        Vector3 center = target.position;
        if (useDeadzone)
        {
            Gizmos.DrawWireCube(center, new Vector3(deadzoneSize.x * 2f, deadzoneSize.y * 2f, 0.01f));
        }

        if (useBounds)
        {
            Gizmos.color = Color.yellow;
            Vector3 min = new Vector3(minBounds.x, minBounds.y, 0f);
            Vector3 max = new Vector3(maxBounds.x, maxBounds.y, 0f);
            Vector3 size = max - min;
            Gizmos.DrawWireCube(min + size / 2f, new Vector3(size.x, size.y, 0.01f));
        }
    }
}
