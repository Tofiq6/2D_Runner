using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // SAHNE YENİDEN YÜKLEMEK İÇİN

public class CharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpForce = 1000.0f;
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private bool onGround;

    [Header("Ground Check")]
    [Tooltip("Karakterin yere temasını kontrol etmek için ayak noktası (bir child Transform).")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Components")]
    public Animator animator;
    public AppleSpawner appleSpawner;

    [Header("GamePlay Elements")]
    public int coinCount = 0;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI finalCoinText;

    [Header("UI Buttons")]
    public Button jumpButton;
    public Button rollButton;

    [Header("Death Screen UI")]
    public GameObject deathPanel;     // Öldün ekranı
    public Button restartButton;      // Yeniden başla butonu

    private Rigidbody2D rb;
    private const float verticalThreshold = 0.05f; // titreşimi önlemek için küçük eşik
    private bool isDead = false; // Ölüm durumunu takip etmek için

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Oyun yeniden başladığında zaman ölçeklemesi sıfırlanmış olsun
        Time.timeScale = 1f;

        if (jumpButton != null)
            jumpButton.onClick.AddListener(JumpButton);
        else
            Debug.LogWarning("CharacterController: jumpButton atanmadı.");

        if (rollButton != null)
            rollButton.onClick.AddListener(RollButton);

        if (groundCheck == null)
            Debug.LogWarning("CharacterController: groundCheck atanmadı. Ground kontrolü çalışmayacaktır.");

        // Death panel başta kapalı olsun (sahnede unutursan diye garantiye al)
        if (deathPanel != null)
            deathPanel.SetActive(false);

        // Restart butonunu bağla
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        else
            Debug.LogWarning("CharacterController: restartButton atanmadı.");
    }

    private void Update()
    {
        if (isDead)
            return; // Öldüysek hareket etme

        // Yatay sabit hareket
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);

        // Yer kontrolü
        if (groundCheck != null)
        {
            Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            onGround = hit != null;
        }

        // Animator isFall kontrolü: yukarı çıkıyorsa false, aşağı iniyorsa true
        if (animator != null)
        {
            if (onGround)
            {
                animator.SetBool("isFall", false);
            }
            else
            {
                float vy = rb.linearVelocity.y;
                if (vy > verticalThreshold)
                    animator.SetBool("isFall", false);
                else if (vy < -verticalThreshold)
                    animator.SetBool("isFall", true);
                // eşiğin içindeyse mevcut değeri koru
            }
        }
    }

    public void JumpButton()
    {
        if (!onGround || isDead) return;

        rb.AddForce(Vector2.up * jumpForce);

        if (animator != null)
        {
            animator.SetTrigger("Jump");
            animator.SetBool("isFall", false);
        }
    }

    public void RollButton()
    {
        if (isDead) return;

        if (animator != null)
            animator.SetTrigger("Roll");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            coinCount++;
            if (coinText != null)
                coinText.text = coinCount.ToString();

            PlayerPrefs.SetInt("CoinCount", coinCount);

            if (appleSpawner != null)
                appleSpawner.OnPlayerTriggered(other.gameObject);
        }

        if (other.CompareTag("Trap"))
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Hareketi durdur
        rb.linearVelocity = Vector2.zero;

        // Ölüm animasyonu varsa tetikle
        if (animator != null)
        {
            animator.SetBool("isFall", false);
            animator.SetTrigger("Die"); // Animator'da "Die" trigger'ın yoksa sil
        }

        // Toplanan coin sayısını göster
        int toplananCoin = coinCount; // İstersen PlayerPrefs'ten de alabilirsin
        if (finalCoinText != null)
            finalCoinText.text = toplananCoin.ToString();

        // Ölüm panelini aç
        if (deathPanel != null)
            deathPanel.SetActive(true);

        // Oyunu durdur
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        // Zamanı tekrar normale al
        Time.timeScale = 1f;

        // Aktif sahneyi yeniden yükle
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
