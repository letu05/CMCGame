using System.Collections;
using UnityEngine;

/// <summary>
/// Gắn vào GameObject "Cột" (Column/Pole) của lâu đài.
/// Khi Player chạm vào Trigger của cột:
///   1. Cột di chuyển xuống (lerp xuống đất).
///   2. Thông báo cho CastleGate mở cửa.
///   3. Gọi LevelManager.LevelComplete().
/// </summary>
public class CastleColumn : MonoBehaviour
{
    [Header("Column Drop Settings")]
    [Tooltip("Cột sẽ di chuyển xuống bao nhiêu đơn vị?")]
    [SerializeField] private float dropDistance = 3f;

    [Tooltip("Thời gian (giây) để cột trượt xuống")]
    [SerializeField] private float dropDuration = 1f;

    [Header("References")]
    [Tooltip("Script CastleGate trên cánh cửa lâu đài")]
    [SerializeField] private CastleGate castleGate;

    [Tooltip("Tag của Player (mặc định: Player)")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Bao nhiêu giây sau khi cột chạm đất thì gọi LevelComplete?")]
    [SerializeField] private float completionDelay = 1.2f;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag(playerTag)) return;

        hasTriggered = true;

        // Vô hiệu hóa input của player để lock lại (tuỳ chọn)
        var pc = other.GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        StartCoroutine(DropColumn());
    }

    private IEnumerator DropColumn()
    {
        // --- Bước 1: Cột trượt xuống ---
        Vector3 startPos = transform.position;
        Vector3 endPos   = startPos + Vector3.down * dropDistance;
        float   elapsed  = 0f;

        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / dropDuration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        transform.position = endPos;

        // --- Bước 2: Cửa mở ---
        if (castleGate != null)
            castleGate.OpenGate();

        // --- Bước 3: Chờ rồi gọi LevelComplete ---
        yield return new WaitForSeconds(completionDelay);
        LevelManager.Instance?.LevelComplete();
    }
}
