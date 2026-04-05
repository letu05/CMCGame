using UnityEngine;

/// <summary>
/// Singleton tồn tại trong scene — lưu vị trí checkpoint gần nhất.
/// Được reset mỗi lần scene load lại (không DontDestroyOnLoad).
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    // Vị trí spawn mặc định (gán trong Inspector = vị trí spawn đầu level)
    [SerializeField] private Transform defaultSpawnPoint;

    // Vị trí checkpoint đang active
    private Vector3 currentRespawnPos;
    private bool    hasCheckpoint = false;

    // Tham chiếu tới player để dịch chuyển khi respawn
    private Transform   playerTransform;
    private Rigidbody2D playerRb;

    private void Awake()
    {
        Instance = this;

        // Mặc định respawn ở điểm bắt đầu level
        if (defaultSpawnPoint != null)
            currentRespawnPos = defaultSpawnPoint.position;
    }

    // ─── Đăng ký Player ─────────────────────────────────────────────────────

    /// <summary>Gọi từ PlayerController.Start() để đăng ký tham chiếu.</summary>
    public void RegisterPlayer(Transform player, Rigidbody2D rb)
    {
        playerTransform = player;
        playerRb        = rb;

        // Nếu chưa có checkpoint nào → spawn pos = vị trí ban đầu của player
        if (!hasCheckpoint && defaultSpawnPoint == null)
            currentRespawnPos = player.position;
    }

    // ─── Cập nhật Checkpoint ────────────────────────────────────────────────

    /// <summary>Gọi từ Checkpoint.cs khi player đi qua.</summary>
    public void SetCheckpoint(Vector3 position)
    {
        currentRespawnPos = position;
        hasCheckpoint     = true;
    }

    // ─── Respawn ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Dịch chuyển player về checkpoint. Gọi từ GameManager.PlayerDied() khi còn mạng.
    /// </summary>
    public void RespawnPlayer()
    {
        if (playerTransform == null) return;

        // Dừng vận tốc để tránh bay vọt
        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;

        playerTransform.position = currentRespawnPos;

        // Reset animator về idle — xoá Die trigger/state
        ResetAnimator();

        // Mở khoá di chuyển
        PlayerController ctrl = playerTransform.GetComponent<PlayerController>();
        if (ctrl != null) ctrl.ClearDead();
    }

    // ─── Reset Animator ──────────────────────────────────────────────────────

    private void ResetAnimator()
    {
        if (playerTransform == null) return;

        // Ưu tiên dùng PlayerPowerUp để sync đúng model đang active
        PlayerPowerUp powerUp = playerTransform.GetComponent<PlayerPowerUp>();
        if (powerUp != null)
        {
            powerUp.ResetAfterDeath();
            return;
        }

        // Fallback: tìm Animator bất kỳ trên player và Rebind về idle
        Animator anim = playerTransform.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }
    }

    public Vector3 GetRespawnPos() => currentRespawnPos;
}
