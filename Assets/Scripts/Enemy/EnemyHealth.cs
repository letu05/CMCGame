using UnityEngine;

/// <summary>
/// Script sức khoẻ cơ bản cho quái.
/// Gắn vào cùng GameObject với Enemy / EnemyAl.
/// </summary>
public class EnemyHealth : MonoBehaviour, IcanTakeDamage
{
    [SerializeField] private int maxHp = 3;
    private int currentHp;

    private void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"[EnemyHealth] {gameObject.name} bị {damage} sát thương. HP còn: {currentHp}");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[EnemyHealth] {gameObject.name} đã chết.");
        // TODO: thêm animation chết, điểm, effect tuỳ project
        Destroy(gameObject);
    }
}
