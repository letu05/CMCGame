using System.Collections;
using UnityEngine;

/// <summary>
/// Máu / xử lý chết cho Piranha Plant.
/// Gắn vào cùng GameObject với PiranhaPlant.
/// Tích hợp IcanTakeDamage để nhận đạn từ PlayerFire.
/// </summary>
public class PiranhaPlantHealth : MonoBehaviour, IcanTakeDamage
{
    [Header("Máu")]
    [SerializeField] private int maxHp = 3;

    [Header("Điểm thưởng")]
    [SerializeField] private int score = 20;

    [Header("Hiệu ứng chết (tuỳ chọn)")]
    [SerializeField] private GameObject deathEffectPrefab; // particle / animation khi chết

    [Header("Hiệu ứng trúng đạn")]
    [SerializeField] private float flashDuration = 0.1f;   // thời gian nháy trắng

    private int          _currentHp;
    private SpriteRenderer _sr;
    private bool         _isDead = false;

    private void Start()
    {
        _currentHp = maxHp;
        _sr = GetComponent<SpriteRenderer>();
    }

    // ── nhận sát thương ─────────────────────────────────────────────────────
    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHp -= damage;

        // Nháy trắng khi bị đánh
        if (_sr != null)
            StartCoroutine(FlashWhite());

        if (_currentHp <= 0)
            Die();
    }

    private void Die()
    {
        _isDead = true;

        // Cộng điểm
        GameManager.Instance?.AddScore(score);

        // Đăng ký kill cho achievement (nếu cần)
        AchievementManager.Instance?.RegisterEnemyKill();

        // Sinh hiệu ứng nếu có
        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    // ── hiệu ứng nháy trắng ─────────────────────────────────────────────────
    private IEnumerator FlashWhite()
    {
        if (_sr == null) yield break;
        Color original = _sr.color;
        _sr.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        if (_sr != null) // kiểm tra chưa bị Destroy
            _sr.color = original;
    }
}
