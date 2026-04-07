using System.Collections;
using UnityEngine;

/// <summary>
/// Hoa ăn thịt kiểu Mario (Piranha Plant).
/// Đứng yên tại chỗ, nhô lên và rút xuống theo chu kỳ.
/// Gây sát thương player khi va chạm.
///
/// --- Cách setup trong Unity ---
/// 1. Tạo GameObject "PiranhaPlant".
/// 2. Gắn script này + PiranhaPlantHealth + Collider2D (Is Trigger) + Rigidbody2D (Kinematic).
/// 3. Tạo 2 GameObject con rỗng: "VisiblePoint" (vị trí nhô ra) và "HiddenPoint" (vị trí ẩn).
/// 4. Kéo 2 GameObject đó vào field visiblePoint và hiddenPoint trong Inspector.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PiranhaPlant : MonoBehaviour
{
    [Header("Timing (giây)")]
    [SerializeField] private float riseTime   = 0.5f;   // Thời gian nhô lên
    [SerializeField] private float stayTime   = 2.0f;   // Thời gian ở trên
    [SerializeField] private float hideTime   = 0.5f;   // Thời gian rút xuống
    [SerializeField] private float waitTime   = 1.0f;   // Thời gian ẩn trước khi nhô lên lại

    [Header("Vị trí (kéo Transform vào)")]
    [SerializeField] private Transform visiblePoint; // Điểm hoa nhô ra (trên)
    [SerializeField] private Transform hiddenPoint;  // Điểm hoa ẩn (dưới, trong ống)

    [Header("Sát thương")]
    [SerializeField] private int   damageAmount = 1;    

    // ── trạng thái nội tại ──────────────────────────────────────────────────
    private Vector3 _visiblePos;
    private Vector3 _hiddenPos;
    private bool    _isActive;

    // Tránh gây sát thương nhiều lần cùng frame
    private float   _damageCooldown = 0.5f;
    private float   _nextDamageTime;

    private Collider2D _col;

    // ── khởi tạo ────────────────────────────────────────────────────────────
    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _col.isTrigger = true;
    }

    private void Start()
    {
        if (visiblePoint == null || hiddenPoint == null)
        {
            Debug.LogError("[PiranhaPlant] Chưa gán visiblePoint hoặc hiddenPoint trong Inspector!", this);
            return;
        }

        _visiblePos = visiblePoint.position;
        _hiddenPos  = hiddenPoint.position;

        // Bắt đầu ở vị trí ẩn
        transform.position = _hiddenPos;

        StartCoroutine(PlantCycle());
    }

    // ── vòng lặp chính ──────────────────────────────────────────────────────
    private IEnumerator PlantCycle()
    {
        while (true)
        {
            // --- CHỜ ---
            _isActive = false;
            yield return new WaitForSeconds(waitTime);

            // --- NHÔ LÊN ---
            _isActive = true;
            yield return MoveTo(_hiddenPos, _visiblePos, riseTime);

            // --- Ở TRÊN ---
            yield return new WaitForSeconds(stayTime);

            // --- RÚT XUỐNG ---
            _isActive = false;
            yield return MoveTo(_visiblePos, _hiddenPos, hideTime);
        }
    }

    // Nội suy vị trí (linear) trong thời gian duration
    private IEnumerator MoveTo(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // Dùng SmoothStep để chuyển động mượt hơn
            float smooth = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.LerpUnclamped(from, to, smooth);
            yield return null;
        }
        transform.position = to;
    }

    // ── gây sát thương ──────────────────────────────────────────────────────
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_isActive) return;
        if (Time.time < _nextDamageTime) return;

        // Chỉ tấn công player
        var takeDamage = other.GetComponentInParent<IcanTakeDamage>();
        if (takeDamage == null)
            takeDamage = other.GetComponent<IcanTakeDamage>();

        if (takeDamage != null)
        {
            takeDamage.TakeDamage(damageAmount);
            _nextDamageTime = Time.time + _damageCooldown;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerStay2D(other); // xử lý ngay khi bước vào
    }
}
