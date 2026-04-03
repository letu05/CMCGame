using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private int coinValue = 10; // Số coin nhận được khi nhặt

    // Dùng khi Collider có Is Trigger = true
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    // Dùng khi Collider có Is Trigger = false (va chạm vật lý thường)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (coinSound != null)
            AudioSource.PlayClipAtPoint(coinSound, transform.position);

        GameManager.Instance?.AddCoin(coinValue);
        Destroy(gameObject);
    }
}
