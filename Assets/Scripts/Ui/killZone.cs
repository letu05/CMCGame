using UnityEngine;

public class killZone : MonoBehaviour
{
    // Khi player chạm vào vùng này → chết ngay lập tức
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth == null)
            playerHealth = collision.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.Die();
        }
    }
}
