using UnityEngine;

/// <summary>
/// Script sức khoẻ cơ bản cho quái.
/// Gắn vào cùng GameObject với Enemy / EnemyAl.
/// </summary>
public class EnemyHealth : MonoBehaviour, IcanTakeDamage
{
    [SerializeField] private int maxHp = 3;
    private int currentHp;
    [SerializeField] private int score = 10;
    private void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0) Die();
    }

    private void Die()
    {
        GameManager.Instance?.AddScore(score);
        AchievementManager.Instance?.RegisterEnemyKill();
        Destroy(gameObject);
    }

    /// <summary>Bị player giẫm lên đầu → chết ngay.</summary>
    public void Stomp()
    {
        Die();
    }
}
