using UnityEngine;

/// <summary>
/// Gắn vào GameObject Checkpoint trong scene.
/// - Khi player đi vào trigger: activate tất cả children, báo CheckpointManager.
/// - Children nên được để inactive ban đầu (cờ, hiệu ứng, v.v.)
/// </summary>
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Nếu để trống, dùng vị trí của chính Checkpoint này làm điểm respawn.")]
    [SerializeField] private Transform respawnPoint;

    // Đã kích hoạt chưa (tránh trigger nhiều lần)
    private bool isActivated = false;

    private void Start()
    {
        // Tắt hết children lúc đầu (chỉ bật khi player đi qua)
        SetChildrenActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;
        if (!other.CompareTag("Player")) return;

        isActivated = true;

        // Hiện toàn bộ children (cờ, hiệu ứng, ...)
        SetChildrenActive(true);

        // Lưu vị trí respawn vào CheckpointManager
        Vector3 spawnPos = respawnPoint != null ? respawnPoint.position : transform.position;
        CheckpointManager.Instance?.SetCheckpoint(spawnPos);

        Debug.Log($"[Checkpoint] Activated at {spawnPos}");
    }

    // ─── Helper ──────────────────────────────────────────────────────────────

    private void SetChildrenActive(bool active)
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(active);
    }
}
