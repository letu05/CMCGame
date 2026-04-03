using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    [SerializeField]
    private int TimeToLive = 10;   // Thời gian tồn tại (giây)

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= TimeToLive)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Chạm Player → kích hoạt shield
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerShield playerShield = collision.gameObject.GetComponent<PlayerShield>();
            if (playerShield != null)
            {
                playerShield.ActivateShield();
                Debug.Log("[ShieldPickup] Player nhặt shield!");
            }
            else
            {
                Debug.LogWarning("[ShieldPickup] Không tìm thấy PlayerShield trên player!");
            }
            Destroy(gameObject);
        }
    }
}
