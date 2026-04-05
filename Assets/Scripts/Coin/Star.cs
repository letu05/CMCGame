using UnityEngine;

/// <summary>
/// Gắn vào từng ngôi sao trong scene.
/// Mỗi màn có đúng 3 sao (starIndex = 1, 2, 3).
/// Khi player chạm vào → báo LevelManager cập nhật UI.
/// </summary>
public class Star : MonoBehaviour
{
    [SerializeField] private int starIndex = 1; // 1, 2 hoặc 3
    [SerializeField] private AudioClip starSound;
    [SerializeField] private int scoreBonus = 1000;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Collect();
    }

    private void OnCollisionTrigger2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            Collect();
    }

    private void Collect()
    {
        if (starSound != null)
            AudioSource.PlayClipAtPoint(starSound, transform.position);

        GameManager.Instance?.AddScore(scoreBonus);
        LevelManager.Instance?.CollectStar(starIndex);
        Destroy(gameObject);
    }
}
