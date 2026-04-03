using UnityEngine;

public class PlayerPowerUp : MonoBehaviour
{

    [SerializeField]
    private float jumpForceAdditional = 5f; // Lực nhảy thêm khi bự
    [SerializeField]
    private GameObject PlayerSmall;
    [SerializeField]
    private GameObject PlayerBig;
    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerHealth     = GetComponentInChildren<PlayerHealth>(true);
        Debug.Log($"[PlayerPowerUp] playerHealth = {(playerHealth == null ? "NULL !!!" : playerHealth.ToString())}");
        // Nếu Find không ra thì lấy từ inspector
        if (PlayerSmall == null) PlayerSmall = transform.Find("PlayerSmall").gameObject;
        if (PlayerBig == null) PlayerBig = transform.Find("PlayerBig").gameObject;

        // Bắt đầu game với hình dạng nhỏ
        SetPlayerState(isBig: false);
    }

    // Biến trạng thái
    public bool isPowerUpBig { get; private set; } = false;

    // Hàm gọi khi ăn nấm
    public void GrowBig()
    {
        if (!isPowerUpBig)
        {
            playerController.JumpForce += jumpForceAdditional; // Tăng lực nhảy khi to lên
            SetPlayerState(isBig: true);
            // Thêm hạt (Particle) hoặc Animation khựng lại/đứng hình giống Mario ở đây
        }
    }

    // Hàm gọi khi bị quái chạm vào lúc đang bự
    public void ShrinkSmall()
    {
        if (isPowerUpBig)
        {
            playerController.JumpForce -= jumpForceAdditional; // Trừ đi lực nhảy khi bé lại
            SetPlayerState(isBig: false);
            // Thêm hiệu ứng nhấp nháy bất tử tạm thời (Invulnerability frames) ở đây
        }
    }

    // Xử lý bật/tắt đúng model nhân vật
    private void SetPlayerState(bool isBig)
    {
        isPowerUpBig = isBig;
        if (PlayerSmall != null) PlayerSmall.SetActive(!isBig);
        if (PlayerBig != null) PlayerBig.SetActive(isBig);

        // Sau khi đổi model, cập nhật lại Animator cho PlayerController
        // vì Animator nằm trong child object nên phải lấy lại từ model đang active
        GameObject activeModel = isBig ? PlayerBig : PlayerSmall;
        if (activeModel != null)
        {
            Animator newAnimator = activeModel.GetComponentInChildren<Animator>();
            playerController.SetAnimator(newAnimator);
            playerHealth?.SetAnimator(newAnimator); // đồng bộ animator cho PlayerHealth
        }

       
    }

    // Xử lý va chạm vật lý
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi ăn nấm
        if (collision.CompareTag("PointItem"))
        {
            GrowBig();
            Destroy(collision.gameObject);
            return;
        }

        // Khi chạm địch
        if (collision.CompareTag("Enemy"))
        {
            // ── Kiểm tra stomp: player đang rơi + chạm đỉnh enemy ────────────
            Rigidbody2D rb      = GetComponent<Rigidbody2D>();
            Collider2D playerCol = GetComponent<Collider2D>();

            if (rb != null && playerCol != null && rb.linearVelocity.y < 0f
                && playerCol.bounds.min.y >= collision.bounds.center.y)
            {
                // ── STOMP: giết enemy + nảy lên ──────────────────────────────
                EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.Stomp();
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x,
                        playerController.JumpForce * 0.7f);   // nảy lên ~70% jump
                    Debug.Log($"[PlayerPowerUp] Stomp '{collision.name}' → chết + nảy lên!");
                }
                return; // không nhận damage
            }

            // ── Va chạm bình thường từ hướng khác ────────────────────────────
            if (isPowerUpBig)
            {
                ShrinkSmall();
            }
            else
            {
                // Trạng thái thường bị enemy chạm → chết
                // Sự kiện die do PlayerHealth quản lý
                Debug.Log("[PlayerPowerUp] Player trạng thái thường bị chạm → gọi Die!");
                playerHealth?.Die();
            }
        }
    }
}
